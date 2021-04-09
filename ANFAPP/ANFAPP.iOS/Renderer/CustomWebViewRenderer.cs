using System;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace ANFAPP.iOS.Renderer
{
	using System.ComponentModel;
	using CoreGraphics;
	using Foundation;
	using UIKit;

	public class CustomWebViewDelegate : UIWebViewDelegate
	{
		private CustomWebView _element;
		private WebViewRenderer _renderer;

		public CustomWebViewDelegate (CustomWebView element, WebViewRenderer renderer) : base()
		{	
			_element = element;
			_renderer = renderer;
		}

		public override void LoadingFinished (UIWebView webView)
		{
			string h = webView.EvaluateJavascript ("document.body.offsetHeight");
			//System.Diagnostics.Debug.WriteLine (string.Format("{1} height {0}", h, webView.Request.Url.AbsoluteString, h2));

			_element.HeightRequest = double.Parse (h);
		}

		public override void LoadFailed (UIWebView webView, NSError error)
		{
			System.Diagnostics.Debug.WriteLine (error.LocalizedDescription);
		}

		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			var shouldLoad = _element.ShouldLoadUrl (request.Url.AbsoluteString);
			return shouldLoad;
		}
	}

	public class CustomWebViewRenderer : WebViewRenderer
	{


		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);



			if (e.OldElement == null)
			{
				var webView = this;
				webView.ScalesPageToFit = true;
				webView.ScrollView.Bounces = false;
				webView.ScrollView.ScrollEnabled = true;
				webView.Delegate = new CustomWebViewDelegate(e.NewElement as CustomWebView, webView);

				e.NewElement.PropertyChanged += async (object sender, System.ComponentModel.PropertyChangedEventArgs args) =>
				{
					if (string.Equals(args.PropertyName, CustomWebView.CustomSourceProperty.PropertyName))
					{

						var webViewElement = sender as CustomWebView;

						var source = webViewElement.GetValue(CustomWebView.CustomSourceProperty) as string;
						if (null != source)
						{
							this.LoadFromString(source);
						}
					}
					else if (string.Equals(args.PropertyName, CustomWebView.ScrollEnabledProperty.PropertyName))
					{

						var webViewElement = sender as CustomWebView;
						this.ScrollView.ScrollEnabled = webViewElement.ScrollEnabled;
					}
					else if (string.Equals(args.PropertyName, CustomWebView.VideoUrlProperty.PropertyName))
					{

						var webViewElement = sender as CustomWebView;
						var source = webViewElement.GetValue(CustomWebView.VideoUrlProperty) as string;
						if (!string.IsNullOrWhiteSpace(source))
						{
							try
							{
								var iFrame = @"<iframe width=""100%"" height=""200"" src=""" + source + @""" frameborder=""0"" allowfullscreen></iframe>";
								this.LoadFromString(iFrame);
								// this.Load(new Uri(source));
							}
							catch (Exception ex)
							{
								System.Diagnostics.Debug.WriteLine(ex);
							}
						}
					}
				};
			}
		}

		public void Load(Uri uri)
		{
			if (uri != null)
			{
				this.LoadRequest(new NSUrlRequest(new NSUrl(uri.AbsoluteUri)));
			}
		}

		public void LoadContent(object sender, string contentFullName)
		{
			this.LoadHtmlString(contentFullName, null/*new NSUrl(NSBundle.MainBundle.BundlePath, true)*/);
		}

		public void LoadFromString(string html)
		{
			this.LoadContent(null, html);
		}

    }
}