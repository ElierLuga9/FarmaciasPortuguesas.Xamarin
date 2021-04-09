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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ANFAPP.Droid.Renderer;
using ANFAPP.Views.Common;
using Android.Graphics;
using ANFAPP.Droid.Utils;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]

namespace ANFAPP.Droid.Renderer
{
    public class CustomLabelRenderer : LabelRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

			if (Control != null && this.Element != null)
            {
                var element = (CustomLabel)this.Element;

                // Sets this custom typefont
                var font = element.CustomFont;
                if (!string.IsNullOrEmpty(font)) SetCustomFont(font);

                // Sets padding
                SetCustomPadding(element.LeftMargin, element.TopMargin, element.RightMargin, element.BottomMargin);

                // Set underline
                SetUnderline(element.IsUnderline);
            }
        }

        /// <summary>
        /// Called whenever a property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            if (Control != null)
            {
                var element = (CustomLabel)this.Element;

                if (e.PropertyName.Equals("CustomFont") || e.PropertyName.Equals("Text") || e.PropertyName.Equals("FontSize") || e.PropertyName.Equals("TextColor"))
                {
                    // Sets this custom typefont
                    var font = element.CustomFont;
                    if (!string.IsNullOrEmpty(font)) SetCustomFont(font);
                }
                else if (e.PropertyName.Equals("CustomMargin"))
                {
                    // Sets padding
                    SetCustomPadding(element.LeftMargin, element.TopMargin, element.RightMargin, element.BottomMargin);
                }
                else if (e.PropertyName.Equals("IsUnderline"))
                {
                    SetUnderline(element.IsUnderline);
                }
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
                if (font != null) ((TextView)this.Control).SetTypeface(font, TypefaceStyle.Normal);
            }
            catch 
            { }
        }

        /// <summary>
        /// Sets the background drawable for this view.
        /// </summary>
        /// <param name="drawableName"></param>
        protected void SetBackgroundDrawable(string drawableName)
        {
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
        /// Sets the text as underline
        /// </summary>
        protected void SetUnderline(bool underline)
        {
            if (underline) Control.PaintFlags = PaintFlags.UnderlineText;
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