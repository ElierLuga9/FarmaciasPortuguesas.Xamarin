using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomImage), typeof(CustomImageRenderer))]
namespace ANFAPP.WinPhone.Renderer
{

	public class CustomImageRenderer : ImageRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			SetImageSource();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName.Equals("Source"))
			{
				SetImageSource();
			}
		}

		#region Setters

		protected async void SetImageSource()
		{
			if (Control == null || Element == null) return; 

			string url = null;
			if (Element.Source != null && Element.Source is FileImageSource)
			{
				url = ((FileImageSource)Element.Source).File;
			}
			else if (Element.Source != null && Element.Source is UriImageSource)
			{
				url = ((UriImageSource)Element.Source).Uri.ToString();
			}

			if (url != null && !url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
			{
				/// Local resource
				if (!url.Contains("file:\\")) url = "Resources/" + url;
				if (!url.Contains(".png") && !url.Contains(".jpg")) url += ".png";
			}

			//this.Control.Source = !string.IsNullOrEmpty(url) ? new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute)) : null;

			// 
			if (!string.IsNullOrEmpty(url) && url.Contains("file:\\"))
			{
				this.Control.Source = await LoadImageFromIsolatedStorage(url);
			}
			else
			{
				this.Control.Source = !string.IsNullOrEmpty(url) ? new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute)) : null;
			}
		}

		public async Task<BitmapImage> LoadImageFromIsolatedStorage(string path)
		{
			string filename = Path.GetFileName(path);
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await localFolder.GetFileAsync(filename);

			using (Stream outputStream = await storageFile.OpenStreamForReadAsync())
			{
				var image = new BitmapImage();
				image.SetSource(outputStream);
				return image;
			}
		}

		#endregion

	}
}
