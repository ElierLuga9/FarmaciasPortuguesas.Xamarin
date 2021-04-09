using System;
using System.Globalization;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
	public class DoubleToCardImageConvert : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is double)) return false;
			if (((double)value) == 2)
			{
				return "cartao_vale_sauda_2";
			}
			else if (((double)value) == 5)
			{
				return "cartao_vale_sauda_5";
			}
			else if (((double)value) == 10)
			{
				return "cartao_vale_sauda_10";
			}
			else if (((double)value) == 20)
			{
				return "cartao_vale_sauda_20";
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
