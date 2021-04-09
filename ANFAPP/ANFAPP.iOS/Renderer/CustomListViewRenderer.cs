using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms;
using System;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using UIKit;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace ANFAPP.iOS.Renderer
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged (ElementChangedEventArgs<ListView> e) {

            base.OnElementChanged(e);

            if (Control != null && this.Element != null) {
                Control.AllowsSelection = true;
                UpdateEdgeInsets ();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName.Equals ("SeparatorInset")) {
				UpdateEdgeInsets ();
			}
        }

        private void UpdateEdgeInsets()
        {
            var insets = ((CustomListView)this.Element).SeparatorInset;
            Control.SeparatorInset = new UIEdgeInsets ((nfloat)insets.Top, (nfloat)insets.Left, (nfloat)insets.Bottom, (nfloat)insets.Right);
        }
    }
}

