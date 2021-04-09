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
    public partial class ArticlesListDetailPage : ANFPage
    {
		private bool _pageLoaded;

        #region Page Initialization

        private ArticleDetailViewModel _viewmodel;
        private int _articleId;

		private ArticlesListDetailPage() : base() { }

        public ArticlesListDetailPage(int articleId) : this()
        {
            _articleId = articleId;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion
        #region Events
        async void OnSuggestedArticlesClicked(object sender, EventArgs args)
        {
            var view = sender as View;
            if (view.BindingContext == null || !(view.BindingContext is HighlightOut)) return;

            LoadingView.IsVisible = true;
            HighlightOut p = view.BindingContext as HighlightOut;

            var page = new ArticlesListDetailPage(p.Id);
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await NavigationUtils.PushPageWithNoHistory(page, Navigation);
            //await Navigation.PushAsync(page);
        }

        async void OnSuggestedProductClicked(object sender, EventArgs args)
        {
            var view = sender as View;
            if (view.BindingContext == null || !(view.BindingContext is ProductOut)) return;

            LoadingView.IsVisible = true;
            ProductOut p = view.BindingContext as ProductOut;

			var page = new StoreProductDetailPage(p.CNP.GetValueOrDefault());
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
            await Navigation.PushAsync(page);
        }

       
        void OnFacebookTap(object sender, EventArgs args) 
        {
            string link=AppResources.FacebookShareLink+_viewmodel.ArticleDetail.url;
            
            Device.OpenUri(new Uri(link));
        }

        void OnTwitterTap(object sender, EventArgs args)
        {

            string link = AppResources.TwitterShareLink + _viewmodel.ArticleDetail.url;

            Device.OpenUri(new Uri(link));
        }

        void OnGoogleTap(object sender, EventArgs args)
        {
 
            string link = AppResources.GooglePlusShareLink + _viewmodel.ArticleDetail.url;

            Device.OpenUri(new Uri(link));
        }


        void OnSuccess()
        {
			if (BindingContext == null) {
				ArticlesRelated.SetBinding (Grid.IsVisibleProperty, new Binding ("articlesList", BindingMode.Default, ConverterResources.HasElements));
				ProductsRelated.SetBinding (Grid.IsVisibleProperty, new Binding ("productsList", BindingMode.Default, ConverterResources.HasElements));
			}

            BindingContext = _viewmodel.ArticleDetail;
            ConfigureContentWebview();
    
            LoadingView.IsVisible = false;
        }

        void OnError(string title, string message)
        {
           // LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

        async Task OnStart()
        {
            LoadingView.IsVisible = true;
        }

        #endregion

        #region Lifecycle Events
        protected override void OnAppearing()
        {
            base.OnAppearing();

			if (_viewmodel == null) {
				_viewmodel = new ArticleDetailViewModel (_articleId);

				_viewmodel.OnError += OnError;
				_viewmodel.OnLoadStart += OnStart;
				_viewmodel.OnSuccess += OnSuccess;

				_viewmodel.LoadData ();
			} else {
				_viewmodel.OnError += OnError;
				_viewmodel.OnLoadStart += OnStart;
				_viewmodel.OnSuccess += OnSuccess;

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

		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);

			// We need to adjust the webview size after the table rotates.
			if (_pageLoaded) ConfigureContentWebview ();
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

        #region Auxiliary Methods

		public void ConfigureContentWebview()
        {
			if (_viewmodel == null || _viewmodel.ArticleDetail == null)
				return;

			var viewportWidth = Content.Width;
			System.Diagnostics.Debug.WriteLine ("Viewport Width: {0}", viewportWidth);

			//var vPadding = DeviceInfo.Instance.OS == OperatingSystemType.iOS ? "<p><br/></p>" : "";
			var innerContent = _viewmodel.ArticleDetail.content != null ? _viewmodel.ArticleDetail.content.Trim () : String.Empty;

			string content = string.Format("<html><head>"
				+ "<meta name=\"viewport\" content=\"width={0}; minimum-scale=1.0; maximum-scale=1.0; user-scalable=no\">"
				+ "<meta http-equiv='content-type' content='text/html; charset=utf-8'>"
				+ "<meta charset=\"UTF-8\">"
				+ "</head><body style=\"width: {0}; padding:0;margin:0\"><div style=\"margin-left: 15px; margin-right:15px; width: {1}px; \">"
				+ "{2}"
				+ "</div></body></html>", 
				viewportWidth, 
				viewportWidth - 30,
				innerContent); 
            
            Content.CustomSource = content;
            VideoBox.VideoUrl = _viewmodel.ArticleDetail.video;
			SocialWidget.IsVisible = !string.IsNullOrWhiteSpace (_viewmodel.ArticleDetail.content);

			_pageLoaded = true;
        }

        #endregion
    }
}
