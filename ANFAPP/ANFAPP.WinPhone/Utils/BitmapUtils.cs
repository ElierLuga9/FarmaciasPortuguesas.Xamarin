using ANFAPP.Logic.Utils;
using ANFAPP.WinPhone.Utils;
using Microsoft.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xamarin.Forms;

[assembly: Dependency(typeof(BitmapUtils))]
namespace ANFAPP.WinPhone.Utils
{
	public class BitmapUtils : IBitmapUtils
	{
		public byte[] ResizeImage(byte[] image, double width, double height)
		{
			byte[] resizedData;

			using (MemoryStream streamIn = new MemoryStream(image))
			{
				WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(streamIn, (int)width, (int)height);

				double ImageHeight = 0;
				double ImageWidth = 0;

				var OriginalHeight = ImageHeight = bitmap.PixelHeight;
				var OriginalWidth = ImageWidth = bitmap.PixelWidth;


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
            
				using (MemoryStream streamOut = new MemoryStream())
				{
					bitmap.SaveJpeg(streamOut, (int)ImageWidth, (int)ImageHeight, 0, 80);
					resizedData = streamOut.ToArray();
				}
			}

			return resizedData;
		}
	}
}
