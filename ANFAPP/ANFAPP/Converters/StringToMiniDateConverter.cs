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
    public class StringToMiniDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is DateTime)) return null;

			DateTime dt = (DateTime)value;
			return dt.ToLocalTime().ToString("dd/MM/yy HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
