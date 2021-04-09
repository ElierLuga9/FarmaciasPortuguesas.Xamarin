using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Util;

namespace ANFAPP.Droid.Utils
{
	public static class DateUtils
	{

		private static readonly string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

		#region Converters

		public static DateTime DateToDateTime(Date date)
		{
			if (date == null) return DateTime.Now;

			SimpleDateFormat dateFormat = new SimpleDateFormat(DATE_FORMAT);
			return DateTime.ParseExact(dateFormat.Format(date), DATE_FORMAT, null);
		}

		public static Date DateTimeToDate(DateTime date)
		{
			var dateStr = date.ToString(DATE_FORMAT);
			SimpleDateFormat dateFormat = new SimpleDateFormat(DATE_FORMAT);
			return dateFormat.Parse(dateStr);
		}

		#endregion

	}
}