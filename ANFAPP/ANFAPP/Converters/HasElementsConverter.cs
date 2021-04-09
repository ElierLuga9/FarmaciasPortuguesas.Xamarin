using System;
using System.Globalization;
using System.Collections;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
    /// <summary>
    /// Used for Visibility data bindings
    /// </summary>
    public class HasElementsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null &&
                (
                    (value is ICollection && ((ICollection)value).Count > 0) ||
                    (value is string && ((string)value).Length > 0)
                );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}