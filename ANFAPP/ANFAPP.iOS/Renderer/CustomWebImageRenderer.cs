using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;

[assembly: ExportRenderer(typeof(CustomWebImage), typeof(CustomWebImageRenderer))]
namespace ANFAPP.iOS.Renderer
{
	using System.Net;
	using System.Threading.Tasks;

	using Foundation;
	using UIKit;

	using Xamarin.Forms;
	using Xamarin.Forms.Platform.iOS;

	using XLabs.Platform.Services;
	using System.ComponentModel;

	/// <summary>
	/// Class CustomWebImageRenderer.
	/// </summary>
	public class CustomWebImageRenderer : ImageRenderer
	{
		private UIImage _defaultImage;

		private CustomWebImage _customWebImage
		{
			get { return (CustomWebImage)Element; }
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

			if (_customWebImage != null) {

				var imageUrl = _customWebImage.ImageUrl;
				if (imageUrl == null)
					imageUrl = string.Empty;

				LoadImage (imageUrl);
			}

			//if (e.OldElement == null && Element != null) {
				

			//	if (!string.IsNullOrEmpty (_customWebImage.DefaultImage) && null == _defaultImage) {
			//		_defaultImage = new UIImage (_customWebImage.DefaultImage);
			//	}

			//	if (null != _defaultImage) {
			//		Control.Image = _defaultImage;
			//		Control.ContentMode = UIViewContentMode.ScaleAspectFit;
			//	}
			//}

			//Control.Image = _defaultImage;

			//// Save url for comparison.
			//var imageUrl = null != _customWebImage ? _customWebImage.ImageUrl : null;
			//if (imageUrl == null) imageUrl = string.Empty;

			//// Check for connectivity.
			//var networkStatus = Reachability.InternetConnectionStatus();
			//var isReachable = networkStatus != NetworkStatus.NotReachable;

			//// Download Image URL, if one was set.
			//if (isReachable && !string.IsNullOrEmpty(imageUrl)) 
			//{
			//	var image = await GetImageFromWeb (imageUrl);
			//	if (image != null && _customWebImage != null && imageUrl.Equals(_customWebImage.ImageUrl)) 
			//	{
			//		// Make sure the view wasn't recycled
			//		Control.Image = image;
			//	}
			//}	
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (string.Equals(e.PropertyName, "ImageUrl"))
			{
				var imageUrl = _customWebImage.ImageUrl;
				if (imageUrl == null) imageUrl = string.Empty;

				LoadImage(imageUrl);
			}

		}

		private async void LoadImage(string imageUrl)
		{
			// Added a condition so the image won't be reloaded if the URL is the same as the previously loaded.
			if (string.Equals(_lastUrl, imageUrl)) return;

			if (Element != null)
			{
				if (!string.IsNullOrEmpty(_customWebImage.DefaultImage) && null == _defaultImage)
				{
					_defaultImage = new UIImage(_customWebImage.DefaultImage);
				}

				if (null != _defaultImage)
				{
					Control.Image = _defaultImage;
					Control.ContentMode = UIViewContentMode.ScaleAspectFit;
				}
			}

			Control.Image = _defaultImage;

			// Check for connectivity.
			var networkStatus = Reachability.InternetConnectionStatus();
			var isReachable = networkStatus != NetworkStatus.NotReachable;

			// Download Image URL, if one was set.
			if (isReachable && !string.IsNullOrEmpty(imageUrl))
			{
				var image = await GetImageFromWeb(imageUrl);
				if (image != null && _customWebImage != null && imageUrl.Equals(_customWebImage.ImageUrl))
				{
					// Make sure the view wasn't recycled
					Control.Image = image;
					
					// Save the last loaded URL
					_lastUrl = imageUrl;
				}
			}	
		}

		/// <summary>
		/// Download an image from the web.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private async Task<UIImage> GetImageFromWeb(string url)
		{
         // TODO: Implement Cache??
         try {
            using (var webclient = new WebClient ()) {
               var imageBytes = await webclient.DownloadDataTaskAsync (url);

               if (null != imageBytes && imageBytes.Length > 0) {
                  return UIImage.LoadFromData (NSData.FromArray (imageBytes));
               } 
            }
         } catch (WebException) {
            // The network is not available.
         }

         return null;
		}
	}
}