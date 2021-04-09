using ANFAPP.Logic.Models.Objects;
using ANFAPP.WinPhone.PlatformSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Geolocation;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationServices_WP))]
namespace ANFAPP.WinPhone.PlatformSpecific
{
	public class LocationServices_WP : ILocationServices
	{

		private Location _lastLocation { get; set; }
		private Geolocator _geolocator { get; set; }


		public void Init(object context) 
		{ 
			_geolocator = new Geolocator() 
			{
				ReportInterval = 100
			}; 
		}

        /// <summary>
        /// Check if location servicers are available
        /// </summary>
        /// <returns></returns>
        public bool LocationServicesAvailable()
        {
			return _geolocator.LocationStatus == PositionStatus.Ready;
        }

        /// <summary>
        /// Get the latest location
        /// </summary>
        /// <returns></returns>
        public ANFAPP.Logic.Models.Objects.Location CurrentUserLocation()
        {
			return _lastLocation;
        }

        /// <summary>
        /// Start location updates
        /// </summary>
        public void StartUpdatingLocation()
        {
			try 
			{ 
				if (_geolocator == null) Init(null);
				_geolocator.PositionChanged += OnPositionChanged;
			}
			catch (Exception ex) { }
        }

        /// <summary>
        /// Stop location updates
        /// </summary>
        public void StopUpdatingLocation()
        {
			if (_geolocator == null) return;
			_geolocator.PositionChanged -= OnPositionChanged;
			_geolocator = null;
        }

		#region Location Events

		void OnPositionChanged(object sender, PositionChangedEventArgs args)
		{
			if (args == null || args.Position == null) return;

			var location = _lastLocation = new Location() 
			{ 
				Latitude = args.Position.Coordinate.Latitude, 
				Longitude = args.Position.Coordinate.Longitude 
			};

			// Send update
			MessagingCenter.Send(location, ANFAPP.Logic.Settings.MS_LOCATOR_GOT_LOCATION);
		}

		#endregion

    }
}