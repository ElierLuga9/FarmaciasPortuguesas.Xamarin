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
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms;
using ANFAPP.Droid.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using System.ComponentModel;
using ANFAPP.Logic.Database.Models;
using Android.Gms.Maps.Model;
using ANFAPP.Logic;
using ANFAPP.Droid.Utils;
using ANFAPP.Logic.Utils;

[assembly: ExportRenderer(typeof(NativeMap), typeof(MapRenderer_Droid))]
namespace ANFAPP.Droid.Renderer
{
    public class MapRenderer_Droid : MapRenderer, Android.Gms.Maps.GoogleMap.IOnMapLoadedCallback, IOnMapReadyCallback, GoogleMap.IOnInfoWindowClickListener, GoogleMap.IInfoWindowAdapter
    {

        #region Constants

        private readonly double PORTUGAL_MIN_LAT = 36.8298;
        private readonly double PORTUGAL_MAX_LAT = 42.3044;
        private readonly double PORTUGAL_MIN_LONG = -10.3572;
        private readonly double PORTUGAL_MAX_LONG = -6.0506;

        #endregion

        #region Properties

        private GoogleMap _map;
        private Dictionary<string, Pharmacy> _pharmacyMarkers;
		Marker polyline;
        #endregion

        #region Renderer Element Changes

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control != null && _map == null)
            {
                // Initialize the map object
                ((MapView)Control).GetMapAsync(this);
            }

				((NativeMap)Element).PinToAnnotations = ( () => { 
					PinMapToAnnotations(); 
				});
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("PharmacyList"))
            {
                // Update map
                UpdateMap();
            }
			if (e.PropertyName == ANFAPP.Views.Common.NativeMap.RouteCoordinatesProperty.PropertyName )
			{
				UpdatePolyLine();
			}
        }

        #endregion

        #region Map Updates

		  private void PinMapToAnnotations()
		  {
				// XXX: How to get markers in Android?
			   /*
				double max_long = -180.0;
				double min_long = 180.0;
				double max_lat = -90.0;
				double min_lat = 90.0;	
				*/
		  }
		private void UpdatePolyLine()
		{
			UpdateMap();
		}	
        /// <summary>
        /// Update the map with the list of pharmacies.
        /// </summary>
        protected void UpdateMap()
        {
            if (this.Control == null || this.Element == null|| _map == null) return;

            // Clear the map from all markers
            _map.Clear();

            // Get list of pharmacies
            ICollection<Pharmacy> pharmacies = ((NativeMap)this.Element).PharmacyList ?? new List<Pharmacy>();
            _pharmacyMarkers = new Dictionary<string, Pharmacy>();

            // Don't continue if there arent any pharmacies
            if (pharmacies.Count == 0)
            {
                // Animate to Portugal
                //AnimateMap(PORTUGAL_MIN_LAT, PORTUGAL_MIN_LONG, PORTUGAL_MAX_LAT, PORTUGAL_MAX_LONG);
                return;
            }

            double max_long = -180.0;
            double min_long = 180.0;
            double max_lat = -90.0;
            double min_lat = 90.0;

            // Add all pharmacies on the map
            foreach (Pharmacy pharmacy in pharmacies)
            {
                // Build Marker
                var markerBuilder = (new MarkerOptions())
                    .SetPosition(new LatLng(pharmacy.Latitude, pharmacy.Longitude))
                    .SetTitle(pharmacy.Name)
                    .SetSnippet(pharmacy.OpenStatusTxt);

                // Add marker to map
                var marker = _map.AddMarker(markerBuilder);
                marker.SetIcon(BitmapDescriptorFactory.FromResource(
                    Resources.GetIdentifier(
                        PharmacyUtils.IconForPharmacyDroid(pharmacy), 
                        "drawable",
                        Context.PackageName)));

                // Add new reference on the dictionary
                _pharmacyMarkers.Add(marker.Id, pharmacy);

                // Calculate regions for map position
                if (pharmacy.Latitude > max_lat)
                    max_lat = pharmacy.Latitude;

                if (pharmacy.Latitude < min_lat)
                    min_lat = pharmacy.Latitude;

                if (pharmacy.Longitude > max_long)
                    max_long = pharmacy.Longitude;

                if (pharmacy.Longitude < min_long)
                    min_long = pharmacy.Longitude;
            }

            // Animate to positions
		if (((NativeMap)this.Element).AdjustRegionToAnnotations) AnimateMap(min_lat, min_long, max_lat, max_long);
    

		}


		private void AnimateMap (double minLat, double minLong, double maxLat, double maxLong)
		{
			if (_map == null)
				return;
			// Initialize visible region

			// Add a margin around the pharmacies so that the outer POIs are not pinned to the edge of the screen.
			minLat = Math.Max(-85.0, minLat - 0.0025);
			maxLat = Math.Min(85.0, maxLat + 0.0025);
			minLong = Math.Max(-180.0, minLong - 0.0025);
			maxLong = Math.Min(180.0, maxLong + 0.0025);

			try
			{	
				_map.AnimateCamera (
					CameraUpdateFactory.NewLatLngBounds (
						new LatLngBounds (
							new LatLng (minLat, minLong),
							new LatLng (maxLat, maxLong)), 
							LayoutUtils.DpToPx(60, Context.Resources)));
			}
			catch (Java.Lang.IllegalStateException)
			{
				// Catch transition crash
			}
		}

        #endregion

        #region Callback Implementations

        /// <summary>
        /// Called when the Google Map is ready to be used.
        /// </summary>
        /// <param name="googleMap"></param>
        public void OnMapReady(GoogleMap googleMap)
        {
            // Initialize the map object
            _map = googleMap;
            _map.SetOnInfoWindowClickListener(this);
            _map.SetOnMapLoadedCallback(this);
            _map.SetInfoWindowAdapter(this);
            // Disable zoom buttons
            var settings = _map.UiSettings;
            settings.ZoomControlsEnabled = false;
			UpdatePolyLine();
        }

        public void OnMapLoaded()
        {
            // Update the map when loaded
            UpdateMap();
        }

        /// <summary>
        /// Called whenever an info window is clicked.
        /// </summary>
        /// <param name="marker"></param>
        public void OnInfoWindowClick(Marker marker)
        {
            if (_pharmacyMarkers == null || _pharmacyMarkers.Count == 0) return;

            // Get pharmacy from dictionary and send it through the messaging center.
            var pharmacy = _pharmacyMarkers[marker.Id];
            if (pharmacy != null) MessagingCenter.Send<Pharmacy>(pharmacy, Settings.MS_LOCATOR_PHARMACY_TAPPED);
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            LinearLayout info = new LinearLayout(Context);
            info.Orientation = Orientation.Vertical;

            TextView title = new TextView(Context);
            title.SetTextColor(Android.Graphics.Color.Black);
            title.Gravity = GravityFlags.Center;
            title.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            title.SetText(marker.Title, TextView.BufferType.Normal);

            TextView snippet = new TextView(Context);
            snippet.SetTextColor(Android.Graphics.Color.Gray);
            snippet.SetText(marker.Snippet, TextView.BufferType.Normal);

            info.AddView(title);
            info.AddView(snippet);

            return info;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
           return null;
        }

        #endregion

    }
}