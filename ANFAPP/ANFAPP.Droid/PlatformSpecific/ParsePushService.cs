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
using Gcm.Client;
using ANFAPP.Logic;
using Newtonsoft.Json;
using ANFAPP.Logic.Models;
using Android.Media;
using WindowsAzure.Messaging;
using ANFAPP.Logic.Network.Azure;

namespace ANFAPP.Droid.PlatformSpecific
{

    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "pt.anf.farmaciasportuguesas" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "pt.anf.farmaciasportuguesas" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "pt.anf.farmaciasportuguesas" })]
    public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        
        public static string[] SENDER_IDS = new string[] { Settings.ANDROID_GCM_SENDER_ID };
    }

    [Service]
    public class GcmService : GcmServiceBase
    {
        public GcmService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

        protected override async void OnRegistered(Context context, string registrationId)
        {
            //Receive registration Id for sending GCM Push Notifications to Parse
            //await ParseRestClient.RegisterDeviceToken(registrationId);

			//CreateNotification(null, "Test");
			if (string.IsNullOrEmpty(registrationId)) return;
			var azureHub = new NotificationHub(Settings.AZURE_NOTIFICATION_HUB, Settings.AZURE_CONNECTION_STRING, context);
			try {
				// Register/Update the token with Azure
				azureHub.UnregisterAll(registrationId);
				azureHub.Register(registrationId, null);

				// Save the token in session
				AzureClient.RegisterDeviceToken(registrationId);
			} 
			catch (Exception ex) 
			{ 
			}
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            // Get Message
            var message = intent.GetStringExtra("data");
            if (string.IsNullOrEmpty(message)) return;

            var alert = JsonConvert.DeserializeObject<ParseNotification>(message);
            if (alert == null || string.IsNullOrEmpty(alert.Alert)) return;

			// Build intent
			Intent actionIntent = new Intent(this, typeof(SplashActivity));

			if (alert.Aid.HasValue) actionIntent.PutExtra(MainActivity.PARSE_INTENT_AREAID, alert.Aid.Value);
			if (!string.IsNullOrEmpty(alert.Ctx)) actionIntent.PutExtra(MainActivity.PARSE_INTENT_CONTEXTID, alert.Ctx);
			
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
                .SetContentText(alert.Alert)
				.SetStyle(
					new Notification.BigTextStyle()
						.BigText(alert.Alert)
						.SetBigContentTitle(AppResources.ApplicationName))
				.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetContentIntent(resultPendingIntent)
			    .SetAutoCancel(true);
			//if (alert.Aid.HasValue && alert.Aid.Value == 9)
			//{
			//	var notificationActionIntent = new Intent(this, typeof(DosageActivity));
			//	notificationActionIntent.PutExtra(MainActivity.PARSE_INTENT_CONTEXTID, alert.Ctx);

			//	// Dosage Alert
			//	builder.AddAction(
			//		new Notification.Action(
			//			Resource.Drawable.ic_done_black_24dp, 
			//			"Marcar como Tomada", 
			//			PendingIntent.GetActivity(
			//				this,
			//				0,
			//				notificationActionIntent, 
			//				PendingIntentFlags.UpdateCurrent)));
			//}

			var notification = builder.Build();

            // Get Notification Manager
            var manager = GetSystemService(Context.NotificationService) as NotificationManager;

            // Send Notification
            manager.Notify(0, notification);
        }

        protected override void OnUnRegistered(Context context, string registrationId) { }
        protected override void OnError(Context context, string errorId) { }

    }
}