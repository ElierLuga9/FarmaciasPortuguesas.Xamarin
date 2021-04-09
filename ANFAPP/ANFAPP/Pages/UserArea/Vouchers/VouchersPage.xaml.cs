using System;
using System.Windows.Input;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Views;
using Xamarin.Forms;
using System.Threading.Tasks;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Pages.UserArea.Vouchers
{
    public partial class VouchersPage : ANFPage
    {

		#region Properties
		private Voucher _voucher;
        private bool _isInitialized = false;
        public VouchersViewModel _viewModel { get; set; }
        public ICommand VoucherItemCommand { get; set; }
		private VoucherOut.Condition vouchersDetailedOut;
        #endregion

        #region Page Initialization

        public VouchersPage () : base () { }

        protected override void InitPage ()
        {
            InitializeComponent ();
            base.InitPage ();
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing();

            if (!_isInitialized)
            {
                BindingContext = _viewModel = new VouchersViewModel();
            }

            _viewModel.OnError += OnLoadError;
            _viewModel.OnSuccess += OnLoadSuccess;

            if (!_isInitialized)
            {
                // Initialize context
                _viewModel.LoadData();

                _isInitialized = true;
			}
			else
			{
				LoadingView.IsVisible = false;
			}
        }

        protected override void OnDisappearing ()
        {
            base.OnDisappearing ();
			LoadingView.IsVisible = true;

            _viewModel.OnError -= OnLoadError;
            _viewModel.OnSuccess -= OnLoadSuccess;
        }

        #endregion




        #region Events

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args) {
			Voucher item = args.SelectedItem as Voucher;
			VoucherList.SelectedItem = null;
			//Calls the ANF WS to get voucher details data such as ExclusivePharmacy 
			var voucherOut = await UserCardWS.GetUserVoucherDetails(SessionData.PharmacyUser.CardNumber);
			SessionData.VoucherWithPharmDetail = voucherOut.Vouchers;

			if (item == null) return;
			if (item.Type.Equals(Settings.VOUCHER_TYPE_SPONSORED) || item.Type.Equals(Settings.VOUCHER_TYPE_PHARMACY))
			{

				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY); 				//Safe delays 
				//searches on the new request if the vouchers matches with the previous ones, if so, sends the voucher to the Page and seens if its avaliable on the selected farm
				foreach (var voucher in voucherOut.Vouchers)
				{
					if (item.Code == voucher.Code)  //if previous voucher code matches the new list voucher
					{	
						vouchersDetailedOut = voucher.BurnCondition;			//gets his burn condition 
					}
				}
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);		//Safe delays 
				await Navigation.PushAsync(new VouchersDetailPage(item, vouchersDetailedOut));		//and sends it to the next page


			}
			else {
				if ( item.Value == 2 ||item.Value == 5 ||
				item.Value == 10 || item.Value == 20)
				{
					LoadingView.IsVisible = true;
					await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
					await Navigation.PushAsync(new IndustryVoucherDetailPage(item));
				}
				else {
					return;
				}

			}

		}

        void OnLoadError (string title, string message)
        {
            // Hide Loading
            DisplayAlert (title, message, AppResources.OK);
            LoadingView.IsVisible = false;
        }

        void OnLoadSuccess ()
        {
            // Hide Loading
            LoadingView.IsVisible = false;
        }

        #endregion

        #region Button Events

		//void VoucherItemClick (object sender, EventArgs args)
		//{
		//	Navigation.PushAsync (new VouchersDetailPage (((Grid)sender).BindingContext as Voucher));
		//}

        async void OnVoucherHistory_Clicked (object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new VouchersHistoryPage ());
        }

        async void OnAquireVoucher_Clicked (object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new AquireVoucherListPage ());
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle ()
        {
            return AppResources.VouchersPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
