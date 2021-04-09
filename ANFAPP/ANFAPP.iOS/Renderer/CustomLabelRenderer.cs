using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace ANFAPP.iOS.Renderer
{
   public class CustomLabelRenderer : LabelRenderer
   {

      protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
      {
         base.OnElementChanged (e);

         if (Control != null && this.Element != null) {

            var thisElement = this.Element as CustomLabel;

            // Set margin
            SetMargin (
               thisElement.LeftMargin, 
               thisElement.TopMargin, 
               thisElement.RightMargin, 
               thisElement.BottomMargin);

            // Sets the underline attribute
            SetUnderline (thisElement.IsUnderline, thisElement.Text);
         }
      }

        /// <summary>
        /// Sets the text underline.
        /// </summary>
        /// <param name="underline"></param>
        private void SetUnderline(bool underline, string text)
        {
            // Set the style
			if (null != text && underline) {
				this.Control.AttributedText = new NSAttributedString(
					text,
					underline ? 
					new UIStringAttributes() {
						UnderlineStyle = NSUnderlineStyle.Single
					} : null
				);
			} 
        }

        /// <summary>
        /// Sets the control margin.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        private void SetMargin(int left, int top, int right, int bottom)
        {
			if (this.Control.RespondsToSelector (new Selector ("setLayoutMargins:"))) {
				this.Control.LayoutMargins = new UIEdgeInsets (top, left, bottom, right);
			} else {
				CGRect f = this.Control.Frame;
				f.Offset(new CGPoint { X = f.Location.X + left, Y = f.Location.Y + top });
				this.Control.Frame = f;
			}
        }
	}
}

