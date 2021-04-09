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
using ANFAPP.Droid.PlatformSpecific;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Objects;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Provider;
using Android.Locations;
using ANFAPP.Droid.ServiceProviders;
using Android;

[assembly: Dependency(typeof(LocationServices_Droid))]
namespace ANFAPP.Droid.PlatformSpecific
{
    public class LocationServices_Droid : ILocationServices
	{

		#region Instanciation

		private Activity Context = null;

		public void Init(object context)
		{
			if (context == null || !(context is Activity)) return;
			Context = context as Activity;
		}

		#endregion


        /// <summary>
        /// Check if location servicers are available
        /// </summary>
        /// <returns></returns>
        public bool LocationServicesAvailable()
        {
            return GoogleLocationServices.GetInstance(Context).LocationServicesAvailable();
        }

        /// <summary>
        /// Get the latest location
        /// </summary>
        /// <returns></returns>
        public ANFAPP.Logic.Models.Objects.Location CurrentUserLocation()
        {
			return GoogleLocationServices.GetInstance(Context).CurrentUserLocation();
	    }

         /// <summary>
        /// Start location updates
        /// </summary>
        public void StartUpdatingLocation()
		{
			GoogleLocationServices.GetInstance(Context).OnLocationUpdated += OnLocationChanged;
			GoogleLocationServices.GetInstance(Context).StartUpdatingLocation();
		}


		/// <summary>
		/// Stop location updates
		/// </summary>
		public void StopUpdatingLocation()
        {
			GoogleLocationServices.GetInstance(Context).OnLocationUpdated -= OnLocationChanged;
			GoogleLocationServices.GetInstance(Context).StopUpdatingLocation();
        }

		/// <summary>
		/// Called when location changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="location"></param>
        public void OnLocationChanged(object sender, ServiceProviders.LocationEventArgs location)
        {
			if (location == null) return;

            // Send update
            MessagingCenter.Send(location.Location, ANFAPP.Logic.Settings.MS_LOCATOR_GOT_LOCATION);

            // Stop Updates
			StopUpdatingLocation();
        }

    }
}