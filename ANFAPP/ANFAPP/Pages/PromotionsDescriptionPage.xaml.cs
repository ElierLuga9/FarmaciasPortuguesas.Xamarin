using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Pages.UserArea.Vouchers;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.Store;
using ANFAPP.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Pages
{
    public partial class PromotionsDescriptionPage : ANFPage
    {
        private PromotionsOut _promo;
        private PromotionsTypeListItemViewModel _viewModel;

        public PromotionsDescriptionPage():base() {}

        public PromotionsDescriptionPage(PromotionsOut promotion) : base() 
        {
            _promo = promotion;
			if (_promo.FullPathImage == null)
			{
				PromoImage.IsVisible = false;
			}
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #region Page Lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();


            // Initialize binding context

            BindingContext = _viewModel = new PromotionsTypeListItemViewModel();
            
            LoadData();

            //_viewModel.OnError += OnError_EventHandler;
            _viewModel.OnSuccess += OnLoadSuccess;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // _viewModel.OnError -= OnError_EventHandler;
            _viewModel.OnSuccess -= OnLoadSuccess;
            LoadingView.IsVisible = false;
        }


        void OnLoad()
        {
            LoadingView.IsVisible = true;
        }

        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;
        }


        private async void OnButtonClicked(object sender, EventArgs args)
        {
            if (_promo.PromoType == 1)
            {
                int cnp = Int32.Parse(_promo.PromoTypePar);
                await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                await Navigation.PushAsync(new StoreProductDetailPage(cnp));

            }
           
            if (_promo.PromoType == 2)
            {

                LoadingView.IsVisible = true;

                try
                {

                    if (SessionData.IsLogged == false)
                    {
                        LoadingView.IsVisible = false;
                        DisplayAlert(null, AppResources.LoginVoucherPermission, AppResources.OK);

                    }
                    else if (SessionData.PharmacyUser.CardNumber == null)
                    {
                        LoadingView.IsVisible = false;
                        DisplayAlert(null, AppResources.CardVoucherPermission, AppResources.OK);
                    }
                    else
                    {
                        var wallet = await UserCardWS.GetUserVoucher(SessionData.PharmacyUser.CardNumber, _promo.PromoTypePar);
                        var getWallet = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);
                        var voucherOut = getWallet.Vouchers[getWallet.Vouchers.Count - 1];
                        Voucher voucher = new Voucher(voucherOut);
                        await Navigation.PushAsync(new VouchersDetailPage(voucher, voucherOut.BurnCondition, true));
                    }
                }
                catch
                {
                    LoadingView.IsVisible = false;
                    return;
                }

            }

            if (_promo.PromoType == 3)
            {
                LoadingView.IsVisible = true;
                var cnp = _promo.CNPList;
                await Navigation.PushAsync(new PromotionHighlightsPage(cnp));
                /*var page = new PromotionsListPage(cnp);
                await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                await Navigation.PushAsync(page);*/
                LoadingView.IsVisible = false;
            }

        }

        #endregion
        

        void LoadData()
        {
             _viewModel.LoadData(_promo);

             if (_promo.PromoType == 0)
             {
                 PromoTypeButton.IsVisible = false;
             }

             if (_promo.PromoType == 1)
             {
                 PromoTypeButton.Text = "Ver Produto";
             }

             if (_promo.PromoType == 2)
             {
                 PromoTypeButton.Text = "Obter Vale";
             }
             if (_promo.PromoType == 3)
             {
                 PromoTypeButton.Text = "Ver Produtos";
             }
          
        }

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
