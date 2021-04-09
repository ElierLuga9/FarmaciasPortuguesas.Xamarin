using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Pages;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;

using System.Diagnostics;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Helper;
using System.Threading.Tasks;
using ANFAPP.Logic.Network.Services;
using Xamarin.Forms.Maps;
using System.Net.Http;
using System.Globalization;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.ViewModels;

namespace ANFAPP.Pages.PharmacyLocator
{
	public enum MenuState
	{
		MenuStateDefault,
		MenuStateOptions,
		MenuStateRecent,
	}

	public partial class LocatorMap : ANFPage
	{
		private List<Pharmacy> _originalPharmacies;
		private Location _currentUserLocation;
		private MenuTextViewModel _mtVM;
		private PharmacyViewModel _pVM;
		private PharmacyDAO _pDAO;

		private MenuCurrentSelection _currentSelection = MenuCurrentSelection.NONE;
		private bool _isInSearchMode = false;
		private bool _keepViewModeChanges = true;
		private bool isMapToggle = false;
		private bool _isFromServiceTab = false;
		private bool isLoading = false;
		// These would be used when updating the map region.
		// private MapSpan _prevSpan;
		// private bool _updatingPharmacies;

		public LocatorMap()
			: base()
		{
		}

		public LocatorMap(List<Pharmacy> pharmacies, MenuCurrentSelection cSelection, bool? isMapView = null, bool keepViewModeChanges = true)
			: base()
		{
			_currentUserLocation = SessionData.UserLocation;
			_pVM = new PharmacyViewModel();
			_mtVM = new MenuTextViewModel();
			_pDAO = new PharmacyDAO();

			_keepViewModeChanges = keepViewModeChanges;
			_originalPharmacies = pharmacies;
			_currentSelection = cSelection;

			InitData();
			InitInterface(isMapView);
            if (_originalPharmacies != null)
                UpdateMapAndList(false);
        }

		public LocatorMap(List<Pharmacy> pharmacies, MenuCurrentSelection cSelection, bool isFromServiceTab ,bool? isMapView = null, bool keepViewModeChanges = true)
			: base()
		{
			_currentUserLocation = SessionData.UserLocation;
			_pVM = new PharmacyViewModel();
			_mtVM = new MenuTextViewModel();
			_pDAO = new PharmacyDAO();

			_keepViewModeChanges = keepViewModeChanges;
			_originalPharmacies = pharmacies;
			_currentSelection = cSelection;
			_isFromServiceTab = isFromServiceTab;
			InitData();
			InitInterface(isMapView);
			if (_originalPharmacies != null)
				UpdateMapAndList(false);
		}


		public LocatorMap(List<Pharmacy> pharmacies, MenuCurrentSelection cSelection, Location location)
			: this(pharmacies, cSelection)
		{
			if (null != location)
			{
				_currentUserLocation = location;
			}
		}

		void InitData()
		{
			BindingContext = _pVM;

            SessionData.IsOnlyServicePharmacies = true;
            SessionData.isOnlyLongSchedulePharmacies = false;

            if (null != _originalPharmacies)
			{
				_pVM.SetData(_originalPharmacies);
				PharmacyMap.PharmacyList = _originalPharmacies;

			}

			// Bindings for the menu.
			ListOptions.BindingContext = _mtVM;
			ListOptions.ItemSelected += MenuItemSelected;

			RecentList.BindingContext = _mtVM;
			RecentList.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem == null)
					return;
				GetSelectedPharmacy((MenuText)e.SelectedItem);
			};

