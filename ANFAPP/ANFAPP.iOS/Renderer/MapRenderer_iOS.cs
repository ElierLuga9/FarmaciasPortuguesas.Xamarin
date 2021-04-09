using System;
using Xamarin.Forms.Maps.iOS;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;
using MapKit;
using CoreLocation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic;

[assembly: ExportRenderer (typeof (NativeMap), typeof (MapRenderer_iOS))]
namespace ANFAPP.iOS.Renderer
{
	public class MapRenderer_iOS : MapRenderer
	{
		protected MKMapView map;

		protected override void OnElementChanged (ElementChangedEventArgs<View> e) 
        {
            base.OnElementChanged (e);

            if (null != e.OldElement || Element == null) 
                return;

			if (Control != null) 
			{
				map = (MKMapView)Control;
                map.GetViewForAnnotation += GetViewForAnnotation;
                map.CalloutAccessoryControlTapped += CalloutAccessoryControlTapped;

				map.ShowsUserLocation = true;
                map.RotateEnabled = false;
			}

			/*((NativeMap)Element).PinToAnnotations = ( () => { 
				PinMapToAnnotations(); 
			});*/

            UpdateMapData ();
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName.Equals ("PharmacyList")) {
				PinMapToAnnotations();
				UpdateMapData ();
			}
		}

		private void PinMapToAnnotations()
		{
			// FIXME: the first time the list changes to map, the map is not adjusted.
			double max_long = -180.0;
			double min_long = 180.0;
			double max_lat = -90.0;
			double min_lat = 90.0;

			foreach (IMKAnnotation annotation in map.Annotations) {			
					CLLocationCoordinate2D coord = annotation.Coordinate;
				
					if (coord.Latitude > max_lat) 
						max_lat = coord.Latitude;

					if (coord.Latitude < min_lat) 
						min_lat = coord.Latitude;

					if (coord.Longitude > max_long) 
						max_long = coord.Longitude;

					if (coord.Longitude < min_long) 
						min_long = coord.Longitude;					
			}

			MKCoordinateRegion region;
			if (map.Annotations.Length > 0) 
			{
				double center_long = (max_long + min_long) / 2;
				double center_lat = (max_lat + min_lat) / 2;
				// If there is only one pharmacy don't zoom all the way.
				double deltaLat = Math.Max(Math.Abs(max_lat - min_lat), 0.0015);
				double deltaLong = Math.Max(Math.Abs(max_long - min_long), 0.0015);

				// Add a margin around the pharmacies so that the outer POIs are not pinned to the edge of the screen.
				deltaLat += 0.001;
				deltaLong += 0.001;

				CLLocationCoordinate2D center = new CLLocationCoordinate2D (center_lat, center_long);
				MKCoordinateSpan span = new MKCoordinateSpan (deltaLat, deltaLong);
				region = new MKCoordinateRegion (center, span);

				map.SetRegion(region, false);
			} 
			else if (map.UserLocation.Updating)
			{
				CLLocationCoordinate2D center = map.UserLocation.Coordinate;
				MKCoordinateSpan span = new MKCoordinateSpan (2.0, 3.0);
				region = new MKCoordinateRegion (center, span);

				map.SetRegion(region, false);
			}
		}

		public void UpdateMapData() 
		{
			map.RemoveAnnotations (map.Annotations);

			ICollection<Pharmacy> pharmacies = ((NativeMap)this.Element).PharmacyList ?? new List<Pharmacy>();

			double max_long = -180.0;
			double min_long = 180.0;
			double max_lat = -90.0;
			double min_lat = 90.0;

			foreach (Pharmacy p in pharmacies) {
				PharmacyAnnotation pAnnot = new PharmacyAnnotation ();
				pAnnot.SetCoordinate(new CLLocationCoordinate2D (p.Latitude, p.Longitude));
				pAnnot._Title = p.Name;
				pAnnot._Subtitle = p.OpenStatusTxt;
				pAnnot.Pharmacy = p;

				map.AddAnnotation (pAnnot);

				//calculate regions for map position
				if (p.Latitude > max_lat) 
					max_lat = p.Latitude;

				if (p.Latitude < min_lat) 
					min_lat = p.Latitude;

				if (p.Longitude > max_long) 
					max_long = p.Longitude;

				if (p.Longitude < min_long) 
					min_long = p.Longitude;
			}
			if (pharmacies.Count > 2) { 
				((NativeMap)this.Element).AdjustRegionToAnnotations=true ;
			} 
				// Validate if should adjust region
				if (!((NativeMap)this.Element).AdjustRegionToAnnotations) return;

			MKCoordinateRegion region;
			if (pharmacies.Count > 0) 
			{
				double center_long = (max_long + min_long) / 2;
				double center_lat = (max_lat + min_lat) / 2;
				// If there is only one pharmacy don't zoom all the way.
				double deltaLat = Math.Max(Math.Abs(max_lat - min_lat)*2, 0.0015);
				double deltaLong = Math.Max(Math.Abs(max_long - min_long)*2, 0.0015);

				// Add a margin around the pharmacies so that the outer POIs are not pinned to the edge of the screen.
				deltaLat += 0.001;
				deltaLong += 0.001;

				CLLocationCoordinate2D center = new CLLocationCoordinate2D (center_lat, center_long);
				MKCoordinateSpan span = new MKCoordinateSpan (deltaLat, deltaLong);
				region = new MKCoordinateRegion (center, span);

				map.SetRegion(region, true);
			} 
            else if (SessionData.UserLocation != null 
                && !(SessionData.UserLocation.Latitude > -double.Epsilon && SessionData.UserLocation.Latitude < double.Epsilon)
                && !(SessionData.UserLocation.Longitude > -double.Epsilon && SessionData.UserLocation.Longitude < double.Epsilon))
			{
                CLLocationCoordinate2D center = new CLLocationCoordinate2D(SessionData.UserLocation.Latitude, SessionData.UserLocation.Longitude); ;
				MKCoordinateSpan span = new MKCoordinateSpan (2.0, 3.0);
				region = new MKCoordinateRegion (center, span);

				map.SetRegion(region, true);
				//base.OnElementPropertyChanged (Element, new PropertyChangedEventArgs ("VisibleRegion"));
			}
		}
			

		#region Delegate events
		
        protected MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation) 
        {
            MKAnnotationView annotationView = null;

            if (annotation is PharmacyAnnotation)
            {
                annotationView = mapView.DequeueReusableAnnotation("pin");
                if (annotationView == null) {
                    annotationView = new MKAnnotationView (annotation, "pin");
                    annotationView.CanShowCallout = true;
                    annotationView.CalloutOffset = new CGPoint (0, 0);
                    annotationView.RightCalloutAccessoryView = new UIButton (UIButtonType.InfoLight);
                } 

                annotationView.Annotation = annotation;

                Pharmacy pharmacy = ((PharmacyAnnotation)annotation).Pharmacy;
                var iconName = PharmacyUtils.IconForPharmacy (pharmacy);
                if (null != iconName) {
                    annotationView.Image = new UIImage (iconName);
                }
            }

            return annotationView;
        }

        protected void CalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs args)
        {
            var pAnnot = (PharmacyAnnotation)args.View.Annotation;
            MessagingCenter.Send<Pharmacy> (pAnnot.Pharmacy, Settings.MS_LOCATOR_PHARMACY_TAPPED);
        }

		#endregion
	}
}
