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
    /// Converts a boolean into the correct color to display the current selection. </br>
    /// Used for Visibility data bindings
    /// </summary>
    public class DocumentTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool)) return null;

            if ((bool)value)
            {
                // Selected
                return ColorResources.ANFGreen;
            }
            else
            {
                // Unselected
                return ColorResources.TextColorGrey;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
