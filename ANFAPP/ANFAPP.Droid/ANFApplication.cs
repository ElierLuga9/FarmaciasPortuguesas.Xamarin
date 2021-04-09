using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ANFAPP.Logic;
using Xamarin;
using XLabs.Ioc;
using XLabs.Platform.Device;
using System.Threading;
using Android.Gms.Ads.Identifier;
using Java.Lang;

namespace ANFAPP.Droid
{
    #if DEBUG
    [Application(Debuggable=true)]
    #else
    [Application(Debuggable = false)]
    #endif
	public class ANFApplication : Application
    {

        private static ANFApplication _instance;
		public static string AAID = null;


        public ANFApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip) { }

        public override void OnCreate()
        {
            base.OnCreate();
            _instance = this;

			new System.Threading.Thread(new System.Threading.ThreadStart(() => {
				AAID = AdvertisingIdClient.GetAdvertisingIdInfo(BaseContext).Id;
			})).Start();

#if !DEBUG
            // Initialize Xamarin Insights
            //Insights.Initialize(Settings.XAMARIN_INSIGHTS_KEY, this);
#endif

			// Mindscape.Raygun4Net.RaygunClient.Attach("2yPX5OTn8Usv8a80KAXSRg==");

            // Initialize Device Resolver
			var container = new SimpleContainer();
			container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
			container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
			Resolver.SetResolver(container.GetResolver());
        }


        public static ANFApplication GetInstance()
        {
            return _instance;
        }

    }
}