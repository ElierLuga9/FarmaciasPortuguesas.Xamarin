using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.Droid.Renderer;

[assembly: ExportRenderer(typeof(CustomWebImage), typeof(CustomWebImageRenderer))]
namespace ANFAPP.Droid.Renderer
{
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;

	using Android.Graphics;

	using Xamarin.Forms;
	using Xamarin.Forms.Platform.Android;

	using XLabs.Platform.Services;
    using System.Net.Cache;
    using Android.Util;
    using System.IO;

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

			_lastUrl = null; // XXX: cell recycling breaks a couple of assumptions...

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

		async void LoadImage(string imageUrl)
		{
			if (string.Equals(_lastUrl, imageUrl)) return;

			var targetImageView = this.Control;

			// Show default image if one was set
			if (!string.IsNullOrEmpty(CustomWebImage.DefaultImage))
			{
				// Set default image
				var placeholderId = Resources.GetIdentifier(
					CustomWebImage.DefaultImage.Replace(".png", string.Empty), 
					"drawable", 
					Forms.Context.PackageName);

				targetImageView.SetImageResource(placeholderId);
			}

			if (string.IsNullOrEmpty(imageUrl)) return;

			// Call the url and get its bitmap
			try { 
				var remoteImage = await GetImageBitmapFromUrl(imageUrl);
					if (remoteImage != null && imageUrl.Equals(CustomWebImage.ImageUrl))
					{
						targetImageView.SetImageBitmap(remoteImage);
						
						// Save the URL for future comparison.
						_lastUrl = imageUrl;
					}
				}
			catch { } // Do nothing on failure
		}

		/// <summary>
		/// Call a webservice to obtain an image and return its bitmap
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private Task<Bitmap> GetImageBitmapFromUrl(string url)
		{
			return Task.Run<Bitmap>(() => {

				// Check for connectivity
				var networkStatus = Reachability.InternetConnectionStatus();
				var isReachable = networkStatus != NetworkStatus.NotReachable;

				// Validate if the view is still the same
				if (!isReachable || !url.Equals(CustomWebImage.ImageUrl)) return null;

				Bitmap imageBitmap = null;
				using (var webClient = new WebClient())
				{
					// Call webservice
					var imageBytes = webClient.DownloadData(url);

					// Validate if the view is still the same
					if (!url.Equals(CustomWebImage.ImageUrl)) return null;
					if (imageBytes != null && imageBytes.Length > 0)
					{
						// Device the bitmap
						imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
					}

					// Validate if the view is still the same
					if (!url.Equals(CustomWebImage.ImageUrl)) return null;
				}

				return imageBitmap;
			});
			
		}
	}
}