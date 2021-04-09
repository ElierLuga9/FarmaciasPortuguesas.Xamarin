using System;
using Xamarin.Forms;
using System.Globalization;
using ANFAPP.Logic;

namespace ANFAPP.Converters
{
	public class PharmacyIsInServiceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var code = value as string;
			return PharmacyUtils.GetIsInService (code);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

