using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using ANFAPP.iOS.Renderer;
using ANFAPP.Views.Common;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace ANFAPP.iOS.Renderer
{
	public class CustomButtonRenderer : ButtonRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (null != Control)
			{
				Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
				Control.TitleLabel.Lines = 0;
				Control.AdjustsImageWhenHighlighted = false;
				Control.AdjustsImageWhenDisabled = false;
				Control.TintAdjustmentMode = UIViewTintAdjustmentMode.Normal;

                if (null != Element) {
                    SetAccessoryImage (((CustomButton)this.Element).AccessoryImage);
					SetTextAlignment(((CustomButton)this.Element).TextAlignment);

					if (Element.IsEnabled) {
						Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Normal);
					} else {
						Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Disabled);
					}
                }
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName.Equals (Button.TextColorProperty.PropertyName)) {
				if (Element.IsEnabled) {
					Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Normal);
				} else {
					Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Disabled);
				}
			} 
			else if (e.PropertyName.Equals (Button.IsEnabledProperty.PropertyName)) 
			{
				if (Element.IsEnabled) {
					Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Normal);
				} else {
					Control.SetTitleColor (Element.TextColor.ToUIColor (), UIControlState.Disabled);
				}
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

		/// <summary>
		/// Sets the accessory image to the left of the text.
		/// </summary>
		/// <param name="imageName"></param>
		protected void SetAccessoryImage(string imageName)
		{
			if (string.IsNullOrEmpty(imageName)) return;

			Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            var img = new UIImage (imageName);
            // Pad the button.
			Control.ContentEdgeInsets = new UIEdgeInsets(0, 10, 0, 10);
			Control.SetImage(img, UIControlState.Normal);
			Control.ImageEdgeInsets = new UIEdgeInsets (0, -4, 0, 0);
		}

		/// <summary>
		/// Sets the Text Alignment
		/// </summary>
		/// <param name="alignment"></param>
		protected void SetTextAlignment(CustomButton.TextAlignments alignment)
		{
			if (alignment == CustomButton.TextAlignments.Default) return;

			const float padding = 10.0f;
			switch (alignment) 
			{
				case CustomButton.TextAlignments.Left:
					Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
					Control.TitleEdgeInsets = new UIEdgeInsets(0, padding, 0, 0);
					break;
				case CustomButton.TextAlignments.Center:
					Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
					break;
				case CustomButton.TextAlignments.Right:
					Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
					Control.TitleEdgeInsets = new UIEdgeInsets(0, 0, 0, padding);
					break;
			}
		}
	}
}