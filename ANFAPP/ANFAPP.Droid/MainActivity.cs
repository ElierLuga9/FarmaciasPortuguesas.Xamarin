using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using ANFAPP.Droid.PlatformSpecific;
using Gcm.Client;
using Xamarin.Facebook.AppEvents;
using Xamarin.Forms;
using Acr.BarCodes;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Android.Content;
using ANFAPP.Logic.Utils;
using ANFAPP.Droid.ServiceProviders;
using BranchXamarinSDK;
using ANFAPP.Logic;
using Mixpanel.Android.MPMetrics;
using Android;
using Android.Widget;
using System.Net;

[assembly: Permission(Name = Android.Manifest.Permission.Internet)]
[assembly: Permission(Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/app_name")]

namespace ANFAPP.Droid
{
	[Activity (Theme = "@style/Theme.CustomNormal",
		WindowSoftInputMode = SoftInput.AdjustPan, HardwareAccelerated = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

		#region Constants

		public static readonly string PARSE_INTENT_AREAID = "PARSE_AREAID";
		public static readonly string PARSE_INTENT_CONTEXTID = "PARSE_CONTEXTID";

		#endregion

		protected override void OnCreate (Bundle bundle)
		{
			 // You may use ServicePointManager here
	        ServicePointManager
	            .ServerCertificateValidationCallback +=
					(sender, cert, chain, sslPolicyErrors) => true;
			base.OnCreate (bundle);

			// Lock orientation on smartphone
			//if (!Resources.GetBoolean(Resource.Boolean.isTablet)) RequestedOrientation = ScreenOrientation.Portrait;

			global::Xamarin.Forms.Forms.Init (this, bundle);
			global::Xamarin.FormsMaps.Init (this, bundle);
			BarCodes.Init(() => this);

			BranchAndroid.Init(this, ANFAPP.Logic.Settings.BRANCHIO_API_KEY, Intent.Data, Intent.Extras);

			var app = new App();

			// Call this method to enable automatic session management
			BranchAndroid.getInstance().SetLifeCycleHandlerCallback(this, app);

			LoadApplication (app);

			// Initialize Facebook SDK
			DependencyService.Get<IFacebookSDK>().InitService(this);

			// Initialize Services
			DependencyService.Get<ILocationServices>().Init(this);
			DependencyService.Get<IHealthDataProvider>().Init(this);

			// Register for push notifications
			GcmClient.CheckDevice (this);
			GcmClient.CheckManifest (this);

			// Register Client
			GcmClient.Register (this, GcmBroadcastReceiver.SENDER_IDS);

			// Initialize the Mixpanel library for push notifications bad version

			/*mMixpanel = MixpanelAPI.getInstance(this, YOUR_MIXPANEL_PROJECT_ID_TOKEN);
			MixpanelAPI.People people = mMixpanel.getPeople();
			people.identify(THE_DISTINCT_ID_FOR_THE_USER);
			people.initPushHandling(YOUR_12_DIGIT_GOOGLE_SENDER_ID);*/

			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			if (SessionData.IsLogged == true)
			{
				

				MixpanelAPI.PeopleImpl p;
				//*p.InitPushHandling(
				mixpanelWidget.SharedInstanceWithToken(ANFAPP.Logic.Settings.MIXPANEL_KEY);
				mixpanelWidget.Identify(SessionData.PharmacyUser.Username);

			
			}
			//mixpanelWidget.Pushandling(Settings.ANDROID_GCM_SENDER_ID);
			//mixpanelWidget.Pushandling("500562454612");

			// Check if a notification was sent
			if (Intent.HasExtra(MainActivity.PARSE_INTENT_AREAID))
			{
				var aid = Intent.GetIntExtra(MainActivity.PARSE_INTENT_AREAID, -1);
				var ctx = Intent.GetStringExtra(MainActivity.PARSE_INTENT_CONTEXTID);

				// Open target page
				if (aid != -1) ((App)Xamarin.Forms.Application.Current).OpenPage((AppArea)aid, ctx);
			}
			string[] PermissionsLocation =
			{
			  Manifest.Permission.AccessCoarseLocation,
			  Manifest.Permission.AccessFineLocation,  
			  Manifest.Permission.ReadExternalStorage,
			  Manifest.Permission.Camera
			};
			const int RequestLocationId = 0;
			if ((int)Build.VERSION.SdkInt >= 23)
			{
				
				if ((CheckSelfPermission(PermissionsLocation[0]) == (int)Permission.Granted) && (CheckSelfPermission(PermissionsLocation[1]) == (int)Permission.Granted)
					&& (CheckSelfPermission(PermissionsLocation[2]) == (int)Permission.Granted) && (CheckSelfPermission(PermissionsLocation[3]) == (int)Permission.Granted))
					{

					}
					else
					{

						//need to request permission
						if (ShouldShowRequestPermissionRationale(PermissionsLocation[0]))
						{
							Toast.MakeText(this, "É necessario permissão para aceder à sua localização", ToastLength.Long).Show();
							return;

						}
						else if (ShouldShowRequestPermissionRationale(PermissionsLocation[1]))
						{
							Toast.MakeText(this, "É necessario permissão para utilizar o sistema de localizações", ToastLength.Long).Show();
							return;
						}
						else if (ShouldShowRequestPermissionRationale(PermissionsLocation[2]))
						{
							Toast.MakeText(this, "É necessário aceder ao seu armanezamento interno", ToastLength.Long).Show();
							return;

						}
						else if (ShouldShowRequestPermissionRationale(PermissionsLocation[3]))
						{
							Toast.MakeText(this, "É necessário aceder à sua camera", ToastLength.Long).Show();
							return;

						}
						//Finally request permissions with the list of permissions and Id
						RequestPermissions(PermissionsLocation, RequestLocationId);
					}

			}

		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Activate Facebook.
			AppEventsLogger.ActivateApp(this);
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Deactivate Facebook.
			AppEventsLogger.DeactivateApp(this);
		}

		protected override void OnStart()
		{
			base.OnStart();

			// Connect the Google API Clients
			GoogleLocationServices.GetInstance(this).Connect();
			//GoogleFitServices.GetInstance(this).Connect();
		}

		protected override void OnStop()
		{
			base.OnStop();

			// Stop Location Services
			GoogleLocationServices.GetInstance(this).StopUpdatingLocation();
			GoogleLocationServices.GetInstance(this).Disconnect();

			// Disconnect Fit client if connected
			GoogleFitServices.GetInstance(this).Disconnect();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == GooglePlayAPIService.GOOGLE_FIT_REQUEST_CODE && resultCode == Result.Ok)
			{
				// Google Play Client auth result - retry connection
				GoogleFitServices.GetInstance(this).Connect();
			}
			else if (requestCode == GooglePlayAPIService.LOCATION_SERVICE_REQUEST_CODE && resultCode == Result.Ok)
			{
				GoogleLocationServices.GetInstance(this).Connect();
			}

			DependencyService.Get<IFacebookSDK>().SendCallbackResult(requestCode, (int) resultCode, data);
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);

			BranchAndroid.getInstance().SetNewUrl(intent.Data);
		}

	}

	class Snackbar
	{
	}
}
