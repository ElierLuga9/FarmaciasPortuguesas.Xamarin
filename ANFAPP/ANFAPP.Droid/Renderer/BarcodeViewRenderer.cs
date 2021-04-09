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
using ZXing;
using ZXing.Rendering;
using Android.Graphics;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.Droid.Renderer;
using Xamarin.Forms.Platform.Android;
using ANFAPP.Droid.Utils;

[assembly: ExportRenderer(typeof(BarcodeView), typeof(BarcodeViewRenderer))]

namespace ANFAPP.Droid.Renderer
{
    public class BarcodeViewRenderer : ImageRenderer
    {

        /// <summary>
        /// True if the barcode already was generated.
        /// </summary>
        protected bool IsBarcodeGenerated = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!IsBarcodeGenerated && !string.IsNullOrEmpty(((BarcodeView)Element).Code))
            {
                Control.SetImageBitmap(GenerateBarcode(
					LayoutUtils.DpToPx(canvas.Width, Context.Resources),
					LayoutUtils.DpToPx(canvas.Height, Context.Resources), 
					((BarcodeView)Element).Code));

                IsBarcodeGenerated = true;
            }
        }

        protected Bitmap GenerateBarcode(int width, int height, string code)
        {
            var aux = new BarcodeWriter();
            aux.Format = BarcodeFormat.CODE_39;
            aux.Renderer = new ZXing.Rendering.BitmapRenderer() {
                Background = Android.Graphics.Color.Transparent,
                Foreground = Android.Graphics.Color.Black
            };


            aux.Options.Width = width;
            aux.Options.Height = height;

            return aux.Write(code);
        }
    }
}