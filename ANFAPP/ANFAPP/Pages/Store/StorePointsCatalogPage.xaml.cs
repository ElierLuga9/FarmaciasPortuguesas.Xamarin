using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Authorization;

namespace ANFAPP.Pages.Store
{
	public partial class StorePointsCatalogPage : ANFStorePage
	{
		#region Properties

		private StorePointsCatalogViewModel _viewModel = new StorePointsCatalogViewModel();

		#endregion

		#region Page Initialization

		public StorePointsCatalogPage() : base() { }

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _viewModel;
			_viewModel.SetCategory ();
		}

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

		#region Events

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			CategoryOut item = args.SelectedItem as CategoryOut;
			if (item == null)
				return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			if (item.CatId == -1) {
				// Ver Todos
				await Navigation.PushAsync(new StoreSearchPage(new StoreSearchCatalogViewModel(0), StoreNavigationWidget.SelectedTabEnum.Catalog, true));
			} else {
				await Navigation.PushAsync(new StoreSearchPage(new StoreSearchCatalogViewModel(item.CatId), StoreNavigationWidget.SelectedTabEnum.Catalog, true));
				//await Navigation.PushAsync (new StorePointsCatalogProductPage (item.CatId, item.CatName));
			}

			CategoryList.SelectedItem = null;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			App.StoreBasketVM.OnLoadSuccess += OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadStart += OnCartLoadStart;

			LoadingView.IsVisible = _viewModel.IsLoading;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing ();

			App.StoreBasketVM.OnLoadSuccess -= OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError -= OnLoadError;
			App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async Task OnCartLoadStart()
		{
			LoadingView.IsVisible = true;
			LoadingMessage.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async void OnBackTapped(object sender, EventArgs args)
		{
			await _viewModel.PopCategory ();
		}

		void OnBasketLoadSuccess()
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;

			if (_viewModel.NavCount == 0) {
                //////ApplicationBar.UnsetLeftButtonEventHandler (OnBackTapped);
                //////ApplicationBar.SetLeftButtonType(ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE);
				//ApplicationBar.SetLeftButtonType (ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU);
			} else {
				ApplicationBar.SetLeftButtonType (ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK);
				ApplicationBar.SetLeftButtonEventHandler (OnBackTapped);
			}
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			LoadingMessage.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		#endregion

	}
}

