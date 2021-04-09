using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.Store.Checkout
{
	public partial class CheckoutPaymentWebViewPage : ANFPage
	{
		private string _paymentURL;
		private CheckoutStartOut _basket;
		private CheckoutFinalStepViewModel _finalStepViewModel;

		#region Page Initialization

		public CheckoutPaymentWebViewPage(string paymentURL) : base() 
		{ 
			_paymentURL = paymentURL;
		}

		public CheckoutPaymentWebViewPage(string paymentURL, CheckoutStartOut basket, CheckoutFinalStepViewModel finalStepViewModel) : this(paymentURL)
		{
			_basket = basket;
			_finalStepViewModel = finalStepViewModel;
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}
			
		#endregion

		#region Event Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//PaymentWebView.OnShouldLoadUrl += OnPageLoadUrl;
			PaymentWebView.Source = _paymentURL;
			PaymentWebView.Navigating += v_Navigating;

			//_viewModel.OnLoadError += OnLoadError;
			//_viewModel.OnLoadSuccess += OnLoadSuccess;

			LoadingView.IsVisible = false;
		}

		void v_Navigating(object sender, WebNavigatingEventArgs e)
		{
			OnPageLoadUrl(e.Url);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			//PaymentWebView.OnShouldLoadUrl -= OnPageLoadUrl;

			//_viewModel.OnLoadError -= OnLoadError;
			//_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

		bool OnPageLoadUrl(string url)
		{
			System.Diagnostics.Debug.WriteLine (url);

			// Return to the Checkout if the payment was cancelled.
			if (url != null && url.Contains ("/hipay/mapi/cancel")) {
				BackToStore();
				return false;	
			} else if (url != null && url.Contains ("/hipay/mapi/success")) {
				FinishCheckout();
				return false;
			} 

			return true;
		}

		private async void BackToStore()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);
		}

		private async void FinishCheckout()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await NavigationUtils.PushPageAndClearHistory(new CheckoutFinalStepPage(_basket, _finalStepViewModel), Navigation);
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

		protected override string GetAppBarTitle()
		{
			return AppResources.CheckoutPageTitle;
		}

		#endregion
	}
}
