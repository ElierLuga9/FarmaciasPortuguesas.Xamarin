using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;
using System.Collections.Generic;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutConfirmationPage : ANFPage
    {

		#region Properties

		private CheckoutConfirmationViewModel _viewModel = new CheckoutConfirmationViewModel();
		private bool _initialized = false;

		#endregion

        #region Page Initialization

		public CheckoutConfirmationPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _viewModel;
        }

        #endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			_viewModel.OnValidationError += OnValidationError;
			_viewModel.OnValidationSuccess += OnValidationSuccess;

			// Reload Checkout Data
			_viewModel.StartCheckout();

			
			//if (!_initialized) 
			//{
			//	_viewModel.StartCheckout();	
			//	_initialized = true;
			//}
			//else
			//{
			//	_viewModel.RefreshBasket();
			//}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

			_viewModel.OnValidationError -= OnValidationError;
			_viewModel.OnValidationSuccess -= OnValidationSuccess;

			LoadingView.IsVisible = false;
		}

		#endregion

		#region Event Handlers

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			ProductOut item = args.SelectedItem as ProductOut;

			if (item != null && item.CNP != null) {
				LoadingView.IsVisible = true;
				await Task.Delay (Settings.DEFAULT_LOADING_DELAY);

				await Navigation.PushAsync(new StoreProductDetailPage(item.CNP.GetValueOrDefault()));
			}
		}

		async void OnPaymentModeSwitchClicked(object sender, EventArgs args)
		{
			if (sender == null) return;
			var view = sender as View;

			if (view.BindingContext == null) return;
			var context = view.BindingContext as BasketProductOut;
			if (!context.CanToggleAquisition) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.SwitchPaymentMode(context);
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;

			// Validate and show any existing basket error
			if (_viewModel.Basket.HasError && !string.IsNullOrEmpty(_viewModel.Basket.ErrorMessage))
			{
				DisplayAlert(null, _viewModel.Basket.ErrorMessage, AppResources.OK);
			}

		}

		async void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			await DisplayAlert(title, message, AppResources.OK);

			await Navigation.PopAsync();
		}

		async void OnConfirmButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.ValidatePoints();
		}

		void OnValidationSuccess()
		{
			_viewModel.TrackMixPanelCheckoutConfirmation();
			Navigation.PushAsync(new CheckoutDeliveryMethodPage(_viewModel.Basket));

			// TODO: DEV ONLY BYPASS - REMOVE ONCE COMPLETE
			//Navigation.PushAsync(new CheckoutFinalStepPage(null));
		}

		async void OnValidationError(string title, string message)
		{
			LoadingView.IsVisible = false;
			await DisplayAlert(title, message, AppResources.OK);
		}

		#endregion

		#region Search Events

		protected async void OnSearch(string searchValue)
		{
			// Don't search if the query is null
			if (string.IsNullOrEmpty(searchValue)) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new StoreSearchPage(searchValue));
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
