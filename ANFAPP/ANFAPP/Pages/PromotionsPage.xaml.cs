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
    public partial class PromotionsPage : ANFPage
    {
        #region Properties

        public UserCardViewModel _viewModel = new UserCardViewModel();
		private bool _initialized = false;



		//testes
		// public VouchersViewModel _voucherViewModel { get; set; }
		//public PromotionsViewModel _promoViewModel = new PromotionsViewModel();
		private bool _isInitialized = false;
      
        #endregion

        #region Page Initialization

        public PromotionsPage() : base() { }

        protected override void InitPage()
        {
			
            InitializeComponent();
            base.InitPage();

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
			LoadingView.IsVisible = true;

			if (!_initialized)
			{
				// Initialize page (aka token revalidation procedure)
				InitializePage();
			}
			else
			{
				LoadingView.IsVisible = true;

				// If initialized, reload only the necessary data
				LoadData();
			}



			//Task.Delay(Settings.DEFAULT_LOADING_DELAY);




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
				//Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                
					

             
            }
            else
            {
              //  VouchersHeader.IsVisible = false;
               // VouchersRepeaterList.IsVisible = false;
                //NoDataLayout.IsVisible = false;
                //_vouchersAreInitialized = true;
                LoadingView.IsVisible = false;
            }
           
           // HighlightWidget.OnHeaderClicked += ShowHighlights;
           // HighlightWidget.OnProductButtonClicked += OnLoadStarted;

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

         //   HighlightWidget.OnHeaderClicked -= ShowHighlights;
         //   HighlightWidget.OnProductButtonClicked -= OnLoadStarted;

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
			if (SessionData.HasPharmacyCard) { await _viewModel.LoadUserProfile(); }
			await LoadData();

			_initialized = true;
			LoadingView.IsVisible = false;
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

            /*if (!_widgetIsInitialied)
            {
                LoadWidgets();
            }*/
			LoadingView.IsVisible = false;
        }

        private void LoadWidgets()
        {
    //        HighlightWidget.LoadData();
           // _widgetIsInitialied = true;
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

        async void OnPromotionSelect(object sender, EventArgs args)
        {
            var p = sender as PromotionsOut;
            /*if (sender == null || !(sender is View)) return;
            var context = (sender as View).BindingContext as PromotionsOut;


            if (BindingContext == null || !(BindingContext is PromotionsOut)) return;
            var p = BindingContext as PromotionsOut;*/
            if (p.ButtonLabel == "Ver Produto")
				
            {
                int cnp = Int32.Parse(p.PromoTypePar);
                await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                await Navigation.PushAsync(new StoreProductDetailPage(cnp));
            }
            if (p.ButtonLabel == " Ver Produtos")
            {
                LoadingView.IsVisible = true;
                var cnp = p.CNPList;
                await Navigation.PushAsync(new PromotionHighlightsPage(p.CNPList));
                /*var page = new PromotionsListPage(cnp);
                await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                await Navigation.PushAsync(page);*/
                LoadingView.IsVisible = false;
            }

            if (p.ButtonLabel == "Obter Vale")
            {
                LoadingView.IsVisible = true;

                try
                {
					

					if (SessionData.IsLogged == false)
					{
						LoadingView.IsVisible = false;
						await DisplayAlert(null, AppResources.LoginVoucherPermission, AppResources.OK);

					}
					else if (SessionData.PharmacyUser.CardNumber == null)
					{
						LoadingView.IsVisible = false;
						await DisplayAlert(null, AppResources.CardVoucherPemission, AppResources.OK);
					}
					else {
						var wallet = await UserCardWS.GetUserVoucher(SessionData.PharmacyUser.CardNumber, p.PromoTypePar);
						var getWallet = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);
						var voucherOut = getWallet.Vouchers[getWallet.Vouchers.Count - 1];
						Voucher voucher = new Voucher(voucherOut);
						await Navigation.PushAsync(new VouchersDetailPage(voucher,voucherOut.BurnCondition,true));
					}
                }
                catch
                {
                    LoadingView.IsVisible = false;
                    return;
                }
            }

            if (p.ButtonLabel == "Saber Mais")
            {
                LoadingView.IsVisible = true;
                await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

                await Navigation.PushAsync(new PromotionsDescriptionPage(p));
            }

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
