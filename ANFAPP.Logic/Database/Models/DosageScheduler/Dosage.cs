using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
	[Table("Dosages")]
	public class Dosage : DosageBase
	{

		[PrimaryKey]
		public int Id { get; set; }
		public int ScheduleId { get; set; }
		public DateTime Date { get; set; }
		public TimeSpan Time { get; set; }
		public bool Done { get; set; }
		public double Quantity { get; set; }
		public string QuantityWithUnit { get; set; }

		[Ignore]
		public int Order { get; set; }

		[Ignore]
		public DateTime ReprDateTime { 
			get {
				if (Date == null || Date == DateTime.MinValue)
					return DateTime.MinValue;

				// XXX: the date is inserted as UTC but retrieved as Unspecified.
				var date = (Date.Kind == DateTimeKind.Unspecified) ? DateTime.SpecifyKind(Date, DateTimeKind.Utc) : Date;

				if (Time == null || Time.Ticks == 0)
					return date.ToLocalTime ();
				
				var dt =  new DateTime(
					date.Year, 
					date.Month,
					date.Day,
					Time.Hours,
					Time.Minutes,
					Time.Seconds,
					Time.Milliseconds,
					date.Kind);


				return dt.ToLocalTime ();
			}
		}

		public Dosage() { }
		public Dosage(Dosage dosage)
		{
			if (dosage == null) return;

			Id = dosage.Id;
			Date = dosage.Date;
			Time = dosage.Time;
			Done = dosage.Done;
			Quantity = dosage.Quantity;
			ScheduleId = dosage.ScheduleId;
		}

	}
}
