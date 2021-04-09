using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using Xamarin.Forms;
using System.Collections.Generic;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Store
{
    public partial class StoreLandingPage : ANFStorePage
    {
		#region Properties

		private bool _isInitialized = false;
		private StoreLandingViewModel _viewModel = new StoreLandingViewModel();

		#endregion

        #region Page Initialization

		public StoreLandingPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _viewModel;

			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("PharmacyID", SessionData.StorePharmacyId);
			mixpanelWidget.TrackProperties("StoreLandingPage", props);
        }

		#endregion

		#region Lifecycle Events


		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			App.StoreBasketVM.OnLoadSuccess += OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadStart += OnCartLoadStart;
      		

			if (!_isInitialized) 
			{ 
				LoadingView.IsVisible = true;
				_viewModel.LoadData();
				_isInitialized = true;
			}
			else
			{
				LoadingView.IsVisible = false;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

			App.StoreBasketVM.OnLoadSuccess -= OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError -= OnLoadError;
			App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
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

        #region Event Handlers

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var selectedItem = ProductsList.SelectedItem;
			ProductsList.SelectedItem = null;

			if (selectedItem != null && selectedItem is ProductOut) {
				ProductOut p = selectedItem as ProductOut;
				if (p.CNP != null) {
					await Navigation.PushAsync (new StoreProductDetailPage (p.CNP.GetValueOrDefault ()));
				}
			}
		}

		async void SeeRelatedGenericsClicked(object sender, EventArgs args)
		{
			if (sender == null || !(sender is View)) return;

			var view = sender as View;
			if (view.BindingContext == null || !(view.BindingContext is ProductOut)) return;

			var p = view.BindingContext as ProductOut;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PushAsync(new StoreSearchPage(new StoreGenericSearchViewModel(p.CNPEM.GetValueOrDefault(), p.Name), StoreNavigationWidget.SelectedTabEnum.None, true));
		}

		async void ProductsHeaderClicked(object sender, EventArgs args)
		{
			if (sender == null || !(sender is Grid)) return;

			var context = ((Grid) sender).BindingContext;
			if (context == null || !(context is StoreLandingViewModel.ProductGroup)) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			switch (((StoreLandingViewModel.ProductGroup)context).Type)
			{
				case StoreLandingViewModel.GroupType.Campaigns:
					await Navigation.PushAsync(new StoreHighlightsPage(true, AppResources.StoreCatalogLabel));
					break;
				case StoreLandingViewModel.GroupType.Highlights:
					await Navigation.PushAsync(new StoreHighlightsPage(false, AppResources.StoreHighlightsLabel));
					break;
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
			NoResultsLabel.IsVisible = _viewModel.Products == null || _viewModel.Products.Count == 0;
		}

		void OnBasketLoadSuccess()
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
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
			return AppResources.StorePageTitle;
        }

		protected override string GetAppBarSubtitle()
		{
			return SessionData.StorePharmacyName;
		}

        #endregion

    }
}
