using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Pages.BiometricData;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class BiometricGauge : ContentView
	{

        #region Bindable Properties

        public static readonly BindableProperty TitleProperty = BindableProperty.Create<BiometricGauge, string>(p => p.Title, null);
        public static readonly BindableProperty BiometricValueProperty = BindableProperty.Create<BiometricGauge, string>(p => p.BiometricValue, null);
        public static readonly BindableProperty BiometricUnitProperty = BindableProperty.Create<BiometricGauge, string>(p => p.BiometricUnit, null);
        public static readonly BindableProperty BiometricEvaluatorProperty = BindableProperty.Create<BiometricGauge, IBiometricEvaluator>(p => p.BiometricEvaluator, null);
        public static readonly BindableProperty RoundingFlagProperty = BindableProperty.Create<BiometricGauge, bool>(p => p.RoundingFlag, false);

        #endregion

        #region Bindable Objects

        public bool RoundingFlag 
        { 
            get { return (bool)GetValue(RoundingFlagProperty); }
            set { SetValue(RoundingFlagProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string BiometricValue
        {
            get { 
                var val = (string)GetValue(BiometricValueProperty);
                if (string.IsNullOrEmpty(val)) return null;

                double value = 0.0;
				if (double.TryParse(val, out value)) 
                {
                    if (this.RoundingFlag) {
						return Math.Round(value, 1) + string.Empty;
						//return string.Format("{0:0.#}", value);
                    } else {
						return Math.Round(value, 2) + string.Empty;
						//return string.Format("{0:0.##}", value);
                    }
                }

                return val;
            }
            set { SetValue(BiometricValueProperty, value); }
        }

        public string BiometricUnit
        {
            get { return (string)GetValue(BiometricUnitProperty); }
            set { SetValue(BiometricUnitProperty, value); }
        }

        public IBiometricEvaluator BiometricEvaluator
        {
            get { return (IBiometricEvaluator)GetValue(BiometricEvaluatorProperty); }
            set { SetValue(BiometricEvaluatorProperty, value); }
        }

        #endregion

        public BiometricGauge()
		{
			InitializeComponent ();

            TitleLabel.BindingContext = this;
            TitleContainer.BindingContext = this;

            BiometricGraph.BindingContext = this;
            BiometricValueLabel.BindingContext = this;
            BiometricUnitLabel.BindingContext = this;

            WarningTitle.BindingContext = this;
            WarningMessage.BindingContext = this;
        }

        #region Layout Methods

        /// <summary>
        /// Called when a proprety is changed. </br>
        /// Used to update the biometric value text size when needed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(propertyName) && propertyName.Equals("BiometricValue"))
            {
                UpdateBiometricValueFont();
            }
        }


        /// <summary>
        /// Updates the Biometric Label's font size, depending on the length
        /// </summary>
        /// <param name="value"></param>
        private void UpdateBiometricValueFont()
        {
			if (string.IsNullOrEmpty(BiometricValue) || BiometricValue.Length <= 3)
            {
                GraphContainer.Spacing = -6;
                BiometricValueLabel.FontSize = 60;
            }
			else
            {
                GraphContainer.Spacing = -4;
                BiometricValueLabel.FontSize = 35;
            }
        }

        #endregion
    }
}

