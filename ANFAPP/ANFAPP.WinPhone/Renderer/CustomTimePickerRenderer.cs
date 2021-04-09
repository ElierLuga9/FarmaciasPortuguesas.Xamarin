using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using ANFAPP.WinPhone.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class CustomTimePickerRenderer : TimePickerRenderer
	{

		#region Rendering Functions

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			// Set custom styles, because Windows Phone has *unchangable* hidden margins inside the controls.
			if (Control != null) Control.Style = App.Current.Resources["CustomTimePickerStyle"] as System.Windows.Style;

			// Sets the Font Size
			SetFontSize();

			// Sets the Font Color
			SetFontColor();

			// Sets the Background manually, because.. Windows Phone
			SetBackground();

			// Sets the Padding
			SetPadding();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control != null && Element != null)
			{
				if (e.PropertyName.Equals("FontSize"))
				{
					SetFontSize();
				}
				else if (e.PropertyName.Equals("TextColor"))
				{
					SetFontColor();
				}
				else if (e.PropertyName.Equals("CustomPadding"))
				{
					SetPadding();
				}
			}
		}

		#endregion

		#region Setters

		protected void SetBackground()
		{
			if (Control != null)
			{
				Control.BorderThickness = new System.Windows.Thickness(1);
				Control.BorderBrush = new SolidColorBrush(LayoutUtils.ConvertStringToColor("A6A6A6"));
			}
		}

		protected void SetFontSize()
		{
			if (Element == null || Control == null) return;

			Control.FontSize = ((CustomTimePicker)Element).FontSize;
		}

		protected void SetFontColor()
		{
			if (Element == null || Control == null) return;

			var textColor = ((CustomTimePicker)Element).TextColor;

			// Text Color
			Control.Foreground = new SolidColorBrush(
				System.Windows.Media.Color.FromArgb(
					System.Convert.ToByte(255),
					System.Convert.ToByte((int) (textColor.R * 255)),
					System.Convert.ToByte((int) (textColor.G * 255)),
					System.Convert.ToByte((int) (textColor.B * 255))));
		}

		protected void SetPadding()
		{
			if (Element == null || Control == null) return;

			Control.Padding = new System.Windows.Thickness(
				((CustomTimePicker)Element).LeftPadding, ((CustomTimePicker)Element).TopPadding,
				((CustomTimePicker)Element).RightPadding, ((CustomTimePicker)Element).BottomPadding);
		}

		#endregion

	}
}
