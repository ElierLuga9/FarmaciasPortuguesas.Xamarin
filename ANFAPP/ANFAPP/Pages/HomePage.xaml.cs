using System;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Pages.Store;
using System.Threading.Tasks;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Objects;

namespace ANFAPP.Pages
{
    public partial class HomePage : ANFPage
    {

		#region Properties

		public UserCardViewModel _viewModel = new UserCardViewModel();
		private bool _initialized = false;

		private Location _lastLocation;


		#endregion

        #region Page Initialization

        public HomePage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();

            base.InitPage();
        }

        protected async override void OnAppearing()
        {
			base.OnAppearing();

			// Subscribe to location Services
			MessagingCenter.Subscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION, (userLocation) => {

				if (userLocation != null) {
					DependencyService.Get<ILocationServices> ().StopUpdatingLocation();

					SessionData.UserLocation = userLocation;
					_lastLocation = userLocation;

					UpdateBanners();
				}

			});
			DependencyService.Get<ILocationServices> ().StartUpdatingLocation ();

			LoadingView.IsVisible = false;
			SessionData.OnPharmacyChanged += PharmacyWidget.OnPharmacyChanged; 

			CampaignWidget.OnHeaderClicked += ShowCatalog;
			CampaignWidget.OnProductButtonClicked += OnLoadStarted;
            HighlightWidget.OnHeaderClicked += ShowHighlights;
			HighlightWidget.OnProductButtonClicked += OnLoadStarted;

			HomeSaudaWidget.OnWidgetClicked += OnLoadStarted;
			PharmacyWidget.OnWidgetClicked += OnLoadStarted;
			BannersWidget.OnWidgetClicked += OnLoadStarted;
			BannersWidget.OnLoadCompleteEvent += OnBannersLoadComplete;

			_viewModel.OnSuccess += OnUserProfileSuccessEvent;

			// XXX: the event attachment order matters!
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;

			if (!_initialized)
			{
				// Initialize page (aka token revalidation procedure)
				InitializePage();
			} 
			else  
			{
				// If initialized, reload only the necessary data
				LoadData();	
			}
        }

        protected override void OnDisappearing()
        {
			// Unsubscribe to location Services
			DependencyService.Get<ILocationServices> ().StopUpdatingLocation();
			MessagingCenter.Unsubscribe<Location> (this, Settings.MS_LOCATOR_GOT_LOCATION);

            base.OnDisappearing();

			SessionData.OnPharmacyChanged -= PharmacyWidget.OnPharmacyChanged;

            LoadingView.IsVisible = false;

            CampaignWidget.OnHeaderClicked -= ShowCatalog;
			CampaignWidget.OnProductButtonClicked -= OnLoadStarted;
            HighlightWidget.OnHeaderClicked -= ShowHighlights;
			HighlightWidget.OnProductButtonClicked -= OnLoadStarted;

			HomeSaudaWidget.OnWidgetClicked -= OnLoadStarted;
			PharmacyWidget.OnWidgetClicked -= OnLoadStarted;
			BannersWidget.OnWidgetClicked -= OnLoadStarted;
			BannersWidget.OnLoadCompleteEvent -= OnBannersLoadComplete;

			_viewModel.OnSuccess -= OnUserProfileSuccessEvent;

			App.StoreBasketVM.OnLoadError -= OnLoadError;
			App.StoreBasketVM.OnLoadSuccess -= OnLoadSuccess;
            App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
        }

		#endregion

		#region Loaders

		private async Task InitializePage() 
		{
			/*
			MessagingCenter.Subscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION, (userLocation) =>
				{
					SessionData.UserLocation = userLocation;
			});
			*/
			// Load Profile - reloads the ANF token if needed
			if (SessionData.HasPharmacyCard) {
				await _viewModel.LoadUserProfile();
				await _viewModel.LoadFavorites();
			}


			// Loads the pharmacy widget - reloads the Magento token if needed
			UpdateBanners();

			await LoadData();

			_initialized = true;
		}

		private async Task LoadData() 
		{
			// Get the basket count.
			if (SessionData.IsAuthenticatedWithPharmacy) 
			{
				await App.StoreBasketVM.PeekBasket();
			}
			App.StoreBasketVM.OnLoadStart += OnCartLoadStart;

			// Load the favorites if a pharmacy is selected and the favorites are not loaded yet
			if (SessionData.IsAuthenticatedWithPharmacy && !App.FavoritesVM.IsLoaded) 
			{
				App.FavoritesVM.LoadData ();
			}

			if (!_initialized) LoadWidgets();
		}

		private async void UpdateBanners() 
		{
			await BannersWidget.LoadData(_lastLocation);
		}


		private void LoadWidgets() 
		{
			PharmacyWidget.LoadData();
			//HomeSaudaWidget.LoadData();
			HighlightWidget.LoadData();
			CampaignWidget.LoadData();
		}

		#endregion

		#region Event Handlers

		protected async void ShowHighlights(object sender, EventArgs e)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new StoreHighlightsPage(false, AppResources.StoreHighlightsLabel));
		}

		protected async void ShowCatalog(object sender, EventArgs e)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new StorePointsCatalogPage());
		}

		public void OnUserProfileSuccessEvent()
		{
			// Refresh appbar points
			if (SessionData.IsLogged)
			{
				ApplicationBar.SetUserPoints(SessionData.PharmacyUser.Points);
				HomeSaudaWidget.ReloadData();
			}
		}
			
		async Task OnLoadStarted()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async Task OnCartLoadStart()
		{
			LoadingView.IsVisible = true;
			LoadingMessage.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		async Task OnBannersLoadComplete()
		{
			HomeSaudaWidget.LoadData();
		}

		#endregion

		#region Application Bar

		protected override bool HasBasketButton()
		{
			return SessionData.IsAuthenticated;
		}

		protected override bool HasFavoritesButton()
		{
			return SessionData.IsAuthenticated;
		}

        protected override bool HasCardButton()
        {
            return SessionData.IsAuthenticated;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE;
        //}

		protected override string GetAppBarTitle ()
		{
			return AppResources.ApplicationName;
		}

        #endregion
    }
}

