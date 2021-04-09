using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Resources;


namespace ANFAPP.Pages.Articles
{
    public partial class ArticlesMainPage : ANFPage
    {
        #region Properties
        private ArticlesHighlightViewModel _viewmodel = new ArticlesHighlightViewModel();


        #endregion

        #region Page Initialization

        public ArticlesMainPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _viewmodel;

//			var mixpanelWidget = DependencyService.Get<IMixPanel>();
//			var props = new Dictionary<string, string>();
//			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
//			mixpanelWidget.TrackProperties("News_SaudeAZ", props);
        }

        #endregion

        #region Event Handlers

        void OnSuccess()
        {
            LoadingView.IsVisible = false;

			if (_viewmodel.MainArticle != null) {
				ArticlesList.Header = _viewmodel.MainArticle;
			}
        }

        void OnError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

        async Task OnStart()
        {
            LoadingView.IsVisible = true;
        }

        public async void OnMainArticleTapped(object sender, EventArgs args)
        {
            var page = new ArticlesListDetailPage(_viewmodel.MainArticle.Id);

            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
            await Navigation.PushAsync(page);
        }

        public async void OnListItemTapped(object sender, ItemTappedEventArgs args)
        {
            if (args.Item == null) return;
			ArticlesList.SelectedItem = 1;

            HighlightOut p = args.Item as HighlightOut;

            var page = new ArticlesListDetailPage(p.Id);
            
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
            
			await Navigation.PushAsync(page);
        }

        protected async void OnSearch(string searchValue)
        {
            // Don't search if the query is null
            if (string.IsNullOrEmpty(searchValue)) return;

            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new ArticlesSearchResult(searchValue));
        }


        #endregion

        #region Lifecycle Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewmodel.OnError += OnError;
            _viewmodel.OnLoadStart += OnStart;
            _viewmodel.OnSuccess += OnSuccess;

			if (_viewmodel.MainArticle == null) {
				_viewmodel.LoadData ();
			} else {
				LoadingView.IsVisible = false;
			}
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewmodel.OnError -= OnError;
            _viewmodel.OnLoadStart -= OnStart;
            _viewmodel.OnSuccess -= OnSuccess;
        }

        #endregion

        #region Application Bar Settings

        //testes
        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.ArticlesPageTitle;
        }

        #endregion


    }
}

