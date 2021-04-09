using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ANFAPP.Logic.Utils;
using ANFAPP.iOS.Utils;
using Xamarin.Forms;
using System.IO;

using CoreGraphics;
using UIKit;

[assembly: Dependency(typeof(BitmapUtils))]
namespace ANFAPP.iOS.Utils
{
	public class BitmapUtils : IBitmapUtils
	{
		public byte[] ResizeImage(byte[] image, double width, double height)
		{
			// Load the bitmap
			var originalImage = ImageFromByteArray(image);

			double ImageHeight = 0;
			double ImageWidth = 0;

			var OriginalHeight = ImageHeight = originalImage.Size.Height;
			var OriginalWidth = ImageWidth = originalImage.Size.Width;


			if (OriginalHeight >= OriginalWidth && OriginalHeight > height)
			{
				ImageHeight = height;
				double aux = OriginalHeight / height;
				ImageWidth = OriginalWidth / aux;
			}
			else if (OriginalHeight < OriginalWidth && OriginalWidth > width)
			{
				ImageWidth = width;
				double aux = OriginalWidth / width;
				ImageHeight = OriginalHeight / aux;
			}

			height = ImageHeight;
			width = ImageWidth;

			UIGraphics.BeginImageContext(new CGSize(width, height));
			originalImage.Draw(new CGRect(0, 0, width, height));
			var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			var bytesImage = resizedImage.AsJPEG().ToArray();
			resizedImage.Dispose();
			return bytesImage;
		}
			
		public static UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIKit.UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}
	}
}
