using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Helper;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Utils;
using ANFAPP.Pages;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.PharmacyLocator
{
	public partial class DashboardLocator : ANFPage
	{
		private Location _lastLocation;

		#region Page Initialization

		public DashboardLocator() : base() { }

		/// <summary>
		/// Initializes the page
		/// </summary>
		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			//			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
			//			var props = new Dictionary<string,string> ();
			//			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			//			mixpanelWidget.TrackProperties ("PharmacyLocator", props);
		}

		#endregion

		#region inicialization

		void GetSettings() {
			//toggler state
			BtnTogglerPharmacy.State = SessionData.IsAllPharmacies;
		}

		#endregion

		#region Application Bar Methods

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

		protected override string GetAppBarTitle()
		{
			return AppResources.TitleLocator;
		}

		#endregion

		#region Actions

		public async void NearMeAction(object sender, EventArgs args) 
		{
			var locationServices = DependencyService.Get<ILocationServices>();

			if (!locationServices.LocationServicesAvailable ()) 
			{
                await DisplayAlert(AppResources.LocationErrorDisabledTitle, AppResources.LocationErrorDisabledMessage, AppResources.OK);	
			} 
         else if (SessionData.UserLocation == null && _lastLocation == null) 
			{
                await DisplayAlert(AppResources.LocationErrorNotFoundTitle, AppResources.LocationErrorNotFoundMessage, AppResources.OK);	
			} 
			else 
			{
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync (new LocatorMap(null, MenuCurrentSelection.NEAR_ME, _lastLocation ?? SessionData.UserLocation));
			}
		}

		async void ByLocalityAction (object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new LocatorSearch(LocatorDataType.DISTRITOS));
		}

		async void ByServicesAction (object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new LocatorSearchServices());
		}

		async void FavouritsAction(object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync (new LocatorMap(null, MenuCurrentSelection.FAVORITES));
		}

		async void RecentAction(object sender, EventArgs args) 
		{
      		LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync (new LocatorMap(null, MenuCurrentSelection.RECENT));
		}

		void TogglerAction(object sender, EventArgs args) 
		{
            BtnTogglerPharmacy.State = !BtnTogglerPharmacy.State;
			SessionData.IsAllPharmacies = BtnTogglerPharmacy.State;
		}
			
		#endregion

		#region Actions

		void OnSearchTextChanged(object sender, TextChangedEventArgs args) 
		{
			SearchBarButtonImage.Source = (string.IsNullOrEmpty(args.NewTextValue)) ? "magnifier" : "ic_delete";
		}

		public async void OnSearchEntryCompleted(object sender, EventArgs args)
		{
			if (string.IsNullOrWhiteSpace(SearchEntry.Text) || SearchEntry.Text.Length < 3) 
			{
				await DisplayAlert(null, AppResources.PharmacyLocatorMinCharactersError, AppResources.OK);
				return;
			}

			LoadingView.IsVisible = true;

			LocalizationDAO lDAO = new LocalizationDAO();
			var queryStr = await lDAO.GetLocalsWithString(SearchEntry.Text);

			try
			{
				var pharmacies = await PharmacyHelper.GetPharmacies(queryStr);

				if (pharmacies == null || pharmacies.Count == 0)
				{
					/// No results
					await DisplayAlert(null, AppResources.PharmacyNoResultsMessage, AppResources.OK);
				} else {
					await Navigation.PushAsync(new LocatorMap(pharmacies, MenuCurrentSelection.NEAR_ME));
				}
			}
			catch (Exception e)
			{
				// Service error
				ShowErrorDialog(e.Message);
			}

			LoadingView.IsVisible = false;
		}

		public void ClearInputClick(object sender, EventArgs args) 
		{
			SearchEntry.Text = string.Empty;
		}

        private async void ShowErrorDialog(string message)
        {
            await DisplayAlert(null, message, AppResources.OK);
        }

		#endregion

		#region Page Lifecylce

		protected override void OnAppearing()
		{
			MessagingCenter.Subscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION, (userLocation) => {
				SessionData.UserLocation = userLocation;
				_lastLocation = userLocation;
			});

			base.OnAppearing();
            
            GetSettings();

            LoadingView.IsVisible = false;
			DependencyService.Get<ILocationServices> ().StartUpdatingLocation ();

			//MessagingCenter.Subscribe<string>(this, Settings.MS_LOCATOR_NOT_AUTORIZED, (sender) => {});
		}


		protected override void OnDisappearing()
		{            
			base.OnDisappearing();

			MessagingCenter.Unsubscribe<Location> (this, Settings.MS_LOCATOR_GOT_LOCATION);
			//MessagingCenter.Unsubscribe<string> (this, Settings.MS_LOCATOR_NOT_AUTORIZED);
			foreach (var fav in SessionData.FavoritePharmacies)
			{
				try
				{
					fav.Distance = Math.Round(PharmacyHelper.DistanceTo(_lastLocation.Latitude, _lastLocation.Longitude, fav.Latitude, fav.Longitude), 2);
					fav.DistanceTxt = "Distancia: " + fav.Distance + " km";
				}
				catch (Exception ex) { }


			}
		}

		#endregion

	}
}
