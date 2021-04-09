using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutFinalStepPage : ANFPage
    {
		#region Properties

		private CheckoutFinalStepViewModel _viewModel;

		#endregion

        #region Page Initialization

		public CheckoutFinalStepPage(CheckoutStartOut basket, CheckoutFinalStepViewModel viewModel) : base() 
		{ 
			_viewModel = viewModel;
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
			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			// Update (clear) the basket after the payment is complete.
			App.StoreBasketVM.LoadBasket ();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

		#endregion

		#region Event Handlers

		async void OnContinueButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);

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
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE;
        }

        protected override string GetAppBarTitle()
        {
			return AppResources.CheckoutPageTitle;
        }

        #endregion

    }
}
