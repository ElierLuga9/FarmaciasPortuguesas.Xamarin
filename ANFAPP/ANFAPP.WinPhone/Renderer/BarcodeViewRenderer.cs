using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using ZXing;

[assembly: ExportRenderer(typeof(BarcodeView), typeof(BarcodeViewRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class BarcodeViewRenderer : ImageRenderer
	{

		protected bool IsBarcodeGenerated = false;


		 protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName.Equals("Code"))
            {
                GenerateBarcodeWrapper();   
            } else if (e.PropertyName.Equals("Width") || e.PropertyName.Equals("Height")) {
                GenerateBarcodeWrapper();
            }
        }

		private void GenerateBarcodeWrapper()
        {
            if (!string.IsNullOrEmpty(((BarcodeView)Element).Code) && Element.Width > 0 && Element.Height > 0)
            {
                Control.Source = GenerateBarcode((int)Element.Width, (int)Element.Height, ((BarcodeView)Element).Code);
                IsBarcodeGenerated = true;
            }
        }

		protected BitmapImage GenerateBarcode(int width, int height, string code)
        {
            var aux = new BarcodeWriter();
            aux.Format = BarcodeFormat.CODE_39;
            aux.Renderer = new ZXing.Rendering.WriteableBitmapRenderer() {
                Background = Colors.Transparent,
                Foreground = Colors.Black
            };

            aux.Options.Width = width;
            aux.Options.Height = height;

			var writableBitmap = aux.Write(code);

			// Convert the bitmap to a BitmapImage... because Windows Phone
			MemoryStream m = new MemoryStream();
			writableBitmap.SaveJpeg(m, writableBitmap.PixelWidth, writableBitmap.PixelHeight, 0, 100);

			// Read from the stream into a new BitmapImage object
			m.Position = 0;
			BitmapImage image = new BitmapImage();
			image.SetSource(m);

			return image;
        }

	}
}
