using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Resources;
using ANFAPP.Utils;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Store
{
	public partial class StoreProductDetailPage : ANFStorePage
	{
		#region Properties

		private bool _catalogHint;
		private StoreProductDetailViewModel _viewModel;

		#endregion

		#region Page Initialization

		public StoreProductDetailPage(int cnp, bool catalogHint) : this(cnp) 
		{ 
			_catalogHint = catalogHint;
		}

		public StoreProductDetailPage(int cnp) : base() 
		{ 
			_viewModel = new StoreProductDetailViewModel(SessionData.StorePharmacyId, cnp, App.FavoritesVM);
			_catalogHint = false;
		}

		protected override void InitPage()
		{
			InitializeComponent();

			base.InitPage();

			ContentScroll.IsVisible = false;
		}

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		#endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnProductLoadError += OnProductLoadError;
            App.StoreBasketVM.OnLoadStart += OnCartLoadStart;
            App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;
			App.StoreBasketVM.OnLoadError += OnLoadError;

			if (BindingContext == null) {
				// IsVisible is changed (in code) upon initialization, so we need to reset the binding.
				ContentScroll.SetBinding (ScrollView.IsVisibleProperty, new Binding ("Product", BindingMode.Default, ConverterResources.NotNullable));

				BindingContext = _viewModel;
				_viewModel.LoadData ();
			} else {
				LoadingView.IsVisible = false;

				//BindingContext = null;
				//BindingContext = _viewModel;
				_viewModel.UpdateFavorites ();
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnProductLoadError -= OnProductLoadError;

            App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
            App.StoreBasketVM.OnLoadSuccess -= OnLoadSuccess;
			App.StoreBasketVM.OnLoadError -= OnLoadError;
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

		#region Event Handlers

		public async void OnImageSelected(object sender, EventArgs args)
		{
			if (_viewModel.Product.CanZoomImage)
			{
				var page = new StoreImageDetailPage (_viewModel.Product);
				await Navigation.PushAsync (page);
			}
		}

		public async void OnAddToFavorites(object sender, EventArgs args)
		{
			// Do nothing if logged off - http://issue.innovagency.com/view.php?id=20774
			if (!SessionData.IsAuthenticated) return;
			
			if (_viewModel.IsInFavorites) 
			{
				await _viewModel.RemoveFromFavorites ();
			} 
			else 
			{
				await _viewModel.AddToFavorites ();
			}
		}

		public async void OnAddToCart(object sender, EventArgs args)
		{
			if (SessionData.IsAuthenticatedWithPharmacy) {
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
				
				// Add product to Cart
				ProductOut p = new ProductOut {
					CNP = _viewModel.Product.CNP,
					CNPEM = _viewModel.Product.CNPEM,
					Price = _viewModel.Product.Price,
					HasPoints = _viewModel.Product.HasPoints,
					Points = _viewModel.Product.Points
				};

				AddToCartButton.IsEnabled = false;
				await App.StoreBasketVM.AddProductToBasketWithCatalogRules(p, _catalogHint, _viewModel.Quantity);
				//if (_catalogHint) {
				//	await App.StoreBasketVM.AddCatalogProductToBasket(p, _viewModel.Quantity);
				//} else {
				//	await App.StoreBasketVM.AddProductToBasket(p, _viewModel.Quantity);
				//}
					
				AddToCartButton.IsEnabled = true;
			} else if (SessionData.IsAuthenticated) {
				// http://issue.innovagency.com/view.php?id=20566
				LoadingView.IsVisible = false;
				DisplayAlert("", AppResources.AddToCartNoPharmacyMessage, AppResources.OK);
			} else {
				// Do nothing while logged off - http://issue.innovagency.com/view.php?id=20774
				LoadingView.IsVisible = false;
				DisplayAlert("", AppResources.AddToCartNotLoggedMessage, AppResources.OK);
			}
		}

		async Task OnCartLoadStart()
		{
			LoadingView.IsVisible = true;
			LoadingMessage.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;

			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();

			if (_viewModel.Product != null) {
				props.Add("CNP", _viewModel.Product.CNP + string.Empty);
//				props.Add("CNPEM", _viewModel.Product.CNPEM + string.Empty);
			}
			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("PharmacyID", SessionData.StorePharmacyId);

			mixpanelWidget.TrackProperties("ProductView", props);
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		async void OnProductLoadError(string title, string message)
		{
			// Go to the store landing page if the product fails to load: http://issue.innovagency.com/view.php?id=20480 .
			/*if (NavigationUtils.ContainsPageOfType (Type.GetType("StoreLandingPage"), Navigation.NavigationStack)) {
				//NavigationUtils.PopToPageType (Type.GetType("StoreLandingPage"), Navigation);

			} else {
				//NavigationUtils.PushPageAndClearHistory (new StoreLandingPage (), Navigation);
				//Navigation.PushAsync(new StoreLandingPage());
			}*/

			// Navigate only if not already there.
			if (NavigationUtils.TopPageOfType(typeof(StoreLandingPage), Navigation)) return;
			//await NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);


			await Navigation.PushAsync(new StoreLandingPage());
			await DisplayAlert(null, "Este produto não existe na sua Farmácia", AppResources.OK);
			Navigation.RemovePage(this);

		}

		#endregion
	}
}

