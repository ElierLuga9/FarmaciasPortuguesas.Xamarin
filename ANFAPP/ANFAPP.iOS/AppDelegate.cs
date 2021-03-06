using System;

using ANFAPP.Logic;

using Foundation;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using Facebook.CoreKit;
using Mixpanel;
using Acr.BarCodes;
using System.Collections.Generic;

using Infragistics.XF.Controls;
//using BranchXamarinSDK;
using ANFAPP.Logic.Utils;
using WindowsAzure.Messaging;
using ANFAPP.Logic.Network.Azure;

namespace ANFAPP.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register("AppDelegate")]
	//public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	public partial class AppDelegate : global::XLabs.Forms.XFormsApplicationDelegate
	{
		public override UIWindow Window
		{
			get;
			set;
		}

		private App _application;

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			// Initialize Insights for Crash Reporting
		//	Insights.Initialize(ANFAPP.Logic.Settings.XAMARIN_INSIGHTS_KEY);

			// Initialize libraries.
			global::Xamarin.Forms.Forms.Init();
			global::Xamarin.FormsMaps.Init();
			Infragistics.XF.Initializers.Charts.Init();
			BarCodes.Init();


			// Branch initialization
			NSUrl url = null;
			if ((options != null) && options.ContainsKey(UIApplication.LaunchOptionsUrlKey))
			{
				url = (NSUrl)options.ValueForKey(UIApplication.LaunchOptionsUrlKey);
			}

			//BranchIOS.Init( );


			// XXX: Do not remove this line!
			Infragistics.XF.Controls.iOS.XFDataChartViewRenderer renderer = new Infragistics.XF.Controls.iOS.XFDataChartViewRenderer();

			//Mindscape.Raygun4Net.RaygunClient.Attach("2yPX5OTn8Usv8a80KAXSRg==");

			// Initialize Facebook.
			Facebook.CoreKit.Settings.AppID = "1620879804809132";
			Facebook.CoreKit.Settings.DisplayName = "Farmácias Portuguesas";



			if (Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split('.')[0].ToString()) < 8)
			{
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				app.RegisterForRemoteNotificationTypes(notificationTypes);
			}
			else
			{
				UIUserNotificationType userNotificationTypes = (UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound);
				UIUserNotificationSettings settings = UIUserNotificationSettings.GetSettingsForTypes(userNotificationTypes, null);
				app.RegisterUserNotificationSettings(settings);
			}

			var container = new SimpleContainer();
			container.Register<IDevice>(t => AppleDevice.CurrentDevice);
			container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
			//container.Register<INetwork>(t=> t.Resolve<IDevice>().Network);

			Resolver.SetResolver(container.GetResolver());

			LoadApplication(_application = new App());

			// XXX: how should we handle the return?
			ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

			// If the app was launched from a notification we need to handle the notification payload.
			NSDictionary notificationPayload = options != null ? options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary : null;
			if (null != notificationPayload && notificationPayload["aid"] != null)
			{
				HandleNotificationPayloadNoAlert(notificationPayload);
			}

			return base.FinishedLaunching(app, options);
		}

		public override void DidEnterBackground(UIApplication application)
		{
			DependencyService.Get<ILocationServices>().StopUpdatingLocation();
		}

		public override void WillTerminate(UIApplication application)
		{
			DependencyService.Get<ILocationServices>().StopUpdatingLocation();
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// Let branch handle it
			//BranchIOS.getInstance().SetNewUrl(url);

			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
		}

		// Support Universal Links
		public override bool ContinueUserActivity(UIApplication application,
		                                           NSUserActivity userActivity,
		                                           UIApplicationRestorationHandler completionHandler)
		{
		//	bool handledByBranch = BranchIOS.getInstance().ContinueUserActivity(userActivity, _application);
			return true;
		}

		public override void OnActivated(UIApplication application)
		{
			// Clear the notification badge.
			application.ApplicationIconBadgeNumber = -1;

			// Update the idiom and region. We need this to identify the decimal separator correctly.
			// string deviceLanguage = NSLocale.PreferredLanguages[0].Replace("_", "-");

			// The LocaleIdentifier might yield stuff like "en_PT". However, if we
			//      var country = NSLocale.CurrentLocale.CountryCode;
			//      string deviceRegion = string.Format ("{0}-{1}", country.ToLower (), country);
			//
			//  we may get invalid codes like us-US. So we just store the decimal separator
			App.DecimalSeparator = NSLocale.CurrentLocale.DecimalSeparator;

			// Activate Facebook installation.
			AppEvents.ActivateApp();

			// Init MixPanel - iOS only!!
			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//			mixpanelWidget.Identify (mixpanelWidget.DistinctId());
			mixpanelWidget.Track ("SessionStart");
		}

		#region Push Notifications

		public override void DidRegisterUserNotificationSettings(UIApplication app, UIUserNotificationSettings notificationSettings)
		{ 
			app.RegisterForRemoteNotifications(); 
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			System.Diagnostics.Debug.WriteLine("Push subscription failed: {0}", error.LocalizedDescription);
		}

		public async override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			//try
			//{
			//	var tokenStr = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
			//	await ParseRestClient.RegisterDeviceToken(tokenStr);
			//}
			//catch (Exception  ex)
			//{
			//	System.Diagnostics.Debug.WriteLine("Error registering for notifications in RegisteredForRemoteNotifications! Message: {0}", ex.Message);
			//}

			if (deviceToken == null) return;
			var azureHub = new SBNotificationHub(Logic.Settings.AZURE_CONNECTION_STRING, Logic.Settings.AZURE_NOTIFICATION_HUB);
			var registrationId = BitConverter.ToString(deviceToken.ToArray()).Replace("-", "");

			azureHub.UnregisterAllAsync(deviceToken, (error) => {
				if (error != null) return;

				azureHub.RegisterNativeAsync(deviceToken, null, (errorCallback) => {
					if (errorCallback != null) return;

					AzureClient.RegisterDeviceToken(registrationId);
				});
			});
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			if (null != userInfo["aps"] && userInfo["aps"] is NSDictionary)
			{

				NSObject aString = ((NSDictionary)userInfo["aps"])["alert"];
				bool hasCustomPayload = userInfo["aid"] != null;

				// Notify Parse that the push notification was opened.
				try
				{
					//ParseRestClient.HandleOpenPush(aString);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}

				// The application was launched from the background to handle the notification.
				if (application.ApplicationState == UIApplicationState.Inactive || application.ApplicationState == UIApplicationState.Background)
				{
					if (hasCustomPayload)
					{
						HandleNotificationPayloadNoAlert(userInfo);
					}
				}
				else
				{
					if (hasCustomPayload)
					{
						HandleNotificationPayload(userInfo);
					}
					else
					{
						// Display simple alerts...
						if (aString != null)
						{
							UIAlertView alert = new UIAlertView("", aString.ToString(), null, "OK", null);
							alert.Show();
						}
					}
				}
			}
		}

		private void HandleNotificationPayloadNoAlert(NSDictionary userInfo)
		{
			NSNumber aid = userInfo["aid"] as NSNumber;
			NSString ctx = userInfo["ctx"] as NSString;

			if (null != aid)
			{
				((App)Xamarin.Forms.Application.Current).OpenPage((AppArea)aid.Int32Value, ctx);
			
			}
		}

		private void HandleNotificationPayload(NSDictionary userInfo)
		{
			NSObject text = userInfo["alert"];
			NSNumber aid = userInfo["aid"] as NSNumber;
			NSString ctx = userInfo["ctx"] as NSString;

			string otherButton = aid != null ? "Ver" : null;

			if (text != null)
			{
				UIAlertView alert = new UIAlertView("", text.ToString(), null, "OK", otherButton);

				alert.Clicked += (sender, buttonArgs) =>
				{
					if (buttonArgs.ButtonIndex != alert.CancelButtonIndex)
					{
						if (null != aid)
						{
							((App)Xamarin.Forms.Application.Current).OpenPage((AppArea)aid.Int32Value, ctx);
						}
					}
				};
				alert.Show();
			}
		}

		#endregion
	}
}
