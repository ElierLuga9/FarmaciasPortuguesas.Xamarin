using System;
using SQLite;

namespace ANFAPP.Logic.Database.Models
{
	[Table("ScheduleType")]
	public class ScheduleType
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Code { get; set; }
		public string Description { get; set; }
	}
}

