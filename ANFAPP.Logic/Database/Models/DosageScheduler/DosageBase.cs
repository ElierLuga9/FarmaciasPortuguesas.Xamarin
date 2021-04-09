using System;

namespace ANFAPP.Logic.Database.Models
{
	public abstract class DosageBase
	{
		public DateTime LastUpdated { get; set; }
		public bool Updated { get; set; }

		public static DateTime NormalizedDate(DateTime date, TimeSpan time)
		{
			var normalized = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds, date.Kind);
			return normalized.ToUniversalTime ();
		}
	}
}
