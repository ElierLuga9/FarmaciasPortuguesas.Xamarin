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
    /// Converts a boolean into the corresponding female icon, depending on its selection.
    /// </summary>
    public class FemaleSelectedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool)) return null;

            if ((bool)value)
            {
                // Selected
                return "ic_female_selected.png";
            }
            else
            {
                // Unselected
                return "ic_female.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
