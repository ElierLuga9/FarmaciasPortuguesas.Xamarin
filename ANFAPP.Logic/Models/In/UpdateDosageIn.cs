using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	public class UpdateDosageIn
	{

		public string UserID { get; set; }
		public List<DosageIn> Tomas { get; set; }

		public class DosageIn
		{
			public int TomaID { get; set; }
			public string DataHora { get; set; }
			public double Qtd { get; set; }
			public bool FlagTomado { get; set; }
		}

	}
}
