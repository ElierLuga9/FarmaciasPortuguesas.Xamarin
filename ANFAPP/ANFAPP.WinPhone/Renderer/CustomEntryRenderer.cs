using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using ANFAPP.WinPhone.Utils;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace ANFAPP.WinPhone.Renderer
{

	public class CustomEntryRenderer : EntryRenderer
	{

		#region Properties

		private CustomEntry ThisElement { get { return (Element != null) ? Element as CustomEntry : null; } }

		private PhoneTextBox _phoneTextBox = null;
		public PhoneTextBox PhoneTextBox
		{
			get { if (_phoneTextBox == null) InitializeNaviveControls(); return _phoneTextBox; }
			set { _phoneTextBox = value; }
		}

		private PasswordBox _passwordBox = null;
		public PasswordBox PasswordBox
		{
			get { if (_passwordBox == null) InitializeNaviveControls(); return _passwordBox; }
			set { _passwordBox = value; }
		}

		#endregion

		/// <summary>
		/// Initialize the native controls, because Xamarin... and Windows Phone..
		/// </summary>
		private void InitializeNaviveControls() 
		{
			if (this.Control != null)
			{
				/// Because Xamarin and Windows Phone are equally awesome, we have this!
				if (this.Control.Children != null && this.Control.Children.Count > 0)
				{
					foreach (var child in this.Control.Children)
					{
						if (child is PhoneTextBox)
						{
							PhoneTextBox = child as PhoneTextBox;
						}
						else if (child is PasswordBox)
						{
							PasswordBox = child as PasswordBox;
						}
					}
				}
			}
		}

		#region Rendering Functions

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			// Set custom styles, because Windows Phone has *unchangable* hidden margins inside the controls.
			if (PhoneTextBox != null) PhoneTextBox.Style = App.Current.Resources["CustomPhoneTextBoxStyle"] as System.Windows.Style;
			if (PasswordBox != null) PasswordBox.Style = App.Current.Resources["CustomPasswordBoxStyle"] as System.Windows.Style;

			// Sets the Font
			SetFontSize();
			
			// Sets the Background manually, because.. Windows Phone
			SetBackground();

			// Sets the Padding
			SetPadding();

			SetTextAlignment();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control != null && ThisElement != null)
			{
				if (e.PropertyName.Equals("FontSize"))
				{
					SetFontSize();
				}
				else if (e.PropertyName.Equals("CustomPadding"))
				{
					SetPadding();
				}
				else if (e.PropertyName.Equals("BackgroundResource"))
				{
					SetBackground();
				}
				else if (e.PropertyName.Equals("CenterText"))
				{
					SetTextAlignment();
				}
			}
		}

		#endregion

		#region Setters

		protected void SetBackground()
		{
			string resource = ThisElement.BackgroundResource;

			ResetBackground();
			if (string.IsNullOrEmpty(resource)) return;

			if (resource.StartsWith("bg_input"))
			{
				// Windows phone doesn't stretch backgrounds partially, so this will be done via code.
				if (PhoneTextBox != null)
				{
					PhoneTextBox.BorderThickness = new System.Windows.Thickness(1);
					PhoneTextBox.BorderBrush = new SolidColorBrush(LayoutUtils.ConvertStringToColor("A6A6A6"));
				}

				if (PasswordBox != null)
				{
					PasswordBox.BorderThickness = new System.Windows.Thickness(1);
					PasswordBox.BorderBrush = new SolidColorBrush(LayoutUtils.ConvertStringToColor("A6A6A6"));
				}
			}
			else
			{
				resource = "Resources/" + resource;
				if (!resource.Contains(".png")) resource += ".png";

				if (PhoneTextBox != null)
				{
					PhoneTextBox.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(resource, UriKind.RelativeOrAbsolute)) };
					
					
					//());
				}

				if (PasswordBox != null)
				{
					PasswordBox.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(resource, UriKind.RelativeOrAbsolute)) };
					//PasswordBox.Background = new BitmapImage(new Uri(resource, UriKind.RelativeOrAbsolute));
				}
			}
		}

		protected void ResetBackground()
		{
			if (PhoneTextBox != null) 
			{ 
				PhoneTextBox.Background = new SolidColorBrush(Colors.Transparent);
				PhoneTextBox.BorderBrush = new SolidColorBrush(Colors.Transparent);
				PhoneTextBox.BorderThickness = new System.Windows.Thickness(0);
			}

			if (PasswordBox != null)
			{
				PasswordBox.Background = new SolidColorBrush(Colors.Transparent);
				PasswordBox.BorderBrush = new SolidColorBrush(Colors.Transparent);
				PasswordBox.BorderThickness = new System.Windows.Thickness(0);
			}
		}

		protected void SetFontSize()
		{
			if (ThisElement == null || Control == null) return;

			if (PhoneTextBox != null) PhoneTextBox.FontSize = ThisElement.FontSize;
			if (PasswordBox != null) PasswordBox.FontSize = ThisElement.FontSize;
		}

		protected void SetPadding()
		{
			if (ThisElement == null || Control == null) return;

			if (PhoneTextBox != null) PhoneTextBox.Padding = 
				new System.Windows.Thickness(ThisElement.LeftPadding, ThisElement.TopPadding, ThisElement.RightPadding, ThisElement.BottomPadding);

			if (PasswordBox != null) PasswordBox.Padding = 
				new System.Windows.Thickness(ThisElement.LeftPadding, ThisElement.TopPadding, ThisElement.RightPadding, ThisElement.BottomPadding);
		}

		protected void SetTextAlignment() 
		{
			if (ThisElement == null || Control == null || PhoneTextBox == null) return;

			if (ThisElement.CenterText)
			{
				PhoneTextBox.TextAlignment = System.Windows.TextAlignment.Center;
			}
			else
			{
				PhoneTextBox.TextAlignment = System.Windows.TextAlignment.Left;
			}
		}

		#endregion

	}
}
