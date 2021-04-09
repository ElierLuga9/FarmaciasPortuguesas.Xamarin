using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class AddVoucherPage : ANFPage
    {

		#region Properties

		private CheckoutVouchersViewModel _viewModel;
		private CheckoutPaymentViewModel _paymentsVM;

		#endregion

        #region Page Initialization

		public AddVoucherPage(CheckoutPaymentViewModel paymentsVM) : base() 
		{ 
			_paymentsVM = paymentsVM;
			BindingContext = _viewModel = new CheckoutVouchersViewModel(_paymentsVM.Basket);
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			LoadingView.IsVisible = true;

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnError += OnLoadError;
			_viewModel.OnSuccess += OnLoadSuccess;

			// The wallet was refreshed on the previous page so we will not fetch the vouchers again. 
			// The vouchers will be loaded from the local database.
			_viewModel.LoadData(_paymentsVM.VouchersInCart, false);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnError -= OnLoadError;
			_viewModel.OnSuccess -= OnLoadSuccess;
		}

		#endregion

		#region Event Handlers

		void OnVoucherSelected(object sender, ItemTappedEventArgs args) 
		{
			var voucher = args.Item as Voucher;
			if (voucher == null) return;

			VoucherList.SelectedItem = null;
			_viewModel.ToggleVoucherSelection(voucher);
		} 

		void OnConfirmButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			
			if (_paymentsVM != null) _paymentsVM.VouchersInCart = _viewModel.GetSelectedVouchers();

			Navigation.PopAsync();
		}

		void OnCancelButtonClicked(object sender, EventArgs args)
		{
			Navigation.PopAsync();
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
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

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
			return AppResources.CheckoutPageTitle;
        }

        #endregion

    }
}
