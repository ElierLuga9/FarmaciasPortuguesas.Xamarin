using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages;
using ANFAPP.Pages.Store;
using ANFAPP.Resources;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Utils;

namespace ANFAPP.Pages.Articles
{
    public partial class ArticleWebView : ANFPage
    {
		private bool _pageLoaded;

        #region Page Initialization

        private string _articleUrl;

		private ArticleWebView() : base() { }

        public ArticleWebView(string articleUrl) : this()
        {
            _articleUrl = articleUrl;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

        #region Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_pageLoaded) {
                WebView.Source = _articleUrl;
                ApplicationBar.SetLeftButtonEventHandler(OnBackTapped);

                _pageLoaded = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #endregion

        #region Lifecycle Events

        protected override bool OnBackButtonPressed()
        {
            return PopWebView() || base.OnBackButtonPressed();
        }

        void OnBackTapped(object sender, EventArgs args)
        {
            if (!PopWebView()) {
                Navigation.PopAsync();
            }
        }

        private bool PopWebView() {
            if (WebView.CanGoBack)
            {
                WebView.GoBack();
                return true;
            }

            return false;
        }

        #endregion

        #region WebView Events

        void OnWebViewLoadStart(object sender, WebNavigatingEventArgs e)
        {
            LoadingView.IsVisible = true;
        }

        void OnWebViewLoadFinished(object sender, WebNavigatedEventArgs e)
        {
            LoadingView.IsVisible = false;
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

    }
}
