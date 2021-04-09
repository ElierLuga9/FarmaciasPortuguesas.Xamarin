using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.Store;
using ANFAPP.Pages.UserArea.Vouchers;
using ANFAPP.Views;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace ANFAPP.Pages
{
    public partial class PromotionsPageProducts : ANFPage
    {
        #region Properties

        public UserCardViewModel _viewModel = new UserCardViewModel();

     

        //testes
       // public VouchersViewModel _voucherViewModel { get; set; }
		//public PromotionsViewModel _promoViewModel = new PromotionsViewModel();
        private bool _isInitialized = false;
        private bool _vouchersAreInitialized = false;
        private bool _widgetIsInitialied = false;

        #endregion

        #region Page Initialization

		public PromotionsPageProducts() : base() { }

        protected override void InitPage()
        {
			
            InitializeComponent();
			/*BindingContext = _promoViewModel = new PromotionsViewModel();
			_promoViewModel.DataSave();*/

			//_promoViewModel.DataSave();

			LoadWidgets();
			//_promoViewModel.DataSave();
            base.InitPage();

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
			LoadingView.IsVisible = true;

			await InitializePage();
			await LoadData();
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			LoadingView.IsVisible = false;

			//_isInitialized = true;
	//		VouchersHeader.IsVisible = false;
//			VouchersRepeaterList.IsVisible = false;
  //          BindingContext = _voucherViewModel = new VouchersViewModel();
			//await LoadData();

		
         /*   if (!_isInitialized)
			{
				// Initialize page (aka token revalidation procedure)
			}
			else
			{
				// If initialized, reload only the necessary data
			}*/
            //await _voucherViewModel.LoadGiftsData();

            if (SessionData.IsAuthenticatedWithPharmacy)
            {

				//NoDataGrid.IsVisible = true;

				//if (!_vouchersAreInitialized)
				//{
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                
					LoadingView.IsVisible = false;

             
            }
            else
            {
              //  VouchersHeader.IsVisible = false;
               // VouchersRepeaterList.IsVisible = false;
                //NoDataLayout.IsVisible = false;
                _vouchersAreInitialized = true;
                LoadingView.IsVisible = false;
            }
           
            HighlightWidget.OnHeaderClicked += ShowHighlights;
            HighlightWidget.OnProductButtonClicked += OnLoadStarted;

            _viewModel.OnSuccess += OnUserProfileSuccessEvent;

            // XXX: the event attachment order matters!
            App.StoreBasketVM.OnLoadError += OnLoadError;
            App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;


        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LoadingView.IsVisible = false;

         //   _voucherViewModel.OnError -= OnLoadError;
        //    _voucherViewModel.OnSuccess -= OnLoadSuccess;

            HighlightWidget.OnHeaderClicked -= ShowHighlights;
            HighlightWidget.OnProductButtonClicked -= OnLoadStarted;

            _viewModel.OnSuccess -= OnUserProfileSuccessEvent;

            App.StoreBasketVM.OnLoadError -= OnLoadError;
            App.StoreBasketVM.OnLoadSuccess -= OnLoadSuccess;
            App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;

        }

        #endregion

        #region Loaders

        private async Task InitializePage()
        {
            // Load Profile - reloads the ANF token if needed
            if (SessionData.HasPharmacyCard) await _viewModel.LoadUserProfile();

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
                App.FavoritesVM.LoadData();
            }

            if (!_widgetIsInitialied)
            {
                LoadWidgets();
            }
        }

        private void LoadWidgets()
        {
            HighlightWidget.LoadData();
            _widgetIsInitialied = true;
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
            }
        }

        //testes
        public async void OnArticleTapped(object sender, EventArgs args)
        {

            if (sender == null || !(sender is View)) return;

            LoadingView.IsVisible = true;

            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		//	var v = sender as Voucher;
            Voucher v = (sender as View).BindingContext as Voucher;

            await Navigation.PushAsync(new VouchersDetailPage(v));

            LoadingView.IsVisible = false;
        }

        //testes
        async Task OnVouchersTap()
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        async Task OnLoadStarted()
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        async Task OnCartLoadStart()
        {
            LoadingView.IsVisible = true;
            //LoadingMessage.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;
        }

        void OnLoadError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
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


        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.PromotionsPageTitle;
        }

        #endregion
    }
}
