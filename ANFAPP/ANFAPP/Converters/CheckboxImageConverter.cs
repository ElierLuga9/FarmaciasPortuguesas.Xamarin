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
    /// Converts a boolean into the corresponding checkbox icon, depending on its selection.
    /// </summary>
    public class CheckboxImageConverter : IValueConverter
    {
		private bool _flag;

		public CheckboxImageConverter(bool flag = false) : base ()
		{
			_flag = flag;
		}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool)) return null;

            if ((bool)value)
            {
                // Selected
				return _flag ? "bg_checkbox_selected_blue.png" : "bg_checkbox_selected.png";
            }
            else
            {
                // Unselected
                return "bg_checkbox.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
