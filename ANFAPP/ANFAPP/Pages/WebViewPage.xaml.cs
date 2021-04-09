using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Pages.PharmacyLocator;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Utils;

namespace ANFAPP.Pages
{
    public partial class WebViewPage : ANFPage
    {

		private string _pageTitle = null;
		private string _pageUrl = null;

        #region Page Initialization

		public WebViewPage() : base() { }

        public WebViewPage(string url, string title)
            : base()
        {
            _pageUrl = url;
			_pageTitle = title;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			ApplicationBar.SetTitle(_pageTitle);

			if (string.IsNullOrEmpty(_pageUrl)) return;
			WebView.Navigated += WebView_Navigated;

			WebView.Source = _pageUrl;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			WebView.Navigated -= WebView_Navigated;
		}

		void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			LoadingView.IsVisible = false;
		}
		
        #endregion

        #region

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return null;
        }

        #endregion

    }
}
