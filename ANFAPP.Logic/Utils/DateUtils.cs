using System;

namespace ANFAPP.Logic.Utils
{
	public static class DateUtils
	{

		public static bool IsUnderage(DateTime date) {

			var zeroTime = new DateTime(1, 1, 1);
			var currDate = DateTime.Today;

			return ((zeroTime + (currDate - date)).Year - 1) < 18; 
		}
	}
}