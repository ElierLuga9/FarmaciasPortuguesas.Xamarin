using System;
using SQLite;

namespace ANFAPP.Logic.Database.Models
{
	[Table("DosingSchedules")]
	public class DosingSchedule : DosageBase
	{

		[PrimaryKey]
		public int Id { get; set; }
		public string Notes { get; set; }
		public int MedicineId { get; set; }
		public string MedicineName { get; set; }
		public string Description { get; set; }
		public bool SentByPharmacy { get; set; }
		public string Quantity { get; set; }

		public bool HourInterval { get; set; }
		public int Interval { get; set; }
		public int Duration { get; set; }
		public int DurationTypeId { get; set; }

		[Ignore]
		public DateTime? NextDose { get; set; }

	}
}
