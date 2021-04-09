using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Threading.Tasks;

namespace ANFAPP.Pages.Store.Checkout
{ 
	public partial class CheckoutPaymentMBPage : ANFPage
	{

		private CheckoutStartOut _basket;
		private CheckoutConfOut _checkoutResult;
		private CheckoutFinalStepViewModel _finalStepViewModel;

		#region Page Initialization

		public CheckoutPaymentMBPage(CheckoutConfOut checkoutResult) : base() 
		{
			_checkoutResult = checkoutResult;
		}

		public CheckoutPaymentMBPage(CheckoutConfOut checkoutResult, CheckoutStartOut basket, CheckoutFinalStepViewModel finalStepViewModel)
			: this(checkoutResult)
		{
			_basket = basket;
			_finalStepViewModel = finalStepViewModel;

			BindingContext = _checkoutResult;
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

			LoadingView.IsVisible = false;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}

		async void OnContinueButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await NavigationUtils.PushPageAndClearHistory(new CheckoutFinalStepPage(_basket, _finalStepViewModel), Navigation);
		}

		#endregion

		#region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE;
        }

		protected override string GetAppBarTitle()
		{
			return AppResources.CheckoutPageTitle;
		}

		#endregion

	}
}
