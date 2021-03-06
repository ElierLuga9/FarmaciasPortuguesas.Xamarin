using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
	/// <summary>
	/// Entry doesn't support custom fonts in Xamarin.Forms. </br>
	/// Font Family must be changed in the corresponding renderers.
	/// </summary>
	public class CustomEntry : Entry
	{
		public event EventHandler<ElementEventArgs> AccessoryTapped;
		//	BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomButton), null);
		#region Bindable Properties
		public static readonly BindableProperty CustomFontProperty = BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(CustomEntry), string.Empty);
		//public static readonly BindableProperty CustomFontProperty = BindableProperty.Create<CustomEntry, string>(p => p.CustomFont, null);
		public static readonly BindableProperty BackgroundResourceProperty = BindableProperty.Create(nameof(BackgroundResource), typeof(string), typeof(CustomEntry), string.Empty);
		public static readonly BindableProperty CustomFontSizeProperty = BindableProperty.Create(nameof(FontS), typeof(int), typeof(CustomEntry), 14);
		public static readonly BindableProperty CustomPaddingProperty = BindableProperty.Create(nameof(CustomPadding), typeof(string), typeof(CustomEntry), string.Empty);
		public static readonly BindableProperty AllowDecimalsProperty = BindableProperty.Create(nameof(AllowDecimals), typeof(bool), typeof(CustomEntry), true);
		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(CustomEntry), -1);
		public static readonly BindableProperty AllowedCharactersProperty = BindableProperty.Create(nameof(AllowedCharacters), typeof(string), typeof(CustomEntry), null);

		public static readonly BindableProperty AccessoryImageProperty = BindableProperty.Create(nameof(AccessoryImage), typeof(string), typeof(CustomEntry), string.Empty);

		public static readonly BindableProperty CenterTextProperty = BindableProperty.Create(nameof(CenterTextProperty), typeof(bool), typeof(CustomEntry), false);
		#endregion

		#region Bindable Objects

		public string AllowedCharacters
		{
			get { return (string)GetValue(AllowedCharactersProperty); }
			set { SetValue(AllowedCharactersProperty, value); }
		}

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

		public string CustomPadding
		{
			get { return (string)GetValue(CustomPaddingProperty); }
			set
			{
				SetValue(CustomPaddingProperty, value);
				InitCustomPadding(value);
			}
		}

		public string AccessoryImage
		{
			get { return (string)GetValue(AccessoryImageProperty); }
			set { SetValue(AccessoryImageProperty, value); }
		}

		public bool AllowDecimals
		{
			get { return (bool)GetValue(AllowDecimalsProperty); }
			set { SetValue(AllowDecimalsProperty, value); }
		}

		public bool CenterText
		{
			get { return (bool)GetValue(CenterTextProperty); }
			set { SetValue(CenterTextProperty, value); }
		}

		public int MaxLength
		{
			get { return (int)GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}

		public int TopPadding { get; set; }
		public int LeftPadding { get; set; }
		public int RightPadding { get; set; }
		public int BottomPadding { get; set; }


		#endregion

		public CustomEntry()
		{
			this.TextChanged += EnforceMaxLength;
		}

		public void OnAccessoryTapped()
		{
			if (AccessoryTapped != null) AccessoryTapped(this, null);
		}

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

		/// <summary>
		/// Enforce the max length, if exists, and the decimals rule.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void EnforceMaxLength(object sender, TextChangedEventArgs args)
		{
			if (string.IsNullOrEmpty(this.Text)) return;

			var theText = this.Text;

			if (null != AllowedCharacters)
			{
				string tmp = args.NewTextValue;
				theText = Regex.Replace(tmp, string.Format("[^{0}]*", AllowedCharacters), ""); ;
			}

			// Enforce max length
			if (MaxLength != -1 && theText.Length > MaxLength)
			{
				theText = theText.Substring(0, MaxLength);
			}

			// Enforce decimals rule
			string decimalSeparator = App.DecimalSeparator ?? CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			//if (!AllowDecimals && theText.EndsWith (decimalSeparator)) {
			//	theText = theText.Substring (theText.Length - 1);
			//}
			if (!AllowDecimals)
			{
				theText = theText.Replace(".", string.Empty).Replace(",", string.Empty);
			}


			if (!string.Equals(this.Text, theText))
			{
				this.TextChanged -= EnforceMaxLength;
				this.Text = theText;
				this.TextChanged += EnforceMaxLength;
			}
		}

		#endregion
	}
}
