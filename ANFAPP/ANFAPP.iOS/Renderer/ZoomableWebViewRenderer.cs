using System;
using System.Drawing;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;
using UIKit;

[assembly: ExportRenderer(typeof(ZoomableWebView), typeof(ZoomableWebViewRenderer))]


namespace ANFAPP.iOS.Renderer
{
	public class ZoomableWebViewRenderer : WebViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			var view = (UIWebView)NativeView;
			view.ScrollView.ScrollEnabled = true;
			view.ScalesPageToFit = true;
		}

	}
}
