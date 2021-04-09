using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.Vouchers
{
	public partial class VoucherAquisitionPage : ANFPage
	{

		#region Properties


		public VoucherAquisitionViewModel _viewModel { get; set; }
		private string _voucherV;
		private VoucherPointsMatrixOut.VoucherPointsMatrix _voucher { get; set; }
		private bool _isInitialized = false;

		#endregion

		#region Page Initialization

		public VoucherAquisitionPage() : base() { }

		public VoucherAquisitionPage(string voucher) : base()
		{

			_voucherV = voucher;
			voucherImage();

		}

		protected override void InitPage()
		{
			

			InitializeComponent();
			base.InitPage();
		}

		#endregion

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (!_isInitialized)
			{
				// Initialize context
				this.BindingContext = _viewModel = new VoucherAquisitionViewModel();
				_viewModel.LoadPage(_voucherV);
				_viewModel.Voucher = _voucher;
			}

			_viewModel.OnError += OnError_EventHandler;
			_viewModel.OnSuccess += OnSuccess_EventHandler;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnError -= OnError_EventHandler;
			_viewModel.OnSuccess -= OnSuccess_EventHandler;
		}

		#region Event Handlers

		void OnRequestVoucher_Clicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			_viewModel.RequestVoucherAquisition();
		}

		void OnError_EventHandler(string title, string message)
		{
			LoadingView.IsVisible = false;

			// Show error message
			DisplayAlert(title, message, AppResources.OK);
		}

		async void OnSuccess_EventHandler()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("Value", _viewModel._voucherValue + "€");
			mixpanelWidget.TrackProperties("VoucherAcquired", props);

			LoadingView.IsVisible = false;

			// Show success
			await DisplayAlert(null, AppResources.VoucherAquisitionSuccessMessage, AppResources.OK);

			// Go back to the vouchers page
			NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);
		}

		#endregion
		public void voucherImage()
		{
			if (_voucherV == "Vale2Euros")
			{
				vale_sauda.Source = "cartao_vale_sauda_2";
			}
			else if (_voucherV == "Vale5Euros")
			{
				vale_sauda.Source = "cartao_vale_sauda_5";
			}
			else if (_voucherV == "Vale10Euros")
			{
				vale_sauda.Source = "cartao_vale_sauda_10";

			}
			else if (_voucherV == "Vale20Euros")
			{
				vale_sauda.Source = "cartao_vale_sauda_20";

			}

		}

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.VoucherAquisitionPageTitle;
		}


		//testes
		protected override bool HasCardButton()
		{
			return true;
		}

		#endregion

	}
}