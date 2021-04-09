using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class CustomLabel : Label
    {

        #region Bindable Properties
		//	BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomButton), null);
        public static readonly BindableProperty CustomFontProperty = BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomLabel), null);
		public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create(nameof(BackgroundResource), typeof(string), typeof(CustomLabel), null);
		public static readonly BindableProperty CustomMarginProperty = BindableProperty.Create(nameof(CustomMargin), typeof(string), typeof(CustomLabel), null);
		public static readonly BindableProperty UnderlineProperty = BindableProperty.Create(nameof(IsUnderline), typeof(bool), typeof(CustomLabel), false);

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

        public string BackgroundResource
        {
            get { return (string)GetValue(BackgroundResourceProperty); }
            set { SetValue(BackgroundResourceProperty, value); }
        }

        public string CustomMargin
        {
            get { return (string)GetValue(CustomMarginProperty); }
            set 
            { 
                SetValue(CustomMarginProperty, value);
                InitCustomMargin(value);
            }
        }

        public bool IsUnderline
        {
            get { return (bool)GetValue(UnderlineProperty); }
            set
            {
                SetValue(UnderlineProperty, value);
            }
        }

        public int TopMargin { get; set; }
        public int LeftMargin { get; set; }
        public int RightMargin { get; set; }
        public int BottomMargin { get; set; }

        #endregion

        protected override void OnParentSet()
        {
            base.OnParentSet();

            // Initialize Font Family and Custom Margin
            SetFontFamily(CustomFont);
            InitCustomMargin(CustomMargin);
        }

        /// <summary>
        /// Sets the font family for iOS and WP. </br>
        /// Android is done by the corresponding renderer.
        /// </summary>
        /// <param name="fontName"></param>
        public void SetFontFamily(string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return;

            FontFamily = Device.OnPlatform(
                fontName.Replace(".ttf", string.Empty),
                null,
                @"\Assets\Fonts\" + fontName + "#Roboto");
        }

        /// <summary>
        /// Initializes the Custom Margin values </br>
        /// Valid Formats: "Left, Top, Right, Bottom" or "LeftRight, TopBottom".
        /// </summary>
        /// <param name="margin"></param>
        public void InitCustomMargin(string margin)
        {
            TopMargin = LeftMargin = RightMargin = BottomMargin = 0;
            if (string.IsNullOrEmpty(margin)) return;

            // Validate formats
            string[] margins = margin.Split(',');
            if (margins.Length != 2 && margins.Length != 4) return;

            // Trim values && validate if digits
            for (int i = 0; i < margins.Length; i++)
            {
                if (!string.IsNullOrEmpty(margins[i])) 
                    margins[i] = margins[i].Trim();

                // Validate if digit
                int aux = 0;
                if (!Int32.TryParse(margins[i], out aux)) return;
            }

            if (margins.Length == 2)
            {
                // Format: "LeftRight, TopBottom".
                LeftMargin = RightMargin = Int32.Parse(margins[0]);
                TopMargin = BottomMargin = Int32.Parse(margins[1]);
            }
            else if (margins.Length == 4)
            {
                // Format: "Left, Top, Right, Bottom".
                LeftMargin = Int32.Parse(margins[0]);
                TopMargin = Int32.Parse(margins[1]); 
                RightMargin = Int32.Parse(margins[2]);
                BottomMargin = Int32.Parse(margins[3]);
            }
        }
    }
}
