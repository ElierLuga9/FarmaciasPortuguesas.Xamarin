using XLabs.Forms;
using Xamarin.Forms;
using ANFAPP.Droid.Renderer;
using ANFAPP.Views.Common;
using System.Text;
using System.Threading.Tasks;
using Android.Webkit;
using UriDefinition = Android.Net.Uri;
using WebViewDefinition = Android.Webkit.WebView;

using ANFAPP.Droid.Utils;
using System;
using System.ComponentModel;

using Android.Views;
using Android.Webkit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Support.V7.App;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]

namespace ANFAPP.Droid.Renderer
{

	public class CustomWebViewRenderer : WebViewRenderer
	{
		private WebViewDefinition _webView;
		private HtmlWebViewSource _httpSource;

		#region ElementChanged

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Control != null && this.Element != null)
			{
				var customWebView = (CustomWebView)this.Element;

				if (e.PropertyName.Equals("CustomSource") && customWebView.CustomSource != null)
				{
					_httpSource.Html = customWebView.CustomSource;
					_webView.LoadData(_httpSource.Html, "text/html; charset=UTF-8", "utf-8");
				}
				else if (e.PropertyName.Equals("VideoUrl") && customWebView.VideoUrl != null)
				{
					var url = customWebView.VideoUrl;
					_httpSource.Html = @"<iframe width=""100%"" height=""100%"" src=""" + url + @""" frameborder=""0"" allowfullscreen></iframe>";
					_webView.LoadData(_httpSource.Html, "text/html; charset=UTF-8", "utf-8");
				}
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				var customWebView = (CustomWebView)this.Element;

				_httpSource = new HtmlWebViewSource();
				_webView = new WebViewDefinition(this.Context);
				_webView.SetWebViewClient(new Client((CustomWebView)this.Element));
				_webView.SetWebChromeClient(new ChromeClient((CustomWebView)this.Element));
				_webView.Settings.JavaScriptEnabled = true;
				_webView.VerticalScrollBarEnabled = false;

				_webView.Settings.BuiltInZoomControls = true;
				_webView.Settings.DisplayZoomControls = false;

				SetNativeControl(_webView);

				if (customWebView.CustomSource != null) {
					_httpSource.Html = ((CustomWebView)this.Element).CustomSource;
					_webView.LoadData(_httpSource.Html, "text/html; charset=UTF-8", "utf-8");
				}
				if (customWebView.VideoUrl != null) {
					var url = ((CustomWebView)this.Element).VideoUrl;
					_httpSource.Html = @"<iframe width=""100%"" height=""100%"" src=""" + url + @""" frameborder=""0"" allowfullscreen></iframe>";
					_webView.LoadData(_httpSource.Html, "text/html; charset=UTF-8", "utf-8");
				}
			}
		}
		#endregion

		#region WebClient
		private class Client : WebViewClient
		{

			private bool loadedOnce = false;

			//private readonly WeakReference<CustomWebViewRenderer> webHybrid;
			private readonly CustomWebView Element;

			public Client(CustomWebView element)
			{
				Element = element;
			}

			public override void OnPageFinished(WebViewDefinition view, string url)
			{
				base.OnPageFinished(view, url);

				if (!loadedOnce) 
				{ 
					loadedOnce = true;
					CalcHeight(view);

					
				}


				//view.LoadUrl("javascript:alert(document.body.offsetHeight)");

				//try 
				//{
				//	var b = new JavaScriptResult(elm);
				//	view.EvaluateJavascript("document.body.offsetHeight", b);
				//	view.SetMinimumHeight(b.height);	
				//} catch { }
			}

			public void CalcHeight(WebViewDefinition view)
			{
				view.LoadUrl("javascript: (function() { alert(document.body.offsetHeight); return false; })(); ");


				//var httpsource = new HtmlWebViewSource();
				//httpsource.Html = ((CustomWebView)this.Element).CustomSource;
				//view.LoadData(httpsource.Html, "text/html; charset=UTF-8", "utf-8");
			}

			public override bool ShouldOverrideUrlLoading(WebViewDefinition view, string url)
			{
				var shouldLoad = Element.ShouldLoadUrl(url);
				return !shouldLoad;
			}
		}

		private class ChromeClient : WebChromeClient
		{

			private readonly CustomWebView Element;

			public ChromeClient(CustomWebView element)
			{
				Element = element;
			}

			public override bool OnJsAlert(WebViewDefinition view, string url, string message, JsResult result)
			{
				try
				{
					int height = int.Parse(message);
					// 50 px have to be added to avoid scrollbar
					Element.HeightRequest = height;
					Element.Loaded = true;
				}
				catch { }
				result.Cancel();

				return true;
			}

		}

		#endregion
	}
}







