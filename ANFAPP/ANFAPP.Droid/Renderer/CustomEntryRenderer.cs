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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Util;
using ANFAPP.Droid.Utils;
using Android.Graphics.Drawables;
using Android.Content.Res;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace ANFAPP.Droid.Renderer
{
    public class CustomEntryRenderer : EntryRenderer, Android.Views.View.IOnTouchListener
    {
	
		public bool OnTouch(Android.Views.View v, MotionEvent e)
		{
			// Only pickup Action UP
			if (e.Action != MotionEventActions.Up) return false;

			// Validate if drawable exists
			var drawable = Control.GetCompoundDrawables()[2];
			if (drawable == null) return false;

			if (e.GetX() >= (v.Right - LayoutUtils.DpToPx(55, Resources)))
			{
				var element = (CustomEntry)this.Element;
                element.OnAccessoryTapped();
			}

			return false;
		}

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
                var element = (CustomEntry)this.Element;

                if (e.PropertyName.Equals ("CustomFont")) {
                    // Sets this custom typefont
                    var font = ((CustomEntry)this.Element).CustomFont;
                    if (!string.IsNullOrEmpty (font))
                        SetCustomFont (font);
                } else if (e.PropertyName.Equals ("BackgroundResource")) {
                    // Sets background resource
                    SetBackgroundDrawable (element.BackgroundResource);
                } else if (e.PropertyName.Equals ("FontS")) {
                    // Sets the text color and size
                    SetTextProperties (element.FontS);
                } else if (e.PropertyName.Equals ("CustomPadding")) {
                    // Sets padding
                    SetCustomPadding (element.LeftPadding, element.TopPadding, element.RightPadding, element.BottomPadding);
                } else if (e.PropertyName.Equals ("AccessoryImage")) {
					// Sets the accessory image
					SetAccessoryImage(element.AccessoryImage);
				} else if (e.PropertyName.Equals (CustomEntry.CenterTextProperty.PropertyName)) {
					SetTextAlignment (element);
				} 
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Sets this custom typefont
                var font = ((CustomEntry)this.Element).CustomFont;
                if (!string.IsNullOrEmpty(font)) SetCustomFont(font);
                var element = (CustomEntry)this.Element;

                // Sets background resource
                SetBackgroundDrawable(element.BackgroundResource);

                // Sets the text color and size
                SetTextProperties(element.FontS);

                // Sets padding
                SetCustomPadding(element.LeftPadding, element.TopPadding, element.RightPadding, element.BottomPadding);

				// Sets the accessory image
				SetAccessoryImage(element.AccessoryImage);

				// Set the text alignment
				SetTextAlignment(element);
				/*if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
					Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
           		else
                Control.Background.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);*/
            }
        }

		protected void SetTextAlignment(CustomEntry entry)
		{
			if (entry.CenterText) {
				Control.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
			} else {
				Control.Gravity = GravityFlags.Start | GravityFlags.CenterVertical;
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
                if (font != null) ((EditText)this.Control).SetTypeface(font, TypefaceStyle.Normal);
            }
            catch
            { }
        }

        /// <summary>
        /// Sets the custom text properties.
        /// </summary>
        /// <param name="textcolor"></param>
        /// <param name="textSize"></param>
        protected void SetTextProperties(int textSize)
        {

            // Text Size
            ((EditText)Control).SetTextSize(ComplexUnitType.Dip, textSize * 1.0f);
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
				this.Control.SetBackgroundResource(0);
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
                var resId = resources.GetIdentifier(
					drawableName.Replace(".png", string.Empty), 
					"drawable", 
					Forms.Context.PackageName);

                if (resId > 0) this.Control.SetBackgroundResource(resId);
            }
            catch
            { }
        }

        /// <summary>
        /// Sets custom padding for this view.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        protected void SetCustomPadding(int left, int top, int right, int bottom)
        {
            this.Control.SetPadding(
                LayoutUtils.DpToPx(left, Resources),
                LayoutUtils.DpToPx(top, Resources),
                LayoutUtils.DpToPx(right, Resources),
                LayoutUtils.DpToPx(bottom, Resources));
        }

		/// <summary>
		/// Sets the right compound drawable.
		/// </summary>
		/// <param name="drawableName"></param>
		protected void SetAccessoryImage(string drawableName)
		{
			if (string.IsNullOrEmpty(drawableName)) 
			{
				Control.SetOnTouchListener(null);
				return;
			}

			drawableName = drawableName.Replace(".png", string.Empty);
			Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, Resources.GetDrawable(drawableName), null);
			Control.SetOnTouchListener(this);
		}

	}
}