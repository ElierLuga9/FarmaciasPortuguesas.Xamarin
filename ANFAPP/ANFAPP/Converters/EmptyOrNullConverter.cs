using System;
using System.Globalization;
using System.Collections;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
    /// <summary>
    /// Used for Visibility data bindings
    /// </summary>
    public class EmptyOrNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null 
                || (value is ICollection && ((ICollection)value).Count == 0)
                || (value is string && string.IsNullOrWhiteSpace((string)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}