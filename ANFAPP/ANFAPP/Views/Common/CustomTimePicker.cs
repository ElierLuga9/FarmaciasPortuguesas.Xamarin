using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class CustomTimePicker : TimePicker
    {

		#region Bindable Properties

		public static readonly BindableProperty CustomFontProperty = BindableProperty.Create<CustomTimePicker, string>(p => p.CustomFont, null);
		public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create<CustomTimePicker, string>(p => p.BackgroundResource, null);
		public static readonly BindableProperty CustomTextColorProperty = BindableProperty.Create<CustomTimePicker, Color>(p => p.TextColor, Color.Black);
		public static readonly BindableProperty CustomFontSizeProperty = BindableProperty.Create<CustomTimePicker, int>(p => p.FontSize, 14);
		public static readonly BindableProperty CustomPaddingProperty = BindableProperty.Create<CustomTimePicker, string>(p => p.CustomPadding, null);

		#endregion

        #region Bindable Objects

        public string CustomFont
        {
            get { return (string)GetValue(CustomFontProperty); }
            set { SetValue(CustomFontProperty, value); }
        }

        public string BackgroundResource
        {
            get { return (string)GetValue(BackgroundResourceProperty); }
            set { SetValue(BackgroundResourceProperty, value); }
        }

		public int FontSize
		{
			get { return (int)GetValue(CustomFontSizeProperty); }
			set { SetValue(CustomFontSizeProperty, value); }
		}

        public string CustomPadding
        {
            get { return (string)GetValue(CustomPaddingProperty); }
            set
            {
                SetValue(CustomPaddingProperty, value);
                InitCustomPadding(value);
            }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(CustomTextColorProperty); }
            set { SetValue(CustomTextColorProperty, value); }
        }

        public int TopPadding { get; set; }
        public int LeftPadding { get; set; }
        public int RightPadding { get; set; }
        public int BottomPadding { get; set; }

        #endregion

        #region Property Setters

        protected override void OnParentSet()
        {
            base.OnParentSet();

            // Initialize Custom Margin
            InitCustomPadding(CustomPadding);
        }

        /// <summary>
        /// Initializes the Custom Padding values </br>
        /// Valid Formats: "Left, Top, Right, Bottom" or "LeftRight, TopBottom".
        /// </summary>
        /// <param name="margin"></param>
        public void InitCustomPadding(string padding)
        {
            TopPadding = LeftPadding = RightPadding = BottomPadding = 0;
            if (string.IsNullOrEmpty(padding)) return;

            // Validate formats
            string[] paddings = padding.Split(',');
            if (paddings.Length != 2 && paddings.Length != 4) return;

            // Trim values && validate if digits
            for (int i = 0; i < paddings.Length; i++)
            {
                if (!string.IsNullOrEmpty(paddings[i]))
                    paddings[i] = paddings[i].Trim();

                // Validate if digit
                int aux = 0;
                if (!Int32.TryParse(paddings[i], out aux)) return;
            }

            if (paddings.Length == 2)
            {
                // Format: "LeftRight, TopBottom".
                LeftPadding = RightPadding = Int32.Parse(paddings[0]);
                TopPadding = BottomPadding = Int32.Parse(paddings[1]);
            }
            else if (paddings.Length == 4)
            {
                // Format: "Left, Top, Right, Bottom".
                LeftPadding = Int32.Parse(paddings[0]);
                TopPadding = Int32.Parse(paddings[1]);
                RightPadding = Int32.Parse(paddings[2]);
                BottomPadding = Int32.Parse(paddings[3]);
            }
        }

        #endregion

    }
}
