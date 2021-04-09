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
	///
    /// </summary>
	public class CatalogCategoryColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null || !(value is Nullable<int>)) return ColorResources.ANFDarkOrange;

			var catId = value as Nullable<int>;

			// DUMMY SETTINGS
			// TODO: Change rules once they are defined..
			switch (catId.Value)
			{
				case 1: return ColorResources.ANFCatalogYellow;
				case 2: return ColorResources.ANFCatalogPurple;
				case 3: return ColorResources.ANFDarkBlue;
				default: return ColorResources.ANFDarkOrange;
			}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
