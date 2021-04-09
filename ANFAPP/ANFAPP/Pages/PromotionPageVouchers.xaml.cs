using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.Store;
using ANFAPP.Pages.UserArea.Vouchers;
using ANFAPP.Views;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out;

namespace ANFAPP.Pages
{
    public partial class PromotionPageVouchers : ANFPage
    {
        #region Properties

        public UserCardViewModel _viewModel = new UserCardViewModel();

     

        //testes
        public VouchersViewModel _voucherViewModel { get; set; }
		//public PromotionsViewModel _promoViewModel = new PromotionsViewModel();
        private bool _isInitialized = false;
        
		private VoucherOut.Condition vouchersDetailedOut;
        #endregion

        #region Page Initialization

        public PromotionPageVouchers() : base() { }

        protected override void InitPage()
        {
			
            InitializeComponent();
			/*BindingContext = _promoViewModel = new PromotionsViewModel();
			_promoViewModel.DataSave();*/

			//_promoViewModel.DataSave();

			//LoadWidgets();
			//_promoViewModel.DataSave();
            base.InitPage();

        }

        protected async override void OnAppearing()
		{
			base.OnAppearing();
			LoadingView.IsVisible = true;
			BindingContext = _voucherViewModel = new VouchersViewModel();
			//		await _voucherViewModel.LoadGiftsData();
			if (SessionData.IsAuthenticatedWithPharmacy)
			{


				//VouchersHeader.IsVisible = false;
				//	VouchersRepeaterList.IsVisible = false;
				//NoDataGrid.IsVisible = true;

				//if (!_vouchersAreInitialized)
				//{
				noVoucherLabel.IsVisible = false;


				//await _voucherViewModel.LoadGiftsData(false);
				await _voucherViewModel.LoadGiftsData(true);
				//await _voucherViewModel.LoadData();
				LoadingView.IsVisible = false;
				if (_voucherViewModel.VouchersList.Count != 0)
				{
					LoadingView.IsVisible = false;
					VouchersPromos.IsVisible = false;
					VouchersRepeaterList.IsVisible = true;
					noVoucherLabel.IsVisible = false;
				}
				else
				{
					LoadingView.IsVisible = false;
					VouchersPromos.IsVisible = false;
					VouchersRepeaterList.IsVisible = false;
					noVoucherLabel.IsVisible = true;
				}
			}
		//	HighlightWidget.OnHeaderClicked += ShowHighlights;
		//	HighlightWidget.OnProductButtonClicked += OnLoadStarted;
			//_viewModel.OnSuccess += OnUserProfileSuccessEvent;
			// XXX: the event attachment order matters!
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;
			if (!_isInitialized)
			{
				// Initialize page (aka token revalidation procedure)
				InitializePage();

			}
			else
			{
				// If initialized, reload only the necessary data
				//LoadData();
			}

		}
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LoadingView.IsVisible = false;

            _voucherViewModel.OnError -= OnLoadError;
            _voucherViewModel.OnSuccess -= OnLoadSuccess;

     //       HighlightWidget.OnHeaderClicked -= ShowHighlights;
      //      HighlightWidget.OnProductButtonClicked -= OnLoadStarted;

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
			System.Diagnostics.Debug.WriteLine("PromotionPageVoucher OnArticleTapped: VoucherType - " + v.Type);
			var voucherOut = await UserCardWS.GetUserVoucherDetails(SessionData.PharmacyUser.CardNumber);
			SessionData.VoucherWithPharmDetail = voucherOut.Vouchers;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);               //Safe delays 
																			//searches on the new request if the vouchers matches with the previous ones, if so, sends the voucher to the Page and seens if its avaliable on the selected farm
			foreach (var voucher in voucherOut.Vouchers)
			{
				if (v.Code == voucher.Code)  //if previous voucher code matches the new list voucher
				{
					vouchersDetailedOut = voucher.BurnCondition;            //gets his burn condition 
				}
			}
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);       //Safe delays 


			if (v.Type.Equals(Settings.VOUCHER_TYPE_VOUCHER))
			{
				await Navigation.PushAsync(new IndustryVoucherDetailPage(v));      //and sends it to the next page
			}
			else 
			{
				await Navigation.PushAsync(new VouchersDetailPage(v, vouchersDetailedOut));      //and sends it to the next page
			}


		//	await Navigation.PushAsync(new VouchersDetailPage(v));

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
