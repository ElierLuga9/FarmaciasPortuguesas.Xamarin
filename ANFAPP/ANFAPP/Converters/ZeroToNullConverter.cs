using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
    /// <summary>
    /// Validates if the value is 0. If so, converts it to null for display purposes.
    /// "Cross-Platform"!!
    /// </summary>
    public class ZeroToNullConverter : IValueConverter
    { 
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Int
            if (value is int) {
                if ((int)value == 0)
                    return null;

                if (Device.OS != TargetPlatform.iOS)
                    return value;

                return value.ToString ();
            }

            // Double
            if (value is double) {
                var dbl = (double)value;

                if ((dbl <= double.Epsilon && dbl >= -double.Epsilon))
                    return null;

                if (Device.OS != TargetPlatform.iOS)
                    return value;

                NumberFormatInfo nfi = (NumberFormatInfo)culture.NumberFormat.Clone();
                nfi.NumberDecimalSeparator = App.DecimalSeparator ?? culture.NumberFormat.NumberDecimalSeparator;
                return System.Convert.ToString (value, nfi);
            }

            // Float
            if (value is float) {
                var flt = (float)value;

                if ((flt <= float.Epsilon && flt >= -float.Epsilon))
                    return null;

                if (Device.OS != TargetPlatform.iOS)
                    return value;

                NumberFormatInfo nfi = (NumberFormatInfo)culture.NumberFormat.Clone();
                nfi.NumberDecimalSeparator = App.DecimalSeparator ?? culture.NumberFormat.NumberDecimalSeparator;
                return System.Convert.ToString (value, nfi);
            }

            return null;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Device.OS == TargetPlatform.iOS) {
                var str = value as string;
                if (string.IsNullOrEmpty (str))
                    return 0;

                double doubleValue = 0.0;

                NumberFormatInfo nfi = (NumberFormatInfo)culture.NumberFormat.Clone();
                nfi.NumberDecimalSeparator = App.DecimalSeparator ?? culture.NumberFormat.NumberDecimalSeparator;
                Double.TryParse (str, NumberStyles.Any, nfi, out doubleValue);

                return doubleValue;
			}
			else if (Device.OS == TargetPlatform.WinPhone)
			{
				double val = 0.0;

				if (string.IsNullOrEmpty(value as string)) return val;
				string valueStr = value as string;

				// Xamarin.WPhone incorrectly ignores the keyboard culture so... It's Hammer Time!
				NumberFormatInfo nfi = (NumberFormatInfo)culture.NumberFormat.Clone();
				var cultureSeparator = culture.NumberFormat.NumberDecimalSeparator;

				if (valueStr.Contains(",") && !string.Equals(",", cultureSeparator)) valueStr = valueStr.Replace(",", cultureSeparator);

				value = (value as string).Replace(",", ".");
				//Double.TryParse(value as string, out val);

				return value;
			} else {
                return value;
            }
        }
    }
}
