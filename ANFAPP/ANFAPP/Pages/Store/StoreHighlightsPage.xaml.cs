using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Pages.Store
{
	public partial class StoreHighlightsPage : ANFStorePage
	{
		private HighlightsViewModel _viewModel;

		#region Page Initialization

		private StoreHighlightsPage() : base() { }

		public StoreHighlightsPage(bool fromCatalog, string title) : base() 
		{ 
			_viewModel = new HighlightsViewModel (fromCatalog, title);
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _viewModel;
			ProductsList.LoadMoreCommand = new Command (LoadNextPage);
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

			if (_viewModel.InvalidSearchResults)
			{
				_viewModel.LoadData();
			}
			else
			{
				LoadingView.IsVisible = false;
			}
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

		#region Event Handlers

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			ProductOut item = args.SelectedItem as ProductOut;

			if (item != null && item.CNP != null)
			{
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

				await Navigation.PushAsync(new StoreProductDetailPage(item.CNP.GetValueOrDefault()));
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

		protected void LoadNextPage()
		{
			if (!_viewModel.IsLoading)
			{
				// Loads the next page, if one exists
				_viewModel.LoadNextPage();
			}
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

        async void FilterButtonClicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new StoreSearchFilterPage(_viewModel));
        }

        async void OrderButtonClicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new StoreSearchOrderPage(_viewModel));
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
