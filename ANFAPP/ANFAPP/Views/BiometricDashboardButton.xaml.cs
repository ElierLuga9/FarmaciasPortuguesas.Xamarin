using ANFAPP.Pages.BiometricData;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class BiometricDashboardButton : ContentView
	{

        #region Bindable Properties

        public static readonly BindableProperty IconProperty = //BindableProperty.Create<BiometricDashboardButton, string>(p => p.Icon, null);
																BindableProperty.Create(nameof(Icon), typeof(string), typeof(BiometricDashboardButton), string.Empty);
        public static readonly BindableProperty TitleProperty =// BindableProperty.Create<BiometricDashboardButton, string>(p => p.Title, null);
																BindableProperty.Create(nameof(Title), typeof(string), typeof(BiometricDashboardButton), null);
        public static readonly BindableProperty BiometricValueProperty =// BindableProperty.Create<BiometricDashboardButton, string>(p => p.BiometricValue, null);
																		BindableProperty.Create(nameof(BiometricValue), typeof(string), typeof(BiometricDashboardButton), null);
        public static readonly BindableProperty BiometricUnitProperty =// BindableProperty.Create<BiometricDashboardButton, string>(p => p.BiometricUnit, null);
																		BindableProperty.Create(nameof(BiometricUnit), typeof(string), typeof(BiometricDashboardButton), null);
        public static readonly BindableProperty ButtonColorProperty = //BindableProperty.Create<BiometricDashboardButton, Color>(p => p.ButtonColor, Color.Transparent);
																		BindableProperty.Create(nameof(ButtonColor), typeof(Color), typeof(BiometricDashboardButton), Color.Transparent);
        #endregion

        #region Bindable Objects

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string BiometricValue
        {
            get { return (string)GetValue(BiometricValueProperty); }
            set { SetValue(BiometricValueProperty, value); }
        }

        public string BiometricUnit
        {
            get { return (string)GetValue(BiometricUnitProperty); }
            set { SetValue(BiometricUnitProperty, value); }
        }

        public Color ButtonColor
        {
            get { return (Color)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }

        private EventHandler _buttonClicked;
        public EventHandler ButtonClicked
        {
            get { return _buttonClicked; }
            set 
            {
                _buttonClicked = value;
                //SetButtonEventHandler(value);
            }
        }

        #endregion

		public BiometricDashboardButton ()
		{
			InitializeComponent ();

            this.BindingContext = this;
            CategoryIcon.BindingContext = this;
            //CategoryButton.BindingContext = this;
            CategoryNameLabel.BindingContext = this;
            BiometricValueLabel.BindingContext = this;
            BiometricUnitLabel.BindingContext = this;
		}

        //private void SetButtonEventHandler(EventHandler handler) {
        //    if (ButtonClicked != null) CategoryButton.Clicked += handler;
        //}

        void OnButtonClicked(object sender, EventArgs args)
        {
            if (ButtonClicked != null) ButtonClicked(sender, args);
        }

	}
}

