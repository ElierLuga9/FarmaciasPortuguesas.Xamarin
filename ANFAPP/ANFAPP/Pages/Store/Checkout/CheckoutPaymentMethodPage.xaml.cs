using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using ANFAPP.Logic.Database.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using ANFAPP.Utils;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutPaymentMethodPage : ANFPage
    {

		private static readonly decimal ATM_MIN_VALUE = 2;


		private bool _initialized = false;
		private VouchersViewModel _vouchersViewModel = new VouchersViewModel ();
		protected CheckoutPaymentViewModel _viewModel = new CheckoutPaymentViewModel();

        #region Page Initialization

		public CheckoutPaymentMethodPage(CheckoutStartOut basket)
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

		protected virtual void SetupPaymentMethods()
		{
			// If the total = 0€, only show the order button
			if (_viewModel.Basket != null && _viewModel.Basket.TotalInvoice + _viewModel.Basket.MSRMMaxValue <= 0)
			{
				OrderButton.IsVisible = true;
				MB.IsVisible = false;
				MBWAY.IsVisible = false;
				HiPay.IsVisible = false;
				Delivery.IsVisible = false;
			}
			else
			{
				OrderButton.IsVisible = false;

				// If the total > 0€, show the available payment methods
				foreach (var obj in _viewModel.Basket.PaymentTypes)
				{
					if (string.Equals(obj.Id, "mb", StringComparison.OrdinalIgnoreCase))
					{
						MB.IsVisible = obj.IsActive;
						MBWAY.IsVisible = obj.IsActive; //MBWAY Payment Method
					}
					else if (string.Equals(obj.Id, "hipay", StringComparison.OrdinalIgnoreCase))
					{
						HiPay.IsVisible = obj.IsActive;
						VisaLabel.Text = obj.AlternativeDescription;
					}
					else if (string.Equals(obj.Id, "cashondelivery", StringComparison.OrdinalIgnoreCase))
					{
						Delivery.IsVisible = obj.IsActive;
						DeliveryLabel.Text = obj.AlternativeDescription;
					}
				}
			}
		}	

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			SetupPaymentMethods ();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnUpdateVoucherSuccess += OnUpdateVoucherSuccess;
			_viewModel.OnDeleteVoucherSuccess += OnDeleteVoucherSuccess;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			_vouchersViewModel.OnSuccess += OnVoucherLoadSuccess;
			_vouchersViewModel.OnError += OnLoadError;

			// Update the vouchers and totals when we enter the page.
			await _viewModel.UpdateCheckout();

			if (!_initialized) {
				if (SessionData.HasPharmacyCard) {
					// We have a list of vouchers from CheckoutStart but the list elements are id,value tuples.
					// Therefore, we need to match the id with the voucher code from ANF to get a description 
					// fit to be consumed by human beings.
					//
					// A user without card doesn't have any vouchers.
					LoadingView.IsVisible = true;
					await _vouchersViewModel.LoadData ();
				}

				_initialized = true;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnUpdateVoucherSuccess -= OnUpdateVoucherSuccess;
			_viewModel.OnDeleteVoucherSuccess -= OnDeleteVoucherSuccess;

			_vouchersViewModel.OnSuccess -= OnVoucherLoadSuccess;
			_vouchersViewModel.OnError -= OnLoadError;
		}

		#endregion

		#region Event Handlers

		protected async void OnAddVoucherButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new AddVoucherPage(_viewModel));
		}

		protected async Task FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod paymentMethod)
		{
			if (_viewModel.Basket.HasReservationTax &&
				(paymentMethod == CheckoutPaymentViewModel.PaymentMethod.ON_DELIVERY || paymentMethod == CheckoutPaymentViewModel.PaymentMethod.FREE_ORDER))
			{
				// Reservation tax
				await Navigation.PushAsync(new CheckoutPayReservationRatePage(_viewModel.Basket));
				//LaunchPagePostCheckout(new CheckoutPayReservationRatePage(_viewModel.Basket));
				return;
			}

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
			vm.VouchersInCart = _viewModel.VouchersInCart;
			if((paymentMethod == CheckoutPaymentViewModel.PaymentMethod.MBWAY))
			{
				await NavigationUtils.PushPageKeepHistory(new CheckoutPhoneConfirmationPage(_viewModel.Basket,vm), Navigation);
			}
			else
			{
				vm.OnLoadError += OnLoadError;
				var result = await vm.CheckoutConf();
				vm.OnLoadError -= OnLoadError;
				if (result != null && result.Success)
				{

					var mixpanelWidget = DependencyService.Get<IMixPanel>();
					var props = new Dictionary<string, string>();
					//				props.Add("UsedVouchers", (vm.VouchersInCart != null && vm.VouchersInCart.Count > 0) + string.Empty);

					if (paymentMethod == CheckoutPaymentViewModel.PaymentMethod.ATM)
					{
						props.Add("PaymentType", "MB");
					}
					else if (paymentMethod == CheckoutPaymentViewModel.PaymentMethod.HIPAY)
					{
						props.Add("PaymentType", "HIPAY");
					}
					else if (paymentMethod == CheckoutPaymentViewModel.PaymentMethod.ON_DELIVERY)
					{
						props.Add("PaymentType", "On Delivery");
					}
					props.Add("PharmacyID", SessionData.StorePharmacyId);

					//				string cnps = string.Empty;
					//				string cnpems = string.Empty;
					//				string quantities = string.Empty;
					//				string prices = string.Empty;
					//
					//				if (_viewModel.Basket.Products != null) {
					//					foreach (var product in _viewModel.Basket.Products) {
					//						cnps += product.CNP + "_";
					//						cnpems += product.CNPEM + "_";
					//						quantities += product.Quantity + "_";
					//						prices += product.AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY ? product.Price + "€_" : product.Points + " points_";
					//					}
					//
					//					// Removes the last underscore
					//					cnps = cnps.Length > 0 ? cnps.Substring(0, cnps.Length - 1) : cnps;
					//					cnpems = cnpems.Length > 0 ? cnpems.Substring(0, cnpems.Length - 1) : cnpems;
					//					quantities = quantities.Length > 0 ? quantities.Substring(0, quantities.Length - 1) : quantities;
					//					prices = prices.Length > 0 ? prices.Substring(0, prices.Length - 1) : prices;
					//				}

					//				props.Add("CNP", cnps);
					//				props.Add("CNPEM", cnpems);
					//				props.Add("Quantity", quantities);
					//				props.Add("Price", prices);

					props.Add("TotalValue", (_viewModel.Basket.TotalValue + "€").Replace(",", "."));
					props.Add("TotalPoints", _viewModel.Basket.TotalUsedPoints + " Points");

					if (_viewModel.Basket.TotalVouchers.HasValue && _viewModel.Basket.TotalVouchers.Value > 0)
					{
						props.Add("UsedVouchers", (_viewModel.Basket.TotalVouchers + "€").Replace(",", "."));
					}

					props.Add("Quantity", _viewModel.Basket.TotalQuantityInCart + "");
					props.Add("OrderID", result.OrderId);
					props.Add("Email", SessionData.PharmacyUser.Username);
					props.Add("Name", SessionData.PharmacyUser.Name);
					props.Add("CardNumber", SessionData.PharmacyUser.CardNumber);

					mixpanelWidget.TrackProperties("CheckoutFinalized", props);

					vm.OrderId = result.OrderId;
					switch (paymentMethod)
					{
						case CheckoutPaymentViewModel.PaymentMethod.FREE_ORDER:
						case CheckoutPaymentViewModel.PaymentMethod.ON_DELIVERY:
							LaunchPagePostCheckout(new CheckoutFinalStepPage(_viewModel.Basket, vm));
							break;
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
				case CheckoutPaymentViewModel.PaymentMethod.MBWAY:
					message = AppResources.CheckoutPaymentMBWAYConfirmMessage;
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
		protected async void OnMBWAYButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await FinalizeCheckout(CheckoutPaymentViewModel.PaymentMethod.MBWAY);
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

		protected async void OnRemoveVoucherButtonClicked(object sender, EventArgs args)
		{
			Button button = sender as Button;
			if (button.BindingContext is Voucher) {
				var v = button.BindingContext as Voucher; 
		
				await _viewModel.RemoveVoucherInCart (v);

				// Update Payment Methods
				SetupPaymentMethods();
			}
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

		protected async virtual Task OnVoucherSuccess()
		{
			LoadingView.IsVisible = false;

			var selected = new ObservableCollection<Voucher> ();

			if (_viewModel.Basket.Vouchers != null && _vouchersViewModel != null && _vouchersViewModel.Vouchers != null) {

				var allVouchers = new List<Voucher> ();
				foreach (VouchersViewModel.VoucherGroup g in _vouchersViewModel.Vouchers) {
					allVouchers.AddRange (g);
				}

				foreach (VoucherOut vo in _viewModel.Basket.Vouchers) {
					bool added = false;		
					foreach (Voucher v in allVouchers) {
						if (string.Equals (vo.Id, v.Code)) {
							selected.Add (v);
							added = true;
							break;
						}
					}
					// If the voucher is in the cart we need to add it anyway...
					if (!added) {
						selected.Add (new Voucher { Code = vo.Id, Value = (double)vo.Value });
					}
				}
			}
			_viewModel.VouchersInCart = selected;

			// Update payment methods
			SetupPaymentMethods();
		}

		protected async void OnVoucherLoadSuccess()
		{
			LoadingView.IsVisible = false;

			await OnVoucherSuccess ();
		}

		protected async void OnUpdateVoucherSuccess()
		{
			await OnVoucherSuccess ();
		}

		protected async void OnDeleteVoucherSuccess()
		{
			LoadingView.IsVisible = false;
		}

		protected void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);

			if (title != null && title.Equals("Conf"))
			{
				App.StoreBasketVM.PeekBasket();
				NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);
			}
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
