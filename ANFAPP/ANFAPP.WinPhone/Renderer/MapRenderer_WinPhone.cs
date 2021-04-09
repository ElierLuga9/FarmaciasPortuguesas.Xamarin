using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.WP8;

[assembly: ExportRenderer(typeof(NativeMap), typeof(MapRenderer_WinPhone))]
namespace ANFAPP.WinPhone.Renderer
{
	public class MapRenderer_WinPhone : MapRenderer
	{

		#region Properties

		private NativeMap ThisElement { get { return (NativeMap)Element; } }
		private Pushpin SelectedPushpin = null;

		#endregion
		
		#region Element Changes

		protected override void OnElementChanged(Xamarin.Forms.Platform.WinPhone.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
		{
			base.OnElementChanged(e);

			this.Control.Tap += (sender, args) => {
				// Deselect any selected pushpin
				DeselectPushpin(SelectedPushpin);
			};

			UpdateMap();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName.Equals("PharmacyList"))
			{
				// Update map
				UpdateMap();
			}
		}

		#endregion

		#region Map Handlers

		private void UpdateMap() 
		{
			if (this.Control == null || ThisElement == null) return;

			this.Control.Layers.Clear();
			ICollection<Pharmacy> pharmacies = ((NativeMap)this.Element).PharmacyList ?? new List<Pharmacy>();
			if (pharmacies == null || pharmacies.Count == 0) return;

			double max_long = -180.0;
			double min_long = 180.0;
			double max_lat = -90.0;
			double min_lat = 90.0;

			// Get list of pharmacies
			MapLayer layer = new MapLayer();
			foreach (var pharmacy in pharmacies) 
			{
				// A pin must be created inside a "MapOverlay". This will represent the pin and its corresponding Tooltip
				Pushpin pin = new Pushpin();
				MapOverlay overlay = new MapOverlay();

				// Initialize location & template
				pin.GeoCoordinate = overlay.GeoCoordinate = new GeoCoordinate(pharmacy.Latitude, pharmacy.Longitude);
				pin.Template = App.Current.Resources["PushpinTemplate"] as ControlTemplate;
				pin.PositionOrigin = overlay.PositionOrigin = new System.Windows.Point(0, 1);

				// Initialize the pin resource
				pin.DataContext = new BitmapImage(
						new Uri("Resources/" + PharmacyUtils.IconForPharmacy(pharmacy), UriKind.RelativeOrAbsolute));

				// Initialize the template
				var tooltip = BuildTooltip(pharmacy);
				pin.Content = tooltip;
				
				// Tooltip event handler
				tooltip.Tap += (sender, args) => {
					MessagingCenter.Send<Pharmacy>(pharmacy, Settings.MS_LOCATOR_PHARMACY_TAPPED);
					args.Handled = true;
				};
				
				// Pin event handler
				pin.Tap += (sender, args) => {
					SelectPushpin(pin);
					args.Handled = true;
				};

				overlay.Content = pin;
				layer.Add(overlay);

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

			this.Control.Layers.Add(layer);

			// Animate to positions
			if (((NativeMap)this.Element).AdjustRegionToAnnotations) AnimateMap(min_lat, min_long, max_lat, max_long);
		}

		/// <summary>
		/// Builds the UIElement for the tooltip... because Windows Phone!
		/// </summary>
		/// <param name="pharmacy"></param>
		/// <returns></returns>
		private UIElement BuildTooltip(Pharmacy pharmacy)
		{
			var border = new Border() {
				Background = new SolidColorBrush(Colors.Black),
				Visibility = System.Windows.Visibility.Collapsed,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
				VerticalAlignment = System.Windows.VerticalAlignment.Top,
				Padding = new System.Windows.Thickness(10)
			};

			// Content holder for the tooltip
			var contentHolder = new System.Windows.Controls.StackPanel() {
				Orientation = Orientation.Vertical
			};

			// Pharmacy name & telephone
			contentHolder.Children.Add(new TextBlock() {
				FontWeight = System.Windows.FontWeights.Bold,
				Text = pharmacy.Name
			});
			contentHolder.Children.Add(new TextBlock()
			{
				Text = pharmacy.Phone
			});

			border.Child = contentHolder;
			return border;
		}

		private async void AnimateMapAsync(double minLat, double minLng, double maxLat, double maxLng)
		{
			await Task.Delay(2000);
			AnimateMap(minLat, minLng, maxLat, maxLng);
		}

		private void AnimateMap(double minLat, double minLng, double maxLat, double maxLng)
		{
			if (this.Control == null) return;
			//this.Control.SetView(new LocationRectangle(
			//	new GeoCoordinate(maxLat, minLng),
			//	new GeoCoordinate(minLat, maxLng)
			//), MapAnimationKind.Linear);

			var latSpan = (maxLat - minLat) / 2;
			var lngSpan = (maxLng - minLng) / 2;
			MapSpan startingRegion = new MapSpan(new Position(minLat + latSpan, minLng + lngSpan),
				latSpan + 0.075, lngSpan + 0.075);
			ThisElement.MoveToRegion(startingRegion);
		}

		#endregion

		#region Pins

		private void SelectPushpin(Pushpin pin)
		{
			if (SelectedPushpin != null) {
				DeselectPushpin(SelectedPushpin);
				SelectedPushpin = null;
			}

			if (pin == null || pin.Content == null) return;
			((UIElement)pin.Content).Visibility = System.Windows.Visibility.Visible;
			SelectedPushpin = pin;
		}

		private void DeselectPushpin(Pushpin pin)
		{
			if (pin == null || pin.Content == null) return;
			((UIElement) pin.Content).Visibility = System.Windows.Visibility.Collapsed;
		}

		#endregion

	}
}
