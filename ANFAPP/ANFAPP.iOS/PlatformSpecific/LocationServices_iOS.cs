using System;
using MapKit;
using CoreLocation;
using UIKit;
using ANFAPP.iOS;
using System.Diagnostics;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic;
using ANFAPP.Pages.PharmacyLocator;

[assembly: Xamarin.Forms.Dependency (typeof(LocationServices_iOS))]
namespace ANFAPP.iOS
{
	public class LocationServices_iOS : ILocationServices
	{
		private CLLocationManager _locationManager;
		private ANFLocationDelegate _locationDelegate;

		public LocationServices_iOS () : base() 
		{
			_locationManager = new CLLocationManager();
			_locationDelegate = new ANFLocationDelegate ();
			_locationManager.Delegate = _locationDelegate;
			_locationManager.DesiredAccuracy = CLLocation.AccuracyHundredMeters;
		}

        public void Init(object context) { }

		public bool LocationServicesAvailable()
		{
			return CLLocationManager.Status == CLAuthorizationStatus.Authorized || 
				CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
				CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse;
		}

		public Location CurrentUserLocation()
		{
			CLLocation location = _locationManager.Location;

			var currentLocation = new Location () {
				Latitude = location.Coordinate.Latitude,
				Longitude = location.Coordinate.Longitude
			};

			return currentLocation;
		}

		public void StartUpdatingLocation()
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.Restricted || CLLocationManager.Status == CLAuthorizationStatus.Denied)
				return;

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0) && CLLocationManager.Status == CLAuthorizationStatus.NotDetermined) {
				_locationManager.RequestWhenInUseAuthorization ();
			} else {
				_locationManager.StartUpdatingLocation ();
			}
		}

		public void StopUpdatingLocation() 
		{
			_locationManager.StopUpdatingLocation ();
		}


		public class ANFLocationDelegate : CLLocationManagerDelegate
		{
			/*
			public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
			{
				if (newLocation != null) {

					var userLocation = new Location () {
						Latitude = newLocation.Coordinate.Latitude,
						Longitude = newLocation.Coordinate.Longitude
					};

					MessagingCenter.Send(userLocation, Settings.MS_LOCATOR_GOT_LOCATION);
				}
			}
			*/

			public override void LocationsUpdated (CLLocationManager manager, CLLocation[] locations)
			{
				CLLocation location = locations [locations.Length - 1];

				if (location != null) {

					var userLocation = new Location () {
						Latitude = location.Coordinate.Latitude,
						Longitude = location.Coordinate.Longitude
					};

					MessagingCenter.Send(userLocation, Settings.MS_LOCATOR_GOT_LOCATION);
				}
			}

			public override void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status) {
				Debug.WriteLine ("AuthorizationChanged, status: {0}", status);

				switch (status) {
				case CLAuthorizationStatus.AuthorizedAlways:
				case CLAuthorizationStatus.AuthorizedWhenInUse:
				case CLAuthorizationStatus.NotDetermined:
					manager.StartUpdatingLocation();
					break;
				case CLAuthorizationStatus.Denied:
					manager.StopUpdatingLocation();
					break;

				default:
					break;
				}
			}
		}

	}
}

