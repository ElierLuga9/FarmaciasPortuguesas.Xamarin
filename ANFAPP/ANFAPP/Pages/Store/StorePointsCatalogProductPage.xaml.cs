using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Pages.Store
{
	public partial class StorePointsCatalogProductPage : ANFStorePage
	{
		private StorePointsCatalogProductViewModel _viewModel;

		#region Page Initialization

		private StorePointsCatalogProductPage() : base() { }

		public StorePointsCatalogProductPage(long catId, string catName)
			: base() 
		{ 
			_viewModel = new StorePointsCatalogProductViewModel (catId, catName);
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

			BindingContext = _viewModel;

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

            App.StoreBasketVM.OnLoadStart += OnCartLoadStart;
            App.StoreBasketVM.OnLoadError += OnLoadError;
            App.StoreBasketVM.OnLoadSuccess += OnLoadSuccess;

			LoadingView.IsVisible = false;
			_viewModel.LoadData();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing ();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

            App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
            App.StoreBasketVM.OnLoadError -= OnLoadError;
            App.StoreBasketVM.OnLoadSuccess -= OnLoadSuccess;
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

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			ProductOut item = args.SelectedItem as ProductOut;

			if (item != null && item.CNP != null) {
				LoadingView.IsVisible = true;
				await Task.Delay (Settings.DEFAULT_LOADING_DELAY);

				await Navigation.PushAsync(new StoreProductDetailPage(item.CNP.GetValueOrDefault(), true));
			}

			ProductsList.SelectedItem = null;
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

		bool CanDisplayGenerics()
		{
			return false;
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

		void OnLoadSuccess()
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

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePageTitle;
		}

		#endregion
	}
}
