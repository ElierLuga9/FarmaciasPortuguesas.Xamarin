using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content.PM;


using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android;
using Android.Content;

namespace ANFAPP.Droid.ServiceProviders
{

	public class LocationEventArgs : EventArgs
	{
		public ANFAPP.Logic.Models.Objects.Location Location { get; set; }

		public LocationEventArgs(ANFAPP.Logic.Models.Objects.Location location) : base()
		{
			Location = location;
		}
	}

	public class GoogleLocationServices : GooglePlayAPIService, Android.Gms.Location.ILocationListener
	{

		#region Properties

		private static GoogleLocationServices INSTANCE = null;

		public event EventHandler<LocationEventArgs> OnLocationUpdated = delegate {};

		#endregion

		#region Constructors

		public GoogleLocationServices(Activity context) : base(context) { }

		#endregion

		#region Instanciation

		/// <summary>
		/// Returns the singleton, or instanciates a new one if required.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static GoogleLocationServices GetInstance(Activity context)
		{
			if (INSTANCE == null) INSTANCE = new GoogleLocationServices(context);
			return INSTANCE;
		}

		public override GoogleApiClient InitClient(Activity context)
		{
			return new GoogleApiClient.Builder(Context)
				.AddApiIfAvailable(LocationServices.API) // Location API
				.AddOnConnectionFailedListener(this)
				.AddConnectionCallbacks(this)
				.UseDefaultAccount()
				.Build();
		}

		public override int GetClientRequestCode()
		{
			return LOCATION_SERVICE_REQUEST_CODE;
		}

		#endregion

		#region Location Services

		/// <summary>
		/// Check if location services are available.
		/// </summary>
		/// <returns></returns>
		public bool LocationServicesAvailable()
		{
			if (!IsConnected) return false;

			var locationManager = Context.GetSystemService(Android.Content.Context.LocationService) as LocationManager;
			if (locationManager == null) return false;

			// Check if provicers are enabled
			return locationManager.IsProviderEnabled(LocationManager.GpsProvider) || locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
		}

		/// <summary>
		/// Get the latest location
		/// </summary>
		/// <returns></returns>
		public ANFAPP.Logic.Models.Objects.Location CurrentUserLocation()
		{
			if (!IsConnected) return null;

			// Get Last location
			var loc = LocationServices.FusedLocationApi.GetLastLocation(Client);
			return new ANFAPP.Logic.Models.Objects.Location()
			{
				Latitude = loc.Latitude,
				Longitude = loc.Longitude
			};
		}

		/// <summary>
		/// Start location updates
		/// </summary>
		public void StartUpdatingLocation()
		{
			if (!IsConnected) return;

			var locRequest = new LocationRequest();
			locRequest.SetInterval(200);
			locRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
			LocationServices.FusedLocationApi.RequestLocationUpdates(Client, locRequest, this);

		}
		/// <summary>
		/// Stop location updates
		/// </summary>
		public void StopUpdatingLocation()
		{
			if (!IsConnected) return;

			LocationServices.FusedLocationApi.RemoveLocationUpdates(Client, this);
		}

		#endregion

		#region Location Updates

		public void OnLocationChanged(Android.Locations.Location location)
		{
			OnLocationUpdated(this, new LocationEventArgs(new ANFAPP.Logic.Models.Objects.Location()
			{
				Latitude = location.Latitude,
				Longitude = location.Longitude
			}));
		}

		#endregion

	}
}