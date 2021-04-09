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

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class CustomEditorRenderer : EditorRenderer
	{

		#region Rendering Functions

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			// Set custom styles, because Windows Phone has *unchangable* hidden margins inside the controls.
			if (Control != null) Control.Style = App.Current.Resources["CustomTextBoxStyle"] as System.Windows.Style;

			// Sets the Font
			SetFontSize();

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
			if (Control == null) return;

			Control.BorderThickness = new System.Windows.Thickness(1);
			Control.BorderBrush = new SolidColorBrush(LayoutUtils.ConvertStringToColor("A6A6A6"));
		}

		protected void SetFontSize()
		{
			if (Element == null || Control == null) return;

			Control.FontSize = ((CustomEditor)Element).FontSize;
		}

		protected void SetPadding()
		{
			if (Element == null || Control == null) return;

			Control.Padding =
				new System.Windows.Thickness(
					((CustomEditor)Element).LeftPadding, ((CustomEditor)Element).TopPadding,
					((CustomEditor)Element).RightPadding, ((CustomEditor)Element).BottomPadding);
		}

		#endregion

	}
}
