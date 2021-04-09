
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

[assembly: ExportRenderer(typeof(BindablePicker), typeof(BindablePickerRenderer))]

namespace ANFAPP.Droid.Renderer
{
	public class BindablePickerRenderer : PickerRenderer
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
				var element = (BindablePicker)this.Element;

                if (e.PropertyName.Equals("CustomFont"))
                {
                    // Sets this custom typefont
					var font = ((BindablePicker)this.Element).CustomFont;
                    if (!string.IsNullOrEmpty(font)) SetCustomFont(font);
                }
                else if (e.PropertyName.Equals("BackgroundResource"))
                {
                    // Sets background resource
                    SetBackgroundDrawable(element.BackgroundResource);
                }
                else if (e.PropertyName.Equals("FontS"))
                {
                    // Sets the text color and size
                    SetTextProperties(element.TextColor, element.FontS);
                }
                else if (e.PropertyName.Equals("CustomPadding"))
                {
                    // Sets padding
                    SetCustomPadding(element.LeftPadding, element.TopPadding, element.RightPadding, element.BottomPadding);
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Sets this custom typefont
				var font = ((BindablePicker)this.Element).CustomFont;
                if (!string.IsNullOrEmpty(font)) SetCustomFont(font);
				var element = (BindablePicker)this.Element;

                // Sets background resource
                SetBackgroundDrawable(element.BackgroundResource);

                // Sets the text color and size
                SetTextProperties(element.TextColor, element.FontS);

                // Sets padding
                SetCustomPadding(element.LeftPadding, element.TopPadding, element.RightPadding, element.BottomPadding);
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
        protected void SetTextProperties(Xamarin.Forms.Color textcolor, int textSize)
        {
            if (textcolor != null)
            {
                // Text Color
                Control.SetTextColor(new Android.Graphics.Color(
                    (int)(textcolor.R * 255),
                    (int)(textcolor.G * 255),
                    (int)(textcolor.B * 255)));
            }

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
    }
}