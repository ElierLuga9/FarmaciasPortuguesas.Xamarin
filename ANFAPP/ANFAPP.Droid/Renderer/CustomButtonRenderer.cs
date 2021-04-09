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
using ANFAPP.Views.Common;
using ANFAPP.Droid.Renderer;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using ANFAPP.Droid.Utils;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]

namespace ANFAPP.Droid.Renderer
{
    public class CustomButtonRenderer : ButtonRenderer
    {

        /// <summary>
        /// Called whenever a property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
			if (Control != null && this.Element != null)
            {
                if (e.PropertyName.Equals("CustomFont"))
                {
                    // Sets this custom typefont
                    var font = ((CustomButton)this.Element).CustomFont;
                    if (!string.IsNullOrEmpty(font)) SetCustomFont(font);
                }
                else if (e.PropertyName.Equals("BackgroundResource"))
                {
                    // Sets background resource
					SetBackgroundDrawable(((CustomButton)this.Element).BackgroundResource);
                }
				else if (e.PropertyName.Equals("AccessoryImage"))
				{
					SetAccessoryImage(((CustomButton)this.Element).AccessoryImage);
				}
				else if (e.PropertyName.Equals("TextAlignment"))
				{
					SetTextAlignment(((CustomButton)this.Element).TextAlignment);
				}
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

			if (Control != null  && this.Element != null)
            {
                // Sets this custom typefont
                var font = ((CustomButton)this.Element).CustomFont;
                if (!string.IsNullOrEmpty(font)) SetCustomFont(font);

                // Sets background resource
                SetBackgroundDrawable(((CustomButton)this.Element).BackgroundResource);

				// Sets the accessory image
				SetAccessoryImage(((CustomButton)this.Element).AccessoryImage);

				// Sets the Text Alignment
				SetTextAlignment(((CustomButton)this.Element).TextAlignment);
            }
        }

        /// <summary>
        /// Sets a custom typefont for this view.
        /// </summary>
        /// <param name="fontName"></param>
        protected void SetCustomFont(string fontName)
        {
            try
            {
                // Set the custom Typefont
                Typeface font = Typeface.CreateFromAsset(Forms.Context.Assets, "Fonts/" + fontName);
                if (font != null) ((Android.Widget.Button)this.Control).SetTypeface(font, TypefaceStyle.Normal);
            }
            catch
            { }
        }

		/// <summary>
		/// Sets a compound drawable to the left of the button
		/// </summary>
		/// <param name="drawableName"></param>
		protected void SetAccessoryImage(string drawableName)
		{
			if (string.IsNullOrEmpty(drawableName)) return;

			drawableName = drawableName.Replace(".png", string.Empty);
			Control.SetCompoundDrawablesWithIntrinsicBounds(Resources.GetDrawable(drawableName), null, null, null);
			Control.CompoundDrawablePadding = LayoutUtils.DpToPx(10, Resources);
		}

        /// <summary>
        /// Sets the background drawable for this view.
        /// </summary>
        /// <param name="drawableName"></param>
        protected void SetBackgroundDrawable(string drawableName)
        {
            if (drawableName == null)
            {
                // If null, remove background
                this.Control.SetBackgroundDrawable(null);
                return;
            }
            else if (string.IsNullOrEmpty(drawableName))
            {
				
                return;
            }

            try
            {
                // Set the background drawable
                var resources = Forms.Context.Resources;
                var resId = resources.GetIdentifier(drawableName, "drawable", Forms.Context.PackageName);
                if (resId > 0) this.Control.SetBackgroundDrawable(resources.GetDrawable(resId));
            }
            catch
            { }
        }

		/// <summary>
		/// Sets the Text Alignment
		/// </summary>
		/// <param name="alignment"></param>
		protected void SetTextAlignment(CustomButton.TextAlignments alignment)
		{
			if (alignment == CustomButton.TextAlignments.Default) return;

			const int padding = 10;
			switch (alignment)
			{
				case CustomButton.TextAlignments.Left:
					Control.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
					Control.SetPadding(LayoutUtils.DpToPx(padding, Resources), 0, 0, 0);
					break;
				case CustomButton.TextAlignments.Center:
					Control.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
					Control.SetPadding(0, 0, 0, 0);
					break;
				case CustomButton.TextAlignments.Right:
					Control.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
					Control.SetPadding(0, 0, LayoutUtils.DpToPx(padding, Resources), 0);
					break;
			}
		}
    }
}