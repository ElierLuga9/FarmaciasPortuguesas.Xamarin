using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Acr.BarCodes;
using XLabs.Ioc;
using XLabs.Platform.Device;
using Parse;

namespace ANFAPP.WinPhone
{
	public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
			SupportedOrientations = SupportedPageOrientation.Portrait;

			global::Xamarin.FormsMaps.Init();
			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new ANFAPP.App(App.FacebookAccessToken != null));

			BarCodes.Init();
		}

		protected override void OnNavigatedTo(NavigationEventArgs args)
		{
			var json = ParsePush.PushJson(args);
			
			object aid, ctx = null;
			if (json.TryGetValue("aid", out aid) || json.TryGetValue("ctx", out ctx))
			{
				string context = (ctx != null && ctx is string) ? ctx as string : null;

				// Open target page
				if (aid != null && aid is int && ((int)aid) != -1)
					((ANFAPP.App)Xamarin.Forms.Application.Current).OpenPage((AppArea)aid, context);
			}
		}
	}
}
