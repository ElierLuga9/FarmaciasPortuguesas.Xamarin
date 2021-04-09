using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Helpers.Extensions;

namespace ANFAPP.Views.Common
{
	public class BindablePicker : Picker
	{

		#region Fields
		//BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(BindablePicker), null);
		public static readonly BindableProperty AutoSelectFirstProperty =
			//BindableProperty.Create<BindablePicker, bool>(p => p.AutoSelectFirst, false);
			BindableProperty.Create(nameof(AutoSelectFirst), typeof(bool), typeof(BindablePicker), false);
		//Bindable property for the items source
		public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create<BindablePicker, IList>(p => p.ItemsSource, null, propertyChanged: OnItemsSourcePropertyChanged);
		//	BindableProperty.Create(nameof(ItemsSource), typeof(string), typeof(BindablePicker), null);
		//Bindable property for the selected item
		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create<BindablePicker, object>(p => p.SelectedItem, null, BindingMode.TwoWay, propertyChanged: OnSelectedItemPropertyChanged);

		public static readonly BindableProperty CustomFontProperty =
		//	BindableProperty.Create<BindablePicker, string>(p => p.CustomFont, null);
			BindableProperty.Create(nameof(CustomFont), typeof(string), typeof(BindablePicker), string.Empty);
		public static readonly BindableProperty BackgroundResourceProperty =
		//	BindableProperty.Create<BindablePicker, string>(p => p.BackgroundResource, null);
			BindableProperty.Create(nameof(BackgroundResource), typeof(string), typeof(BindablePicker), string.Empty);
		public static readonly BindableProperty CustomTextColorProperty =
		//	BindableProperty.Create<BindablePicker, Color>(p => p.TextColor, Color.Black);
			BindableProperty.Create(nameof(CustomTextColor), typeof(Color), typeof(BindablePicker), Color.Black);
		public static readonly BindableProperty CustomFontSizeProperty =
		//	BindableProperty.Create<BindablePicker, int>(p => p.FontSize, 14);
			BindableProperty.Create(nameof(FontS), typeof(int), typeof(BindablePicker), 14);
		public static readonly BindableProperty CustomPaddingProperty =
		//	BindableProperty.Create<BindablePicker, string>(p => p.CustomPadding, null);
			BindableProperty.Create(nameof(CustomPadding), typeof(string), typeof(BindablePicker), string.Empty);
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the items source.
		/// </summary>
		/// <value>
		/// The items source.
		/// </value>
		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		/// Gets or sets the selected item.
		/// </summary>
		/// <value>
		/// The selected item.
		/// </value>
		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
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

		public bool AutoSelectFirst
		{
			get { return (bool)GetValue(AutoSelectFirstProperty); }
			set { SetValue(AutoSelectFirstProperty, value); }
		}

		public Color CustomTextColor
		{
			get { return (Color)GetValue(CustomTextColorProperty); }
			set { SetValue(CustomTextColorProperty, value); }
		}

		public int TopPadding { get; set; }
		public int LeftPadding { get; set; }
		public int RightPadding { get; set; }
		public int BottomPadding { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Called when [items source property changed].
		/// </summary>
		/// <param name="bindable">The bindable.</param>
		/// <param name="value">The value.</param>
		/// <param name="newValue">The new value.</param>
		private static void OnItemsSourcePropertyChanged(BindableObject bindable, IEnumerable value, IEnumerable newValue)
		{
			var picker = (BindablePicker)bindable;
			var notifyCollection = newValue as INotifyCollectionChanged;
			if (notifyCollection != null)
			{
				notifyCollection.CollectionChanged += (sender, args) =>
				{
					if (args.Action == NotifyCollectionChangedAction.Reset)
					{
						picker.Items.Clear();

						return;
					}

					if (args.NewItems != null)
					{
						foreach (var newItem in args.NewItems)
						{
							picker.Items.Add((newItem ?? "").ToString());
						}
					}
					if (args.OldItems != null)
					{
						foreach (var oldItem in args.OldItems)
						{
							picker.Items.Remove((oldItem ?? "").ToString());
						}
					}
				};
			}

			if (newValue == null)
				return;

			picker.Items.Clear();

			foreach (var item in newValue) 
			{
				picker.Items.Add((item ?? "").ToString());
			}

			if (picker.AutoSelectFirst && picker.ItemsSource != null && picker.ItemsSource.Count > 0)
			{
				picker.SelectedItem = picker.ItemsSource[0];
			}
		}

		/// <summary>
		/// Called when [selected item property changed].
		/// </summary>
		/// <param name="bindable">The bindable.</param>
		/// <param name="value">The value.</param>
		/// <param name="newValue">The new value.</param>
		private static void OnSelectedItemPropertyChanged(BindableObject bindable, object value, object newValue)
		{
			var picker = (BindablePicker)bindable;
			if (picker.ItemsSource != null && picker.SelectedIndex != picker.ItemsSource.IndexOf(picker.SelectedItem))
				picker.SelectedIndex = picker.ItemsSource.IndexOf(picker.SelectedItem);
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName.Equals("SelectedIndex"))
			{
				SelectedItem = ItemsSource != null && ItemsSource.Count > SelectedIndex && SelectedIndex > -1 ? ItemsSource[SelectedIndex] : null;
			}
		}

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
