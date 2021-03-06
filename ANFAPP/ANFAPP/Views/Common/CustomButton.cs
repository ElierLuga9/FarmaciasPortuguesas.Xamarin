using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class CustomButton : Button
    {

		#region Enums

		public enum TextAlignments { Default, Left, Center, Right };

		#endregion

        #region Bindable Properties


		public static readonly BindableProperty CustomFontProperty = BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomButton), null);
		public static readonly BindableProperty AccessoryImageProperty = BindableProperty.Create(nameof(AccessoryImage), typeof(string), typeof(CustomButton), null);
		public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create(nameof(BackgroundResource), typeof(string), typeof(CustomButton), string.Empty);
		public static readonly BindableProperty TextAlignmentProperty = BindableProperty.Create(nameof(TextAlignment), typeof(TextAlignments), typeof(CustomButton), TextAlignments.Default);

        #endregion

        #region Bindable Objects

        public string CustomFont
        {
            get { return (string)GetValue(CustomFontProperty); }
            set 
            { 
                SetValue(CustomFontProperty, value);
                SetFontFamily(value);
            }
        }

		public string AccessoryImage
		{
			get { return (string)GetValue(AccessoryImageProperty); }
			set { SetValue(AccessoryImageProperty, value); }
		}

        public string BackgroundResource
        {
            get { return (string)GetValue(BackgroundResourceProperty); }
            set { SetValue(BackgroundResourceProperty, value); }
        }

		public TextAlignments TextAlignment
		{
			get { return (TextAlignments)GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
		}

        #endregion

        protected override void OnParentSet()
        {
            base.OnParentSet();

            // Initialize Font Family and Custom Margin
            SetFontFamily(CustomFont);
        }


        /// <summary>
        /// Sets the font family for iOS and WP. </br>
        /// Android is done by the corresponding renderer.
        /// </summary>
        /// <param name="fontName"></param>
        public void SetFontFamily(string fontName) {
            if (string.IsNullOrEmpty(fontName)) return;

            FontFamily = Device.OnPlatform(
                fontName.Replace(".ttf", string.Empty), 
                null,
				@"\Assets\Fonts\" + fontName + "#Roboto");
        }
    }
}
