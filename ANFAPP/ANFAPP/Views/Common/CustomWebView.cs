using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections;
using XLabs.Ioc;

using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using XLabs.Serialization;


namespace ANFAPP.Views.Common
{
    public class CustomWebView : WebView
    {
        public static readonly BindableProperty CustomSourceProperty = //BindableProperty.Create<CustomWebView, string>(p => p.CustomSource, null);
																		 BindableProperty.Create(nameof(CustomSource), typeof(string), typeof(CustomWebView), null);
        public static readonly BindableProperty LoadedProperty = //BindableProperty.Create<CustomWebView, bool>(p => p.Loaded, false);
																BindableProperty.Create(nameof(Loaded), typeof(bool), typeof(CustomWebView), false);
        public static readonly BindableProperty VideoUrlProperty = //BindableProperty.Create<CustomWebView, string>(p => p.VideoUrl, null);
																	BindableProperty.Create(nameof(VideoUrl), typeof(string), typeof(CustomWebView), null);
		public static readonly BindableProperty ScrollEnabledProperty = //BindableProperty.Create<CustomWebView, bool>(p => p.ScrollEnabled, true);
																		BindableProperty.Create(nameof(ScrollEnabled), typeof(bool), typeof(CustomWebView), true);

		public delegate bool ShouldLoadUrlEventHandler(string url);

		public event ShouldLoadUrlEventHandler OnShouldLoadUrl;

        public string CustomSource
        {
            get { return (string)GetValue(CustomSourceProperty); }
            set { SetValue(CustomSourceProperty, value); }
        }

        public string VideoUrl
        {
            get { return (string)GetValue(VideoUrlProperty); }
            set { SetValue(VideoUrlProperty, value); }
        }

        public bool Loaded
        {
            get { return (bool)GetValue(LoadedProperty); }
            set { SetValue(LoadedProperty, value); }
        }

		public bool ScrollEnabled
		{
			get { return (bool)GetValue(ScrollEnabledProperty); }
			set { SetValue(ScrollEnabledProperty, value); }
		}

		public bool ShouldLoadUrl (string url)
		{
			if (null != OnShouldLoadUrl)
				return OnShouldLoadUrl (url);

			return true;
		}
    }
}




