using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using XLabs.Platform.Services;

[assembly: ExportRenderer(typeof(CustomWebImage), typeof(CustomWebImageRenderer))]
namespace ANFAPP.WinPhone.Renderer
{

	/// <summary>
	/// Class WebImageRenderer.
	/// </summary>
	public class CustomWebImageRenderer : ImageRenderer
	{
		/// <summary>
		/// Gets the underlying control typed as an <see cref="CustomWebImage" />
		/// </summary>
		/// <value>The web image.</value>
		private CustomWebImage CustomWebImage
		{
			get { return (CustomWebImage) Element; }
		}

		private string _lastUrl = null;

		/// <summary>
		/// Called when [element changed].
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

            // Save url for comparison
			var imageUrl = CustomWebImage.ImageUrl;
			if (imageUrl == null) imageUrl = string.Empty;

			LoadImage(imageUrl);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (string.Equals(e.PropertyName, "ImageUrl"))
			{
				var imageUrl = CustomWebImage.ImageUrl;
				if (imageUrl == null) imageUrl = string.Empty;

				LoadImage(imageUrl);
			}
		}

		private void LoadImage(string imageUrl)
		{
			if (string.Equals(_lastUrl, imageUrl)) return;

			var targetImageView = this.Control;

			// Show default image if one was set
			if (!string.IsNullOrEmpty(CustomWebImage.DefaultImage))
			{
				// Set default image
				SetImageSource(CustomWebImage.DefaultImage);
			}

			if (string.IsNullOrEmpty(imageUrl)) return;

			// Call the url and get its bitmap
			try {

				this.Control.ImageFailed += Control_ImageFailed;
				this.Control.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
			} catch { } // Do nothing on failure
		}

		private void Control_ImageFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
		{
			this.Control.ImageFailed -= Control_ImageFailed;
			SetImageSource(CustomWebImage.DefaultImage);
		}

		protected void SetImageSource(string resourceName)
		{
			if (Control == null || Element == null) return;

			string uri = resourceName;
			if (uri != null && !uri.StartsWith("http"))
			{
				/// Local resource
				uri = "Resources/" + resourceName;
				if (!uri.Contains(".png")) uri += ".png";
			}

			this.Control.Source = !string.IsNullOrEmpty(uri) ? new BitmapImage(new Uri(uri, UriKind.Relative)) : null;
		}
	}
}