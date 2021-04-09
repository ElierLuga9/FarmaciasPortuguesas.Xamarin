using System;
using SQLite;

namespace ANFAPP.Logic.Database.Models
{
	[Table("Localization")]
	public class Localization
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string IdDistrito { get; set; }
		public string DescDistrito { get; set; }
		public string IdConcelho { get; set; }
		public string DescConcelho { get; set; }
		public string IdFreguesia { get; set; }
		public string DescAbreviadoFreguesia { get; set; }
		public string DescCompletaFreguesia { get; set; }

	}
}

