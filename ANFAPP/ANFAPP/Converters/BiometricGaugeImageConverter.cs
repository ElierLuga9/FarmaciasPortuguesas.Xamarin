using ANFAPP.Logic.Resources;
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
    /// Converts a Color into the corresponding gauge image.
    /// </summary>
    public class BiometricGaugeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Color)) return null;

            var color = (Color) value;

            if (color.Equals(ColorResources.ANFGreen))
            {
                return "bg_biometric_gauge_green.png";
            } 
            else if (color.Equals(ColorResources.ANFYellow))
            {
                return "bg_biometric_gauge_yellow.png";
            }
            else if (color.Equals(ColorResources.ANFDarkYellow))
            {
                return "bg_biometric_gauge_darkyellow.png";
            }
            else if (color.Equals(ColorResources.ANFOrange))
            {
                return "bg_biometric_gauge_orange.png";
            }
            else if (color.Equals(ColorResources.ANFRed))
            {
                return "bg_biometric_gauge_red.png";
            }

            return "bg_biometric_gauge_grey.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
