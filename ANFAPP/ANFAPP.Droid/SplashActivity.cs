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
using Android.Content.PM;
using Xamarin;
using ANFAPP.Logic;
using Android.Media;

namespace ANFAPP.Droid
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
	[IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault }, DataScheme = "anfapp", DataHost = "open")]
	[IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault }, DataScheme = "https", DataHost = "bnc.lt", DataPathPrefix="/yExm")]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Lock orientation on smartphone
			//if (!Resources.GetBoolean(Resource.Boolean.isTablet)) RequestedOrientation = ScreenOrientation.Portrait;

			var intent = new Intent(this, typeof(MainActivity));
			if (Intent.Extras != null) intent.PutExtras(Intent.Extras);

			//dummyTest();

			// Start Main Activity
			StartActivity(intent);
		}


		protected void dummyTest()
		{

			Intent actionIntent = new Intent(this, typeof(SplashActivity));

			PendingIntent resultPendingIntent =
				PendingIntent.GetActivity(
					this,
					0,
					actionIntent,
					PendingIntentFlags.UpdateCurrent);

			// Build Notification
			var builder = new Notification.Builder(this)
				.SetContentTitle(AppResources.ApplicationName)
				.SetSmallIcon(Resource.Drawable.icon)
				.SetContentText("THIS IS A TEST NOTIFICATION")
				.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
				.SetContentIntent(resultPendingIntent);

			var notificationActionIntent = new Intent(this, typeof(DosageActivity));
			notificationActionIntent.PutExtra(MainActivity.PARSE_INTENT_CONTEXTID, "1967");

			// Dosage Alert
			builder.AddAction(
				new Notification.Action(
					Resource.Drawable.ic_done_black_24dp,
					"DONE",
					PendingIntent.GetActivity(
						this,
						0,
						notificationActionIntent,
						PendingIntentFlags.UpdateCurrent)));

			var notification = builder.Build();

			// Get Notification Manager
			var manager = GetSystemService(Context.NotificationService) as NotificationManager;

			// Send Notification
			manager.Notify(0, notification);
		}
	}
}