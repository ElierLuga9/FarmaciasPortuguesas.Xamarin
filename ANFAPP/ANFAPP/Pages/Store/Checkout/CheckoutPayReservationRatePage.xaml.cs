using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic;
using ANFAPP.Utils;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.Store.Checkout
{
	public partial class CheckoutPayReservationRatePage : ANFPage
	{

		#region Properties

		private bool _initialized = false;
		protected CheckoutPaymentViewModel _viewModel = new CheckoutPaymentViewModel();

		#endregion

		#region Page Initialization

		public CheckoutPayReservationRatePage(CheckoutStartOut basket)
		{
			_viewModel.Basket = basket;
		}

		protected override void InitPage()
		{
			InitComponent();
			base.InitPage();

			BindingContext = _viewModel;
		}

		protected virtual void InitComponent()
		{
			InitializeComponent();
		}


		#endregion

		#region Lifecycle Events

		protected void SetupPaymentMethods()
		{
			foreach (var obj in _viewModel.Basket.ReservationParams.PaymentMethods) {
				if (string.Equals (obj.Id, "mb", StringComparison.OrdinalIgnoreCase)) {
					MB.IsVisible = obj.IsActive;
				} else if (string.Equals (obj.Id, "hipay", StringComparison.OrdinalIgnoreCase)) {
					HiPay.IsVisible = obj.IsActive;
				}
			}
		}	

		#endregion

		#region Event Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();

			SetupPaymentMethods ();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

			/*
			_vouchersViewModel.OnSuccess -= OnVoucherLoadSuccess;
			_vouchersViewModel.OnError -= OnLoadError;
			*/
		}

		#endregion

		#region Event Handlers

		protected async Task FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod paymentMethod)
		{
			// Show confirmation dialog to the user
			if (!await ConfirmPaymentChoice(paymentMethod)) 
			{
				LoadingView.IsVisible = false;
				return;
			}

			// Set the payment method before finalizing the checkout.
			if (!await _viewModel.SetPaymentMode (paymentMethod)) return;

			// Finish the Checkout
			CheckoutFinalStepViewModel vm = new CheckoutFinalStepViewModel ();

			vm.OnLoadError += OnLoadError;
			var result = await vm.CheckoutConf ();
			vm.OnLoadError -= OnLoadError;

			if (result != null && result.Success) {
				vm.OrderId = result.OrderId;

				switch (paymentMethod) {
					case CheckoutPaymentViewModel.PaymentMethod.HIPAY:
						LaunchPagePostCheckout(new CheckoutPaymentWebViewPage(result.UrlHiPay, _viewModel.Basket, vm));
						break;
					case CheckoutPaymentViewModel.PaymentMethod.ATM:
						LaunchPagePostCheckout(new CheckoutPaymentMBPage(result, _viewModel.Basket, vm));
						break;
					default:
						break;
				}
			}
		}

		protected async Task<bool> ConfirmPaymentChoice(CheckoutPaymentViewModel.PaymentMethod paymentMethod)
		{
			string message = string.Empty;
			switch (paymentMethod)
			{
			case CheckoutPaymentViewModel.PaymentMethod.FREE_ORDER:
				message = AppResources.CheckoutPaymentFreeOrderConfirmMessage;
				break;
			case CheckoutPaymentViewModel.PaymentMethod.ON_DELIVERY:
				message = AppResources.CheckoutPaymentDeliveryConfirmMessage;
				break;
			case CheckoutPaymentViewModel.PaymentMethod.HIPAY:
				message = AppResources.CheckoutPaymentVISAConfirmMessage;
				break;
			case CheckoutPaymentViewModel.PaymentMethod.ATM:
				message = AppResources.CheckoutPaymentMBConfirmMessage;
				break;
			default: break;
			}

			if (string.IsNullOrEmpty(message)) return false;
			return await DisplayAlert(null, message, AppResources.CheckoutPaymentConfirmButton, AppResources.CheckoutPaymentCancelButton);
		}

		protected async void OnVISAButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod.HIPAY);
		}

		protected async void OnATMButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod.ATM);
		}

		protected async void OnDeliveryButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod.ON_DELIVERY);
		}

		protected async void OnOrderButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod.FREE_ORDER);
		}

		protected async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		protected void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;

			// Validate and show any existing basket error
			if (_viewModel.Basket.HasError && !string.IsNullOrEmpty(_viewModel.Basket.ErrorMessage))
			{
				DisplayAlert(null, _viewModel.Basket.ErrorMessage, AppResources.OK);
			}
		}

		protected void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		protected async void LaunchPagePostCheckout(Page page)
		{
			await NavigationUtils.PushPageAndClearHistory(page, Navigation);


			//Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);
			//await NavigationUtils.PopToPage(page, Navigation);

			//await Task.Delay(400);
			//Navigation.InsertPageBefore(new StoreLandingPage(), page);




			//var storeLanding = new StoreLandingPage();
			//Navigation.InsertPageBefore(storeLanding, Navigation.NavigationStack[0]);
			//await NavigationUtils.PopToPage(storeLanding, Navigation);
			//await Navigation.PushAsync(page, false);
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