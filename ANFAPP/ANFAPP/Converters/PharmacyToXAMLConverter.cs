using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic;

namespace ANFAPP.Converters
{
	public class PharmacyToXAMLConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Pharmacy pharmacy = (Pharmacy)value;
			var imageName = PharmacyUtils.IconForPharmacy (pharmacy);

			return imageName ?? "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

