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
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Fitness;

namespace ANFAPP.Droid.ServiceProviders
{
	public abstract class GooglePlayAPIService : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{

		#region Constants

		/// <summary>
		/// Google Fit request code.
		/// </summary>
		public static readonly int GOOGLE_FIT_REQUEST_CODE = 98;

		/// <summary>
		/// Location Services request code.
		/// </summary>
		public static readonly int LOCATION_SERVICE_REQUEST_CODE = 99;

		#endregion

		#region Properties

		public GoogleApiClient Client { get; set; }
		public Activity Context { get; set; }

		/// <summary>
		/// Returns if connected.
		/// </summary>
		/// <returns></returns>
		public bool IsConnected
		{
			get 
			{
				return Client != null && Client.IsConnected;
			}
		}

		#endregion

		#region Events

		public event EventHandler OnConnectedEvent = delegate {};
		public event EventHandler OnConnectionFailedEvent = delegate { };

		#endregion

		#region Constructors

		public GooglePlayAPIService(Activity context)
		{
			Context = context;
			Client = InitClient(context);
		}

		#endregion

		#region Instanciation

		/// <summary>
		/// Initializes the Api Client.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public abstract GoogleApiClient InitClient(Activity context);

		/// <summary>
		/// Returns the request code.
		/// </summary>
		/// <returns></returns>
		public abstract int GetClientRequestCode();

		#endregion

		#region Client Connection Wrappers

		/// <summary>
		/// Connect to the client.
		/// </summary>
		public void Connect()
		{
			if (!Client.IsConnected && !Client.IsConnecting) Client.Connect();
		}

		/// <summary>
		/// Disconnect the client.
		/// </summary>
		public void Disconnect()
		{
			if (!Client.IsConnected) Client.Disconnect();
		}

		#endregion

		#region Callbacks

		public virtual void OnConnected(Bundle connectionHint) 
		{
			OnConnectedEvent(this, null);
		}

		public virtual void OnConnectionSuspended(int cause) { }

		public virtual void OnConnectionFailed(ConnectionResult result)
		{
			OnConnectionFailedEvent(this, null);

			if (!result.HasResolution)
			{
				// No resolution - Show the appropriate message.
				GoogleApiAvailability.Instance.GetErrorDialog(Context, result.ErrorCode, 0).Show();
				return;
			}

			try
			{
				// Try to resolve the problem. Possibly Google Fit permission.
				result.StartResolutionForResult(Context, GetClientRequestCode());
			} catch { }
		}

		#endregion

	}
}