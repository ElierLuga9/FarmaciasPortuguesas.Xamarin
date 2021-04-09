using System;
using SQLite;


namespace ANFAPP.Logic.Database.Models
{
	[Table("ShiftPharmacy")]
	public class ShiftPharmacy
	{
		[PrimaryKey]
        public long Id { get; set; }

		public string IDPharmacy { get; set; }
		public string ShiftDate { get; set; }
	}
}

