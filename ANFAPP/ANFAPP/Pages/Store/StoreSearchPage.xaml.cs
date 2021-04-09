using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.Store
{
    public partial class StoreSearchPage : ANFStorePage
    {

		#region Properties

		private StoreSearchViewModel _viewModel = new StoreSearchViewModel();
		private bool _hideGenericsButton = false;

		#endregion

        #region Page Initialization

		private StoreSearchPage() : base() { }

		public StoreSearchPage(string searchValue) : this()
		{
			_viewModel.SearchValue = searchValue;
		}

		public StoreSearchPage(StoreSearchViewModel viewModel) : this()
		{
			BindingContext = _viewModel = viewModel;
		}

		public StoreSearchPage(StoreSearchViewModel viewModel, StoreNavigationWidget.SelectedTabEnum tab) : this(viewModel)
		{
			NavWidget.SetSelectedTab (tab);
		}

		public StoreSearchPage(StoreSearchViewModel viewModel, StoreNavigationWidget.SelectedTabEnum tab, bool hideGenerics) : this(viewModel, tab)
		{
			_hideGenericsButton = hideGenerics;
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _viewModel;
			ProductsList.LoadMoreCommand = new Command(LoadNextPage);
        }

        #endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			App.StoreBasketVM.OnLoadSuccess += OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError += OnLoadError;
			App.StoreBasketVM.OnLoadStart += OnCartLoadStart;

			if (_viewModel.InvalidSearchResults)
			{
				// Perform search if the filter/query parameters have changed
				_viewModel.PerformSearch();
			}
			else
			{
				LoadingView.IsVisible = false;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

			App.StoreBasketVM.OnLoadSuccess -= OnBasketLoadSuccess;
			App.StoreBasketVM.OnLoadError -= OnLoadError;
			App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
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

			ProductsList.SelectedItem = null;
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
			return !_hideGenericsButton;
		}

		#endregion

		#region Search Events

		protected void OnSearch(string searchValue)
		{
			// Perform Search
			_viewModel.PerformSearch();
		}

		protected void LoadNextPage()
		{
			if (_viewModel.HasMore && !_viewModel.IsLoading)
			{
				// Loads the next page, if one exists
				_viewModel.LoadNextPage();
			}
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
