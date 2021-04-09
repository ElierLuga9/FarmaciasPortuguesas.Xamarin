using System;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.iOS.Renderer;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace ANFAPP.iOS.Renderer
{
    using UIKit;
    using CoreGraphics;
    using Xamarin.Forms.Platform.iOS;

    public class CustomEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged (ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null && this.Element != null) {
                var element = this.Element as CustomEditor;

                // Sets the custom font and size
                SetFont(element.CustomFont, element.FontS);

                // Set the border
                SetBorder(element.CustomBorderColor);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null && this.Element != null) {
                var element = this.Element as CustomEditor;

                if (e.PropertyName.Equals ("CustomFont")) {
                    SetFont (element.CustomFont, element.FontS);   
                } else if (e.PropertyName.Equals ("CustomBorderColor")) {
                    SetBorder(element.CustomBorderColor);
                } else if (e.PropertyName.Equals ("FontS")) {
                    // Sets the text color and size
                    SetFont (element.CustomFont, element.FontS);
                } 
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

        private void SetBorder(Color color)
        {
            Control.Layer.BorderWidth = 1;
            Control.Layer.BorderColor = color.ToUIColor ().CGColor;
        }
    }
}
