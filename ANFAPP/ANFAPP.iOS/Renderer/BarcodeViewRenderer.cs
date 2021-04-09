using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using ZXing;

[assembly: ExportRenderer(typeof(BarcodeView), typeof(BarcodeViewRenderer))]
namespace ANFAPP.iOS.Renderer
{
    public class BarcodeViewRenderer : ImageRenderer
    {

        /// <summary>
        /// True if the barcode already was generated.
        /// </summary>
        protected bool IsBarcodeGenerated = false;

        protected UIImage GenerateBarcode(nfloat width, nfloat height, string code)
        {
            var aux = new BarcodeWriter();
            aux.Format = BarcodeFormat.CODE_39;

            aux.Renderer = new ZXing.Rendering.BitmapRenderer() {
                //Background = Color.Transparent,
                //Foreground = Color.Black
            };

            aux.Options.Width = (int) width;
            aux.Options.Height = (int) height;

            return aux.Write(code);
        }

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
                Control.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);
                Control.Image = GenerateBarcode((nfloat)Element.Width, (nfloat)Element.Height, ((BarcodeView)Element).Code);
                IsBarcodeGenerated = true;
            }
        }
    }
}