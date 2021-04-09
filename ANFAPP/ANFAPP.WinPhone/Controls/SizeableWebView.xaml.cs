using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Sample.View.Components
{
    public sealed partial class SizeableWebView : UserControl
    {
        public SizeableWebView()
        {
            this.InitializeComponent();
            this.WebView = webView;
        }


        public void NavigateToContent(string htmlContent)
        {
            var htmlScript = "<script type=\"text/javascript\">"
                + "function getHeight() "
                + "{ return document.getElementById(\"wrapper\").offsetHeight.toString(); }; </script>";


			var htmlConcat = string.Format(
				"<!DOCTYPE html>" +
				"<html><head><meta name=\"viewport\" content=\"initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0\"/></head>" +
				"<body  style=\"margin:0;padding:0;font-size: 100%\">" +
				"<div id=\"wrapper\" style=\"width:100%; padding:0; margin:0;\">" + 
				"{0}" + 
				"</div></body>{1}</html>", htmlContent, htmlScript);

            webView.NavigateToString(htmlConcat);
			
        }

        #region Properties

		public WebBrowser WebView
        {
			get { return (WebBrowser)GetValue(WebViewProperty); }
            set { SetValue(WebViewProperty, value); }
        }


        // Using a DependencyProperty as the backing store for WebView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WebViewProperty =
			DependencyProperty.Register("WebView", typeof(WebBrowser), typeof(SizeableWebView), new PropertyMetadata(null));

        public bool IsContentAware
        {
            get { return (bool)GetValue(IsContentAwareProperty); }
            set { SetValue(IsContentAwareProperty, value); }
        }

        public static readonly DependencyProperty IsContentAwareProperty =
            DependencyProperty.Register("IsContentAware", typeof(bool), typeof(SizeableWebView), new PropertyMetadata(false));

        public bool InteractionsEnabled
        {
            get { return (bool)GetValue(InteractionsEnabledProperty); }
            set
            {
                SetValue(InteractionsEnabledProperty, value);

                grid.Visibility = value ? Visibility.Collapsed : Visibility.Visible;

            }
        }

        public static readonly DependencyProperty InteractionsEnabledProperty =
            DependencyProperty.Register("InteractionsEnabled", typeof(bool), typeof(SizeableWebView), new PropertyMetadata(false));

        #endregion


    }
}
