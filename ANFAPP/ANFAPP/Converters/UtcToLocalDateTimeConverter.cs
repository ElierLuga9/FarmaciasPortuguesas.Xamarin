using System;
using System.Globalization;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
	public class UtcToLocalDateTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc).ToLocalTime();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

