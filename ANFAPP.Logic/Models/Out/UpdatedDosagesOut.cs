using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	public class UpdatedDosagesOut
	{
		public List<DosageOut> ListaTomas { get; set; }

		public class DosageOut
		{
			public int TomaID { get; set; }
			public int PlanoID { get; set; }
			// Don't trust Newtonsoft.Json to deserialize dates.
			public string DataHora { get; set; }
			public string DataHoraUpdate { get; set; }
			public string NomeUser { get; set; }
			public string Nota { get; set; }
			public double Qtd { get; set; }
			public bool FlagTomado { get; set; }
		}


	}
}
