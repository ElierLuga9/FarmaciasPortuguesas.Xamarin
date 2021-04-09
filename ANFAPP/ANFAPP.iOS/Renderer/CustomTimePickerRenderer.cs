using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms;
using System;
using Xamarin.Forms.Platform.iOS;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

[assembly: ExportRenderer(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]

namespace ANFAPP.iOS.Renderer
{
	public class CustomTimePickerRenderer : TimePickerRenderer
	{

		protected override void OnElementChanged (ElementChangedEventArgs<TimePicker> e) {
		
			base.OnElementChanged(e);

			if (Control != null && this.Element != null) {

				var thisElement = this.Element as CustomTimePicker;

				// Sets the custom font and size
                SetFont(thisElement.CustomFont, thisElement.FontSize);

                // Set custom background and stretch it
                SetCustomBackground(thisElement.BackgroundResource, true);

                // Apply Padding
                ApplyPadding(thisElement.LeftPadding, thisElement.TopPadding, thisElement.RightPadding, thisElement.BottomPadding);
			}
		}

        /// <summary>
        /// Sets a custom font, with a custom size for the current Control.
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="fontSize"></param>
        private void SetFont(string fontName, int fontSize)
        {
            if (fontSize <= 0 || string.IsNullOrEmpty(fontName)) return;

            // Build and initialize the custom font
            Control.Font = UIFont.FromName(fontName.Replace(".ttf", ""), fontSize);
        }

        /// <summary>
        /// Sets a custom background resource and stretches it at its center.
        /// </summary>
        /// <param name="res"></param>
        private void SetCustomBackground(string res, bool stretch)
        {
            if (string.IsNullOrEmpty(res)) return;

            // Get image from file
            var image = UIImage.FromFile(res);
            if (image == null) return;

            // Stretch image at its center
            if (stretch)
            {
                // Stretch image at its center
                image = image.StretchableImage(
                    (nint)Math.Round(image.Size.Width / 2.0),
                    (nint)Math.Round(image.Size.Height / 2.0));
            }

            // Set image
            Control.BorderStyle = UITextBorderStyle.None;
            Control.Background = image;
        }

        /// <summary>
        /// Apply a set padding to the view.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        private void ApplyPadding(int left, int top, int right, int bottom) 
        {
            // Set left
            Control.LeftView = new UIKit.UIView(new CGRect(0, 0, left, 1));
            Control.LeftViewMode = UIKit.UITextFieldViewMode.Always;

            // Set Right
            Control.RightView = new UIKit.UIView(new CGRect(0, 0, right, 1));
            Control.RightViewMode = UIKit.UITextFieldViewMode.Always;
        }
	}
}

