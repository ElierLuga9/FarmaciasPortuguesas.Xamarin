using System;
using System.Globalization;
using Xamarin.Forms;

namespace ANFAPP.Converters
{
	/// <summary>
	/// Returns the uppercase value.
	/// </summary>
	public class TitlecaseConverter : IValueConverter
	{
		// ToTitleCase is not available in the PCL:
		// http://forums.xamarin.com/discussion/27333/cultureinfo-currentculture-textinfo-totitlecase-is-gone
		private string ToTitleCase(string str)
		{
			string auxStr = str.ToLower();
			string[] auxArr = auxStr.Split(' ');
			string result = "";
			bool firstWord = true;
			foreach (string word in auxArr)
			{
				if (!firstWord)
					result += " ";
				else
					firstWord = false;

				result += word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length-1).ToLower();
			}

			return result;

		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is string)) return null;

			var str = value as string;
			return ToTitleCase (str);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

