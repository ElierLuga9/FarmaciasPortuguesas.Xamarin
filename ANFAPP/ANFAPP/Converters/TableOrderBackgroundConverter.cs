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
    /// Converter that returns true if the referenced object is null. </br>
    /// Used for Visibility data bindings
    /// </summary>
    public class TableOrderBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is int)) return null;

            int order = (int) value;
            return ((order % 2) != 0) ? ColorResources.ANFPaleGrey : Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
