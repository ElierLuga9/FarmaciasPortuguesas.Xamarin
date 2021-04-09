using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutReservationPage : ANFPage
    {

		#region Properties

		CheckoutReservationViewModel _viewModel = new CheckoutReservationViewModel();

		#endregion

        #region Page Initialization

		public CheckoutReservationPage(CheckoutStartOut basket)
			: base() 
		{ 
			_viewModel.Basket = basket;
			BindingContext = _viewModel;
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

			LoadingView.IsVisible = false;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

		#endregion

		#region Event Handlers

		async void OnContinueButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			
			// Confirm the Checkout completing the order
			_viewModel.ConfirmCheckout();
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			NavigationUtils.PushPageAndClearHistory(new CheckoutFinalStepPage(_viewModel.Basket, null), Navigation);
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
