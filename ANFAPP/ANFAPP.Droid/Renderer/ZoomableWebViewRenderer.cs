using ANFAPP.Droid.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ZoomableWebView), typeof(ZoomableWebViewRenderer))]

namespace ANFAPP.Droid.Renderer
{

	public class ZoomableWebViewRenderer : WebViewRenderer
	{

		#region ElementChanged

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.Settings.BuiltInZoomControls = true;
				Control.Settings.DisplayZoomControls = false;
				Control.Settings.UseWideViewPort = true;
				Control.Settings.LoadWithOverviewMode = true;
			}

		}

		#endregion

	}
}







