using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;
using Xamarin.Forms;
using System;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using UIKit;
using ObjCRuntime;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace ANFAPP.iOS.Renderer
{
	public class CustomEntryRenderer : EntryRenderer
	{
        private ANFTextField _tf;

        public CustomEntryRenderer()
        {
            _tf = new ANFTextField ();

            _tf.ShouldReturn = delegate(UITextField textField) {
                _tf.ResignFirstResponder();
                return true;
            };
                
            // IL_0000: ldarg.0
            // IL_0001: call instance !0 class Xamarin.Forms.Platform.iOS.VisualElementRenderer`1<class [Xamarin.Forms.Core]Xamarin.Forms.Entry>::get_Element()
            // IL_0006: ldsfld class [Xamarin.Forms.Core]Xamarin.Forms.BindableProperty [Xamarin.Forms.Core]Xamarin.Forms.Entry::TextProperty
            // IL_000b: ldarg.0
            // IL_000c: call instance !1 class Xamarin.Forms.Platform.iOS.ViewRenderer`2<class [Xamarin.Forms.Core]Xamarin.Forms.Entry, class [Xamarin.iOS]UIKit.UITextField>::get_Control()
            // IL_0011: callvirt instance string [Xamarin.iOS]UIKit.UITextField::get_Text()
            // IL_0016: callvirt instance void [Xamarin.Forms.Core]Xamarin.Forms.IElementController::SetValueFromRenderer(class [Xamarin.Forms.Core]Xamarin.Forms.BindableProperty, object)
            // IL_001b: ret
            _tf.EditingChanged += delegate(object sender, EventArgs e) {
                var element = Element as CustomEntry;
                element.Text = _tf.Text;
            };

			_tf.ShouldEndEditing = delegate(UITextField textField) {
				return true;
			};
        }

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e) 
        {
			base.OnElementChanged(e);

			if (e.OldElement == null) 
            {
				if (Control != null) {
					// The native Control is already set up.
					// XXX: maybe base.OnElementPropertyChanged doesn't play well with SetNativeControl? 
					_tf.TextColor = Control.TextColor;
					_tf.KeyboardType = Control.KeyboardType;
					_tf.KeyboardAppearance = Control.KeyboardAppearance;
					_tf.Placeholder = Control.Placeholder;
					_tf.Text = Control.Text;
					_tf.Font = Control.Font;
					_tf.SecureTextEntry = Control.SecureTextEntry;
					_tf.Delegate = Control.Delegate;
					_tf.InputDelegate = Control.InputDelegate;
				}

                SetNativeControl (_tf);   
            }

			if (Control != null && this.Element != null) {
                var thisElement = this.Element as CustomEntry;

                SetAccessoryView (thisElement.AccessoryImage);

				// Sets the custom font and size
                SetFont(thisElement.CustomFont, thisElement.FontS);

                // Set custom background and stretch it
                SetCustomBackground(thisElement.BackgroundResource, true);

                // Apply Padding
                ApplyPadding(thisElement.LeftPadding, thisElement.TopPadding, thisElement.RightPadding, thisElement.BottomPadding);

				// Apply text alignment
				SetCenterText (thisElement);
			}
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null && this.Element != null)
            {
                var element = (CustomEntry)this.Element;

				if (e.PropertyName.Equals ("CustomFont")) {
					SetFont (element.CustomFont, element.FontS);   
				} else if (e.PropertyName.Equals ("BackgroundResource")) {
					// Sets background resource
					SetCustomBackground (element.BackgroundResource, true);
				} else if (e.PropertyName.Equals ("FontS")) {
					// Sets the text color and size
					SetFont (element.CustomFont, element.FontS);
				} else if (e.PropertyName.Equals ("CustomPadding")) {
					// Sets padding
					ApplyPadding (element.LeftPadding, element.TopPadding, element.RightPadding, element.BottomPadding);
				} else if (e.PropertyName.Equals ("AccessoryImage")) {
					var imageName = element.AccessoryImage;
					SetAccessoryView (imageName);
				} else if (e.PropertyName.Equals ("Keyboard")) {
					if (element.Keyboard == Keyboard.Numeric) {
						Control.KeyboardType = UIKeyboardType.DecimalPad;
					} else {
						Control.KeyboardType = UIKeyboardType.Default;
					}
				} else if (e.PropertyName.Equals (CustomEntry.CenterTextProperty.PropertyName)) {
					SetCenterText (element);
				}
            }
        }

        private void OnTap (UIGestureRecognizer gesture) {
            var element = Element as CustomEntry;
            element.OnAccessoryTapped();
        }

		private void SetCenterText(CustomEntry entry)
		{
			if (entry.CenterText) {
				Control.TextAlignment = UITextAlignment.Center;
			} else {
				Control.TextAlignment = UITextAlignment.Left;
			}
		}

        private void SetAccessoryView(string imageName)
        {
            if (!string.IsNullOrEmpty (imageName)) {
                UIImageView im = new UIImageView (new UIImage (imageName));
                im.UserInteractionEnabled = true;

                UIGestureRecognizer gr = new UITapGestureRecognizer (OnTap) {
                    NumberOfTapsRequired = 1
                };
                im.AddGestureRecognizer (gr);

                _tf.RightViewMode = UITextFieldViewMode.Always;
                _tf.RightView = im;
            } else {
                _tf.RightViewMode = UITextFieldViewMode.Never;
                _tf.RightView = null;
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
                    (nint) Math.Round(image.Size.Width / 2.0), 
					(nint) Math.Round(image.Size.Height / 2.0));
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
            _tf.TextMargins = new UIEdgeInsets (top, left, bottom, right);
        }
	}
}