            containerInServiceVsLongSchedule.IsVisible = !SessionData.IsAllPharmacies;
            btnShowInServicePharms.Clicked += BtnShowInServicePharms_Clicked;
            btnShowLongSchedulePharms.Clicked += BtnShowLongSchedulePharms_Clicked;
        }

        private void BtnShowInServicePharms_Clicked(object sender, EventArgs e)
        {
            SessionData.IsOnlyServicePharmacies = true;
            SessionData.isOnlyLongSchedulePharmacies = false;
            btnShowInServicePharms.BackgroundColor = ANFAPP.Logic.Resources.ColorResources.ANFWhite;
            btnShowLongSchedulePharms.BackgroundColor = ANFAPP.Logic.Resources.ColorResources.LocatorSeparatorColorLight;

            UpdateMapAndList(false);
        }

        private void BtnShowLongSchedulePharms_Clicked(object sender, EventArgs e)
        {
            SessionData.IsOnlyServicePharmacies = false;
            SessionData.isOnlyLongSchedulePharmacies = true;

            btnShowLongSchedulePharms.BackgroundColor = ANFAPP.Logic.Resources.ColorResources.ANFWhite;
            btnShowInServicePharms.BackgroundColor = ANFAPP.Logic.Resources.ColorResources.LocatorSeparatorColorLight;

            UpdateMapAndList(false);
        }

        private void InitInterface(bool? isMapView = null)
		{
			UpdateCurrentSelectionLabel();

			var mapView = isMapView.HasValue ? isMapView.Value : SessionData.IsMapView;

			// Setup the initial state of the toggles and other views.
			BtnTogglerViewType.State = !mapView;
			BtnTogglerPharmacy.State = SessionData.IsAllPharmacies;
			PharmacyListContainer.IsVisible = !mapView;

			if (Device.OS != TargetPlatform.iOS) {
				// Fixing an issue that happened only in iOS, where the map wouldn't appear when the list was toggle on by default.
				MapView.IsVisible = mapView;				
			}

		}

		private void MoveToDefaultLocation()
		{
			MapSpan startingRegion = new MapSpan(new Position(38.42, -9.8), 3.0, 4.0);
			PharmacyMap.MoveToRegion(startingRegion);
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			// Setup the initial map region.
			MoveToDefaultLocation();
		}

		#region Application Bar Methods

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.TitleMapLocator;
		}

		#endregion


		#region Events

		public async void DidSelectPharmacyItem(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null)
				return;

			var pharmacyPage = new LocatorPharmacyDetails(args.SelectedItem as Pharmacy);
			await Navigation.PushAsync(pharmacyPage);

			SetMenuState(MenuState.MenuStateDefault);
			((ListView)sender).SelectedItem = null;
		}

		public async void TogglerViewTypeAction(object sender, EventArgs args)
		{

			var isMapView = BtnTogglerViewType.State;
			if (_keepViewModeChanges) SessionData.IsMapView = isMapView;

			PharmacyListContainer.IsVisible = !isMapView;
			MapView.IsVisible = isMapView;
			BtnTogglerViewType.State = !isMapView;
			var location = _currentUserLocation ?? SessionData.UserLocation;

			//if for near me start at list
			if (location != null && MapView.IsVisible && isMapView && isMapToggle == true )
			{
				isMapToggle = false;

				//await FetchNearbyPharmacies(Settings.LOCATOR_MAP_NEARME_RADIUS);



				//HideActivityIndicator();
				await UpdateMapAndList(false);
				// Calculate offset
				var offsetDeg = GeoUtils.ConvertZoomLevelToDegree(Settings.LOCATOR_MAP_NEARME_ZOOMLEVEL);

				// Move to region


				PharmacyMap.MoveToRegion(
				new MapSpan(
					new Position(location.Latitude, location.Longitude),
					offsetDeg, offsetDeg));


			}

			if (MapView.IsVisible)
			{
				//teste
					//MoveToDefaultLocation();
					//await ShowNearMe();
					//MoveToDefaultLocation();


				if (PharmacyMap.PinToAnnotations != null)
				{
					PharmacyMap.PinToAnnotations();
				}
			}
		}

		public async void PharmaciesToggleClick(object sender, EventArgs args)
		{
			SessionData.IsAllPharmacies = !SessionData.IsAllPharmacies;
			BtnTogglerPharmacy.State = SessionData.IsAllPharmacies;
            containerInServiceVsLongSchedule.IsVisible = !SessionData.IsAllPharmacies;
            await UpdateMapAndList(false);
		}

		public void MenuAction(object sender, EventArgs args)
		{
			SetListForMenu();
			SetMenuState(MenuState.MenuStateOptions);
		}

		public void MenuActionExpanded(object sender, EventArgs args)
		{
			_currentSelection = MenuCurrentSelection.NONE;
			SetMenuState(MenuState.MenuStateDefault);
		}

		public void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.NewTextValue) && SearchOptions.IsVisible == true)
				SetMenuState(MenuState.MenuStateRecent);
			else
				SetMenuState(MenuState.MenuStateDefault);
		}

		public async void OnSearchEntryCompleted(object sender, EventArgs args)
		{
			if (string.IsNullOrWhiteSpace(SearchEntry.Text) || SearchEntry.Text.Length < 3) 
			{
				await DisplayAlert(null, AppResources.PharmacyLocatorMinCharactersError, AppResources.OK);
				return;
			}

			_isInSearchMode = !_isInSearchMode;

			SearchEntry.Unfocus();

			GridSearch.IsVisible = true;

			ShowActivityIndicator();

			try
			{
				LocalizationDAO lDAO = new LocalizationDAO();
				var queryStr = await lDAO.GetLocalsWithString(SearchEntry.Text);
				var pharmacies = await PharmacyHelper.GetPharmacies(queryStr);

				_pVM.SetData(pharmacies);
				_originalPharmacies = pharmacies;

				await UpdateMapAndList();

				if (pharmacies == null || pharmacies.Count == 0)
				{
					/// No results
					await DisplayAlert(null, AppResources.PharmacyNoResultsMessage, AppResources.OK);
				}
			}
			catch (Exception e)
			{
				// Service Error
				ShowErrorDialog(e.Message);
			}

			HideActivityIndicator();	
		}

		public async void ActionSearch(object sender, EventArgs args)
		{
			_isInSearchMode = !_isInSearchMode;

			if (_isInSearchMode && string.IsNullOrEmpty(SearchEntry.Text))
			{
				SearchBarButtonImage.Source = "ic_delete";

				// Show Input
				SearchHeaderTitle.Text = AppResources.Recent;
				GridSelection.IsVisible = false;
				GridSearch.IsVisible = true;

				SearchEntry.Focus();

				await _mtVM.UpdateRecentPharmacies();
				SetMenuState(MenuState.MenuStateRecent);

			}
			else if (!_isInSearchMode && string.IsNullOrEmpty(SearchEntry.Text))
			{
				SearchBarButtonImage.Source = "magnifier";

				// Hide Input
				GridSearch.IsVisible = false;
				GridSelection.IsVisible = true;

				SearchEntry.Unfocus();

				SetMenuState(MenuState.MenuStateDefault);
			}
			else
			{
				// Clear text and hide input
				SearchEntry.Text = string.Empty;
				_isInSearchMode = !_isInSearchMode;

				// Go back to default state
				ActionSearch(null, null);

			}
		}

		public async void MenuItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null)
				return;
			
			var selection = (MenuText)args.SelectedItem;

			// See `MenuCurrentSelection`.
			switch (selection.Id)
			{
				case "0":
					await ShowNearMe();
					_currentSelection = MenuCurrentSelection.NEAR_ME;
					SessionData.allPharmacies.Clear();
					break;
				case "1":
					await GoToLocation();
					SessionData.allPharmacies.Clear();
					break;
				case "2":
					await GoToServices ();
					break;
				case "3":
					await ShowFavourites();
					_currentSelection = MenuCurrentSelection.FAVORITES;
					SessionData.allPharmacies.Clear();

					break;
				case "4":
					await ShowRecentPharmacies();
					_currentSelection = MenuCurrentSelection.RECENT;
					SessionData.allPharmacies.Clear();
					break;
				default:
					break;
			}

			if (selection.Id != "1")
			{
				ClearSearchText();
				UpdateCurrentSelectionLabel();
			}
			ListOptions.SelectedItem = null;
		}

		private async void ShowErrorDialog(string message)
		{
			await DisplayAlert(null, message, AppResources.OK);
		}

		#endregion


		#region Helper methods

		private async Task<Tuple<string, string, string>> GetAddressForLocation(Location location)
		{
			try
			{
				var addr = await GeoCoderWS.GetReverseGeocoding(location);
				return addr;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.StackTrace);
			}

			return null;
		}

		private void ShowActivityIndicator()
		{
			LoadingView.IsVisible = true;
		}

		private void HideActivityIndicator()
		{
			LoadingView.IsVisible = false;
		}

		/// <summary>
		/// Updates the map and list with the current pharmacies.
		/// </summary>
		private async Task UpdateMapAndList()
		{
			await UpdateMapAndList(true);

			if (PharmacyMap.PharmacyList.Count == 0)
			{
				MoveToDefaultLocation();
			}
		}

		/// <summary>
		/// Updates the map and list with the current pharmacies.
		/// 
		/// FIXME: the map and list should track the view model property changes, 
		/// for instance, when the selection changes from locations to favorites.
		/// </summary>
		private async Task UpdateMapAndList(bool adjustRegionToAnnotation)
		{
			
			PharmacyMap.AdjustRegionToAnnotations = adjustRegionToAnnotation;


			if (adjustRegionToAnnotation == true)
			{
				SessionData.IsOnlyServicePharmacies = false;
				//var a = await PharmacyHelper.GetPharmMap(SessionData.SelectedService);
				//PharmacyMap.PharmacyList = a;

			}
			// Show Loading
			ShowActivityIndicator();
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);



			if (SessionData.IsAllPharmacies)
			{
                var listMap = new List<Pharmacy>();

                if (null != _pVM.Pharmacies)
				{
                    listMap = new List<Pharmacy>(_pVM.Pharmacies);
					PharmacyList.ItemsSource = listMap;
					PharmacyMap.PharmacyList = listMap;

					SessionData.IsServicePagePharm = false;
				}
			}
			else
			{
                var listMap = new List<Pharmacy>();
                var listModeList = new List<Pharmacy>();

				if (SessionData.IsOnlyServicePharmacies) {
					// Service Only
					listModeList.AddRange(_pVM.ServicePharmacies);

				} else {
					// Long Schedule
					listModeList.AddRange(_pVM.LongSchudlePharmacies);
              		
				}

				listMap.AddRange(_pVM.ServicePharmacies);
				listMap.AddRange(_pVM.LongSchudlePharmacies);

                PharmacyMap.PharmacyList = listMap;
				PharmacyList.ItemsSource = listModeList;
			}

			HideActivityIndicator();

			if (_currentSelection == MenuCurrentSelection.NEAR_ME)
			{
				PharmacyMap.AdjustRegionToAnnotations = false;
				//MoveToDefaultLocation();
			}
			else {
				PharmacyMap.AdjustRegionToAnnotations = true;
			}

			_pVM.Notify();

		}
		public async Task<List<Pharmacy>> GetPharmMap(int serviceId, ICollection<Pharmacy> a)
		{
			DateTime lowerBound = DateTime.UtcNow.AddDays(-1);
			var isoTime = lowerBound.ToString("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);


			//var allPharmacies = new List<Pharmacy> ();
			string auxFilter = string.Format(Settings.WS_PHARMACY_SERVICES_FILTER, serviceId, isoTime);
			//Application.Current.MainPage.
			var pharmaciesOut = await PharmacyWS.GetPharmacies(auxFilter);
			var pharmacies = await PharmacyHelper.PharmaciesOutToDAO(pharmaciesOut, false);
			var allPharmacies = new List<Pharmacy>();


			while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink))
			{

				try
				{
					pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
					if (null != pharmaciesOut)
					{
						pharmacies = await PharmacyHelper.PharmaciesOutToDAO(pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

						allPharmacies.AddRange(pharmacies);
						PharmacyMap.PharmacyList = allPharmacies;

						if (SessionData.IsOnlyServicePharmacies) 
						{
							
						}


					}
					// Stop subsequent requests on network errors.
				}
				catch (HttpRequestException)
				{
					break;
				}
				catch (NetworkingException)
				{
					break;
				}
				catch (InvalidRequestException)
				{
					break;
				}
				catch (TimeoutException)
				{
					break;
				}

			}
			var sorted = await PharmacyHelper.SortPharmacies(allPharmacies);
			return sorted;
		}
		private void SetMenuState(MenuState menuState)
		{
			switch (menuState)
			{

				case MenuState.MenuStateDefault:
					MenuClosed.IsVisible = true;
					MenuExpanded.IsVisible = false;
					SearchOptions.IsVisible = false;
					Shadowbox.IsVisible = false;
					break;

				case MenuState.MenuStateOptions:
					MenuClosed.IsVisible = false;
					MenuExpanded.IsVisible = true;
					SearchOptions.IsVisible = false;
					Shadowbox.IsVisible = true;
					break;

				case MenuState.MenuStateRecent:
					MenuClosed.IsVisible = false;
					MenuExpanded.IsVisible = false;
					SearchOptions.IsVisible = true;
					Shadowbox.IsVisible = true;
					break;
			}
		}

		void SetListForMenu()
		{
			_mtVM.SetMenuEntrys();
		}

		/// <summary>
		/// Fetch the nearby pharmacies by a set radius (in meters)
		/// </summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		private async Task FetchNearbyPharmacies(double offset)
		{
			if (_currentUserLocation != null || SessionData.UserLocation != null)
			{
				try
				{
					// _updatingPharmacies = true;

					var location = _currentUserLocation ?? SessionData.UserLocation;

					Location tl = GeoUtils.OffsetCoordinate(location, -offset, offset);
					Location br = GeoUtils.OffsetCoordinate(location, offset, -offset);
					_originalPharmacies = await PharmacyHelper.GetPharmaciesInBBOX(tl, br);

					System.Diagnostics.Debug.WriteLine("CENTER: {0} TL: {1} BR: {2}", location, tl, br);

					if (null != _originalPharmacies)
					{
						_pVM.SetData(_originalPharmacies);
					}
					// _updatingPharmacies = false;

				}
				catch (Exception e)
				{
					ShowErrorDialog(e.Message);
				}
			}
		}

		private async Task ShowNearMe()
		{
			isMapToggle = true;
			SetMenuState(MenuState.MenuStateDefault);
			ShowActivityIndicator();

#if GET_LOCATION_BY_DISTRICT
            if (_currentUserLocation != null || SessionData.UserLocation != null) {
            
					LocalizationDAO lDAO = new LocalizationDAO ();
					 Tuple<string,string,string> ids = null;
                var addr = await GetAddressForLocation (_currentUserLocation ?? SessionData.UserLocation);
                if (addr != null && addr.Item3 != null && addr.Item2 != null && addr.Item3 != null) {
                    ids = await lDAO.GetIdsForDescriptions (addr.Item1, addr.Item2, addr.Item3);
                }

                try {
                    if (ids != null) {
                        _originalPharmacies = await PharmacyHelper.GetPharmaciesByParish (ids.Item1, ids.Item2, ids.Item3);
                    } else if (addr.Item2 != null) {
                        var nearMeQuery = await lDAO.GetQueryForCounty (addr.Item2.ToUpper ());
                        if (!string.IsNullOrWhiteSpace (nearMeQuery))
                            _originalPharmacies = await PharmacyHelper.GetPharmacies (nearMeQuery);
                    }

                    if (null != _originalPharmacies) {
                        _pVM.SetData (_originalPharmacies);
                    }
                } catch (Exception e) {
                    // Service Error
                    ShowErrorDialog (e.Message);
                }
            }
#else

			// Fetch the nearby pharmacies by a set radius
			await FetchNearbyPharmacies(Settings.LOCATOR_MAP_NEARME_RADIUS);

#endif

			HideActivityIndicator();
			await UpdateMapAndList(false);

			// Focus visible region on the current position
			var location = _currentUserLocation ?? SessionData.UserLocation;

			if (location != null && MapView.IsVisible)
			{
				// Calculate offset
				var offsetDeg = GeoUtils.ConvertZoomLevelToDegree(Settings.LOCATOR_MAP_NEARME_ZOOMLEVEL);

				// Move to region


					PharmacyMap.MoveToRegion(
					new MapSpan(
						new Position(location.Latitude, location.Longitude),
						offsetDeg, offsetDeg));
			

 			}
		}

		private async Task ShowFavourites()
		{
			SetMenuState(MenuState.MenuStateDefault);
			ShowActivityIndicator();

			LocalizationDAO lDAO = new LocalizationDAO();
			var pharmacies = await lDAO.GetFavourits();
			if (_originalPharmacies == null)
			{
				_originalPharmacies = pharmacies;
			}
			//UserCardViewModel a = new UserCardViewModel();
			//await a.LoadFavorites();
			_pVM.SetData(SessionData.FavoritePharmacies);

			await UpdateMapAndList();
			HideActivityIndicator();
		}

		private async Task ShowRecentPharmacies()
		{
			SetMenuState(MenuState.MenuStateDefault);
			ShowActivityIndicator();

			LocalizationDAO lDAO = new LocalizationDAO();
			var pharmacies = await lDAO.GetRecent();
			if (_originalPharmacies == null)
			{
				_originalPharmacies = pharmacies;
			}

			_pVM.SetData(pharmacies);

			await UpdateMapAndList();
			HideActivityIndicator();
		}

		/// <summary>
		/// Go to the location selection.
		/// 
		///	This method changes the navigation stack.
		/// 
		/// </summary>
		async Task GoToLocation()
		{
			PharmacyList.SelectedItem = null;
			ListOptions.SelectedItem = null;

			var navStack = Navigation.NavigationStack;

			int dashboardIndexPage = 0;
			int locatorIndexPage = 0;
			List<Page> removePages = new List<Page>();
			for (int i = navStack.Count - 1; i > -1; i--)
			{
				if (navStack[i] is LocatorSearch)
				{
					removePages.Add(navStack[i]);
				}
				if (navStack[i] is DashboardLocator)
				{
					dashboardIndexPage = i;
				}
				if (navStack[i] is LocatorMap)
				{
					locatorIndexPage = i;
				}
			}
			//if dashboard page and locator page are not in sequence
			if ((dashboardIndexPage + 1) != locatorIndexPage)
			{
                if (removePages.Count > 1)
                {
                    Navigation.InsertPageBefore(new LocatorSearch(LocatorDataType.DISTRITOS), removePages[removePages.Count - 1]);
                }
                else
                {
                    Navigation.InsertPageBefore(new LocatorSearch(LocatorDataType.DISTRITOS), this);
                }

                foreach (Page page in removePages)
				{
					Navigation.RemovePage(page);
				}
			}
			else
			{
				Navigation.InsertPageBefore(new LocatorSearch(LocatorDataType.DISTRITOS), this);
			}

           
            await Navigation.PopAsync();
		}

		/// <summary>
		/// Go to the services selection.
		/// 
		///	This method changes the navigation stack.
		/// 
		/// </summary>
		async Task GoToServices()
		{
			PharmacyList.SelectedItem = null;
			ListOptions.SelectedItem = null;

			var navStack = Navigation.NavigationStack;

			int dashboardIndexPage = 0;
			int locatorIndexPage = 0;
			List<Page> removePages = new List<Page>();
			for (int i = navStack.Count - 1; i > -1; i--)
			{
				if (navStack[i] is LocatorSearchServices)
				{
					removePages.Add(navStack[i]);
				}
				if (navStack[i] is DashboardLocator)
				{
					dashboardIndexPage = i;
				}
				if (navStack[i] is LocatorMap)
				{
					locatorIndexPage = i;
				}
			}
			//if dashboard page and locator page are not in sequence
			if ((dashboardIndexPage + 1) != locatorIndexPage)
			{
                if (removePages.Count > 1)
                {
                    Navigation.InsertPageBefore(new LocatorSearchServices(), removePages[removePages.Count - 1]);
                }
                else
                {
                    Navigation.InsertPageBefore(new LocatorSearchServices(), this);
                }

				foreach (Page page in removePages)
				{
					Navigation.RemovePage(page);
				}
			}
			else
			{
				Navigation.InsertPageBefore(new LocatorSearchServices(), this);
			}

			await Navigation.PopAsync();
		}

		async void GetSelectedPharmacy(MenuText mt)
		{
			Pharmacy p = await _pDAO.GetPharmacyByID(mt.Id);
			var pharmacyPage = new LocatorPharmacyDetails(p);
			await Navigation.PushAsync(pharmacyPage);

			//GridOverlay.IsVisible = false;
			_currentSelection = MenuCurrentSelection.NONE;
		}

		void ClearSearchText()
		{
			SearchEntry.Text = "";
		}

		void UpdateCurrentSelectionLabel()
		{
			switch (_currentSelection)
			{
				case MenuCurrentSelection.FAVORITES:
					LblSelectedOption.Text = AppResources.Favorites;
					break;
				case MenuCurrentSelection.LOCALITY:
					LblSelectedOption.Text = AppResources.PlaceMenuOption;
					break;
				case MenuCurrentSelection.NEAR_ME:
					LblSelectedOption.Text = AppResources.NearMeOption;
					break;
				case MenuCurrentSelection.RECENT:
					LblSelectedOption.Text = AppResources.Recent;
					break;
				case MenuCurrentSelection.SERVICES:
					LblSelectedOption.Text = AppResources.Service;
					break;
				default:
					break;
			}
		}

		#endregion


		#region page lifecylce

		protected async override void OnAppearing()
		{
			if (Device.OS == TargetPlatform.Android)
			{
				MapView.VerticalOptions = LayoutOptions.FillAndExpand;
				PharmacyMap.VerticalOptions = LayoutOptions.FillAndExpand;
			}


			base.OnAppearing();
			if (_isFromServiceTab) 
			{
				PharmacyList.LoadMoreCommand = new Command(LoadNextPage);
			}

			//PharmacyList.LoadMoreCommand = null;
			//SessionData.allPharmacies.Clear();
			PharmacyList.SelectedItem = null;
			ListOptions.SelectedItem = null;
			// We only need to set this the first time this view appears.
			if (_originalPharmacies == null)
			{
				switch (_currentSelection)
				{
					case MenuCurrentSelection.RECENT:
						await ShowRecentPharmacies();
						break;
					case MenuCurrentSelection.FAVORITES:
						await ShowFavourites();
						break;
					case MenuCurrentSelection.LOCALITY:
                        await UpdateMapAndList(false);
                        break;
					case MenuCurrentSelection.NEAR_ME:
						await ShowNearMe();
						break;
					default:
                        break;
				}
			}


			MessagingCenter.Subscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION, (userLocation) =>
			{
				_currentUserLocation = userLocation;

			});
			MessagingCenter.Subscribe<Pharmacy>(this, Settings.MS_LOCATOR_PHARMACY_TAPPED, (sender) =>
			{
				var pharmacyPage = new LocatorPharmacyDetails(sender);
				Navigation.PushAsync(pharmacyPage);

			});

			if (_currentSelection == MenuCurrentSelection.LOCALITY && null != _originalPharmacies && _originalPharmacies.Count == 0)
			{
				await DisplayAlert(AppResources.PlaceMenuOption, "Não existem Farmácias nesta localidade", AppResources.OK);
			}
		}
		public async void LoadNextPage() 
		{
			isLoading = true;
			if (_isFromServiceTab) 
			{
				PharmacyHelper.LoadNext(); 
			}
		//	await Task.Delay(1500);
			isLoading = false;
		
		}
		protected override void OnDisappearing()
		{
			if (Device.OS == TargetPlatform.Android)
			{
				MapView.VerticalOptions = LayoutOptions.End;
				PharmacyMap.VerticalOptions = LayoutOptions.End;
			}

			base.OnDisappearing();
			SessionData.IsServicePagePharm = false;
            LoadingView.IsVisible = false;

			MessagingCenter.Unsubscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION);
			MessagingCenter.Unsubscribe<Pharmacy>(this, Settings.MS_LOCATOR_PHARMACY_TAPPED);
		}

		#endregion
	}
}
