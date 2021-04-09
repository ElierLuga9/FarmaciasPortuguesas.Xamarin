using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using Microsoft.Phone.Controls;
using Sample.View.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class CustomWebViewRenderer : ViewRenderer<CustomWebView, SizeableWebView> // WebViewRenderer
	{

		#region Properties

		public CustomWebView ThisElement { get { return (CustomWebView)Element; } }

		#endregion

		#region Properties Changed

		protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
		{
			base.OnElementChanged(e);

			var webView = new SizeableWebView() { IsContentAware = true, InteractionsEnabled = true };

			webView.webView.IsScriptEnabled = true;
			webView.webView.Navigating += OnWebView_Navigating;
			webView.webView.LoadCompleted += OnWebView_LoadCompleted;

			SetNativeControl(webView);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (this.Control == null || this.Element == null) return;

			if (e.PropertyName.Equals("CustomSource"))
			{
				// Sets the custom HTML source
				SetCustomSource();
			}
			else if (e.PropertyName.Equals("VideoUrl"))
			{
				// Sets the video URL
				SetVideoSource();
			}
		}

		#endregion

		#region Source Setters

		protected void SetCustomSource()
		{
			Control.NavigateToContent(ThisElement.CustomSource);
		}

		protected void SetVideoSource()
		{
			var iFrame = 
				@"<html><head>" +
				@"<meta name=""viewport"" content=""width=100%, height=200, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"" />" +
				@"</head>" + 
				@"<body style=""margin: 0; padding: 0; overflow: hidden; -ms-content-zooming: none; width:100%; height: 200;"">" +
				@"<iframe width=""100%"" height=""200"" src=""" + ThisElement.VideoUrl + @""" frameborder=""0"" allowfullscreen></iframe>" +
				@"</body></html>";
			Control.NavigateToContent(iFrame);
		}

		#endregion

		#region WebView Event Handlers

		void OnWebView_ScriptNotify(object sender, NotifyEventArgs e)
		{
			var result = e.Value;
			if (string.IsNullOrEmpty(result)) return;

			int height = 0;
			int.TryParse(result, out height);

			if (height > 0) ThisElement.HeightRequest = height;
			ThisElement.Loaded = true;
		}

		void OnWebView_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			string contentHeight = (string)Control.webView.InvokeScript("eval", "getHeight();");
			ThisElement.HeightRequest = int.Parse(contentHeight);
			Control.webView.Height = int.Parse(contentHeight);
		}

		void OnWebView_Navigating(object sender, NavigatingEventArgs e)
		{
			if (!ThisElement.ShouldLoadUrl(e.Uri.ToString())) e.Cancel = true;
		}

		#endregion

	}
}
