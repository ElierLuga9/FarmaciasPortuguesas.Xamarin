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
        /// Converter that returns proper string when recieving int points. </br>
        /// Used for Text formating
        /// </summary>
        public class PointsToStringConverter : IValueConverter
        {
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return value.ToString()+" Pts";
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return null;
                }
        }
}
