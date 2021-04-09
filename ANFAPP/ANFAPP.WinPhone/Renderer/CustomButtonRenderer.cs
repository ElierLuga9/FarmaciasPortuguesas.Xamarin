using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class CustomButtonRenderer : ButtonRenderer
	{

		#region Properties

		private CustomButton ThisElement { get { return (Element != null) ? Element as CustomButton : null; } }

		#endregion

		#region Rendering Functions

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			// Set custom styles, because Windows Phone has *unchangable* hidden margins inside the controls.
			if (Control != null) Control.Style = App.Current.Resources["CustomButtonStyle"] as System.Windows.Style;

			SetAccessoryImage();

			SetTextAlignment();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control != null && ThisElement != null)
			{
				if (e.PropertyName.Equals("AccessoryImage"))
				{
					SetAccessoryImage();
				}
				else if (e.PropertyName.Equals("TextAlignment"))
				{
					SetTextAlignment();
				}
			}
		}

		#endregion

		#region Setters

		protected void SetTextAlignment()
		{
			if (ThisElement == null || Control == null || ThisElement.TextAlignment == null) return;

			switch (ThisElement.TextAlignment)
			{
				case CustomButton.TextAlignments.Left:
					Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
					break;
				case CustomButton.TextAlignments.Right:
					Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
					break;
				case CustomButton.TextAlignments.Center:
					Control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
					break;
			}
		}

		protected void SetAccessoryImage()
		{
			if (ThisElement == null || Control == null) return;

			
		}

		#endregion

	}
}
