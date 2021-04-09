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
using ANFAPP.Logic.Utils;
using ANFAPP.Droid.Utils;
using Xamarin.Forms;
using Android.Graphics;
using System.IO;

[assembly: Dependency(typeof(BitmapUtils))]
namespace ANFAPP.Droid.Utils
{
	public class BitmapUtils : IBitmapUtils
	{
		public byte[] ResizeImage(byte[] image, double width, double height)
		{
			// Load the bitmap 
			Bitmap originalImage = BitmapFactory.DecodeByteArray(image, 0, image.Length);
			
			double ImageHeight = 0;
			double ImageWidth = 0;
			
			var OriginalHeight = ImageHeight = originalImage.Height;
			var OriginalWidth = ImageWidth = originalImage.Width;
			
			
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

			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ImageWidth, (int)ImageHeight, false);
			
			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 80, ms);
				return ms.ToArray();
			}
		}
	}
}