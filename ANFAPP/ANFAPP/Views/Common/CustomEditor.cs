using System;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
    public class CustomEditor : Editor
    {
        #region Bindable Properties

        //public static readonly BindableProperty CustomFontProperty = BindableProperty.Create<CustomEditor, string>(p => p.CustomFont, null);
        public static readonly BindableProperty CustomFontProperty = BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomEditor), string.Empty);

		//public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create<CustomEditor, string>(p => p.BackgroundResource, null);
		public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create(nameof(BackgroundResource), typeof(string), typeof(CustomEditor), string.Empty);

		//public static readonly BindableProperty CustomFontSizeProperty = BindableProperty.Create<CustomEditor, int>(p => p.FontSize, 14);
		public static readonly BindableProperty CustomFontSizeProperty = BindableProperty.Create(nameof(FontS), typeof(int), typeof(CustomEditor), 14);

	//	public static readonly BindableProperty CustomBorderColorProperty = BindableProperty.Create<CustomEditor, Color>(p => p.CustomBorderColor, Color.Black);
		public static readonly BindableProperty CustomBorderColorProperty = BindableProperty.Create(nameof(CustomBorderColor), typeof(Color), typeof(CustomEditor), Color.Black);

		//public static readonly BindableProperty CustomPaddingProperty = BindableProperty.Create<CustomEntry, string>(p => p.CustomPadding, null);
		public static readonly BindableProperty CustomPaddingProperty = BindableProperty.Create(nameof(CustomPadding), typeof(string), typeof(CustomEditor), string.Empty);

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

        public int FontS
        {
            get { return (int)GetValue(CustomFontSizeProperty); }
            set { SetValue(CustomFontSizeProperty, value); }
        }
            
        public Color CustomBorderColor
        {
            get { return (Color)GetValue(CustomBorderColorProperty); }
            set { SetValue(CustomBorderColorProperty, value); }
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

		public int TopPadding { get; set; }
		public int LeftPadding { get; set; }
		public int RightPadding { get; set; }
		public int BottomPadding { get; set; }

        #endregion

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
    }
}
