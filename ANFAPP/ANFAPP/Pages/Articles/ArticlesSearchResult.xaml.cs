using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out.Articles; 

using ANFAPP.Views;

namespace ANFAPP.Pages.Articles
{
    public partial class ArticlesSearchResult : ANFPage
    {
        #region Properties
        private ArticlesSearchViewModel _viewModel = new ArticlesSearchViewModel();

        #endregion
        #region Page Initialization

        public ArticlesSearchResult() : base() { }

        public ArticlesSearchResult(string searchValue)
            : this()
        {
            _viewModel.SearchValue = searchValue;
            _viewModel.CategoryName = @"Resultados obtidos para: """+ searchValue +@"""";
            _viewModel.CatId = -1;
        }

        public ArticlesSearchResult(ArticlesSearchViewModel viewModel)
            : this()
        {
            BindingContext = _viewModel = viewModel;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
            LoadingView.IsVisible = false;

            BindingContext = _viewModel;
            ArticlesList.LoadMoreCommand = new Command(LoadNextPage);

        }

        #endregion
        #region Event Handlers

        public async void OnListItemTapped(object sender, ItemTappedEventArgs args)
        {
            if (args.Item == null) return;

            // Show Loading
            LoadingView.IsVisible = true;

            HighlightOut p = args.Item as HighlightOut;

            var page = new ArticlesListDetailPage(p.Id);
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
            await Navigation.PushAsync(page);
        }


        async Task OnLoadStart()
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;

            noResultsLabel.IsVisible = (_viewModel.hasResults) ? false : true;
            ArticlesList.IsVisible = !noResultsLabel.IsVisible;
     
        }

        void OnLoadError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }
        #endregion
        #region Lifecycle Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnLoadStart += OnLoadStart;
            _viewModel.OnLoadError += OnLoadError;
            _viewModel.OnLoadSuccess += OnLoadSuccess;

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
            LoadingView.IsVisible = false;
            base.OnDisappearing();

            _viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
            _viewModel.OnLoadSuccess -= OnLoadSuccess;
        }

        #endregion
        #region Start Events
        protected void OnSearch(string searchValue)
        {
            // Perform Search
            _viewModel.SearchValue = searchValue;
            _viewModel.CategoryName = @"Resultados obtidos para: """ + searchValue + @"""";
            _viewModel.PerformSearch();
        }

        protected void LoadNextPage()
        {
            if (_viewModel.HasMore && !LoadingView.IsVisible)
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
            return AppResources.ArticlesPageTitle;
        }

        #endregion

       /* protected async void OnSearchPress(string searchValue)
        {
            // Don't search if the query is null
            if (string.IsNullOrEmpty(searchValue)) return;

            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new ArticlesSearchResult(searchValue));
        }*/
    }
}
