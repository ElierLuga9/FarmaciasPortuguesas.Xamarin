using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Pages.Store.Checkout;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using System.Collections.Generic;

namespace ANFAPP.Pages.Store
{
	public partial class StoreBasketPage : ANFStorePage
    {
		// The Entry connected to the keyboard.
		private Entry _editingTf;
		private CheckoutConfirmationViewModel _checkoutViewModel = new CheckoutConfirmationViewModel();

        #region Page Initialization

		public StoreBasketPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = App.StoreBasketVM;
        }

        #endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			App.StoreBasketVM.OnLoadStart += OnLoadStart;
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;

			_checkoutViewModel.OnLoadStart += OnLoadStart;
			_checkoutViewModel.OnLoadError += OnLoadError;
			_checkoutViewModel.OnValidationError += OnLoadError;
			_checkoutViewModel.OnLoadSuccess += OnCheckoutStartSuccess;
			_checkoutViewModel.OnValidationSuccess += OnValidatePointsSuccess;

			// Reload Basket
			App.StoreBasketVM.LoadBasket();

			//if (_viewModel.InvalidSearchResults)
			//{
			//	// Perform search if the filter/query parameters have changed
			//	_viewModel.PerformSearch();
			//}
			//else
			//{
			//	LoadingView.IsVisible = false;
			//}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.StoreBasketVM.OnLoadStart -= OnLoadStart;
			App.StoreBasketVM.OnLoadError -= OnLoadError;
			App.StoreBasketVM.OnLoadSuccess -= OnLoadSuccess;

			_checkoutViewModel.OnLoadStart -= OnLoadStart;
			_checkoutViewModel.OnLoadError -= OnLoadError;
			_checkoutViewModel.OnValidationError -= OnLoadError;
			_checkoutViewModel.OnLoadSuccess -= OnCheckoutStartSuccess;
			_checkoutViewModel.OnValidationSuccess -= OnValidatePointsSuccess;
		}

		#endregion

		#region Event Handlers

		async void OnItemClicked(object sender, EventArgs args)
		{
			var view = sender as View;
			if (view == null || view.BindingContext == null) return;

			ProductOut item = view.BindingContext as ProductOut;
			if (item == null || !item.CNP.HasValue || item.CNP == 0) return;

			if (item != null && item.CNP != null) {
				LoadingView.IsVisible = true;

				await Task.Delay (Settings.DEFAULT_LOADING_DELAY);

				await Navigation.PushAsync(new StoreProductDetailPage(item.CNP.GetValueOrDefault()));
			}
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
			if (App.StoreBasketVM.Basket.HasError && !string.IsNullOrEmpty(App.StoreBasketVM.Basket.ErrorMessage))
			{
				DisplayAlert(null, App.StoreBasketVM.Basket.ErrorMessage, AppResources.OK);
			}

			// Dismiss the keyboard proxy if open
			DismissKeyboardProxy(null, null);
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		void OnCheckoutStartSuccess() 
		{
			// Validate and show any existing basket error
			if (_checkoutViewModel.Basket.HasError && !string.IsNullOrEmpty(_checkoutViewModel.Basket.ErrorMessage))
			{
				LoadingView.IsVisible = false;
				DisplayAlert(null, _checkoutViewModel.Basket.ErrorMessage, AppResources.OK);
			}

			_checkoutViewModel.ValidatePoints();
		}

		void OnValidatePointsSuccess() 
		{
			_checkoutViewModel.TrackMixPanelCheckoutConfirmation();
			Navigation.PushAsync(new CheckoutDeliveryMethodPage(_checkoutViewModel.Basket));

		}

		async void OnCheckoutClicked(object sender, EventArgs args)
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var properties = new Dictionary<string, string>();
			properties.Add("PharmacyID", SessionData.StorePharmacyId);

			mixpanelWidget.TrackProperties("CheckoutStarted", properties);

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			if (App.StoreBasketVM.CanToggleProductAquisitionState())
			{
				// If we can change any aquisition, go to checkout start
				await Navigation.PushAsync(new CheckoutConfirmationPage());
			}
			else
			{
				// If we cannot, start the checkout right away
				_checkoutViewModel.StartCheckout();
			}
		}
		async void OnClearCartVouchersClicked(object sender, EventArgs args)
		{
			var result = await DisplayAlert(null, AppResources.StoreBasketEmptyCartConfirmationMessage, AppResources.Yes, AppResources.No);
			if (result) { await App.StoreBasketVM.ClearBasketWithOnlyVouchers();}
		}
		void OnContinueShoppingClicked(object sender, EventArgs args)
		{
			Navigation.PopAsync();
		}

		async void OnClearCartClicked(object sender, EventArgs args)
		{
			var result = await DisplayAlert(null, AppResources.StoreBasketEmptyCartConfirmationMessage, AppResources.Yes, AppResources.No);
			if (result) await App.StoreBasketVM.ClearBasket();
		}

		#endregion

		#region Product Events

		public void DismissKeyboardProxy(object sender, EventArgs args) 
		{
			// Dismiss the keyboard.
			var entry = _editingTf;

			if (_editingTf != null)
			{
				_editingTf.IsEnabled = false;
				_editingTf.IsEnabled = true;
				_editingTf = null;
			}
			TapHandler.IsVisible = false;

			// Update the basket.
			OnQuantityEditComplete (entry, null);
		}

		async void OnTextChanged(object sender, TextChangedEventArgs args)
		{
			// If the text value is null, the view is loading.
			if (args.OldTextValue != null) {
				if (sender != _editingTf) {
					TapHandler.IsVisible = true;
					_editingTf = sender as Entry;
				}
			}
		}

		async void OnQuantityEditComplete(object sender, EventArgs args)
		{
			if (sender == null || !(sender is Entry)) return;
			
			var entry = sender as Entry;
			if (entry == null || entry.BindingContext == null) return;

			entry.Unfocus ();

			// Parse quantity 
			int quantity = 0;
			int.TryParse(entry.Text, out quantity);

			// Update the quantity and reload the basket
			var basket = entry.BindingContext as BasketProductOut;
			if (null != basket) {
				if (quantity != basket.Quantity) {
					await App.StoreBasketVM.EditProductQuantity (basket, quantity);
				}
			}
		}

		void OnDeleteButtonClicked(object sender, EventArgs args) 
		{
			if (sender == null || !(sender is View)) return;

			var view = sender as View;
			if (view == null || view.BindingContext == null) return;

			// Remove product from basket and reload the basket
			App.StoreBasketVM.RemoveProductFromBasket(view.BindingContext as BasketProductOut);
		}

		#endregion

		#region Search Events

		protected async void OnSearch(string searchValue)
		{
			// Don't search if the query is null
			if (string.IsNullOrEmpty(searchValue)) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			//await Navigation.PushAsync(new StoreSearchPage(new StoreSearchViewModel(searchValue)));
			await Navigation.PushAsync(new StoreSearchPage(searchValue));
		}

		#endregion

        #region Application Bar Settings

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.StoreBasketTitle;
        }

        #endregion

    }
}
