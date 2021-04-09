using System;
using UIKit;
using CoreGraphics;

namespace ANFAPP.iOS.Renderer
{
    public class ANFTextField : UITextField
    {
        public UIEdgeInsets TextMargins { get; set; }

        public ANFTextField () : base() 
        {
            TextMargins = UIEdgeInsets.Zero;
        }

        public override CGRect EditingRect (CGRect forBounds)
        {
            //return TextMargins.InsetRect (base.EditingRect (forBounds));
            return TextMargins.InsetRect (forBounds);
        }

        public override CGRect PlaceholderRect (CGRect forBounds)
        {
            //return TextMargins.InsetRect (base.PlaceholderRect (forBounds));
            return TextMargins.InsetRect (forBounds);
        }

        public override CGRect TextRect (CGRect forBounds)
        {
            //return TextMargins.InsetRect (base.TextRect (forBounds));
            return TextMargins.InsetRect (forBounds);
        }

        public override CGRect RightViewRect (CGRect forBounds)
        {
            var rect = base.RightViewRect (forBounds);
            rect.Offset(new CGPoint(-10, 0));

            return rect;
        }
    }
}
