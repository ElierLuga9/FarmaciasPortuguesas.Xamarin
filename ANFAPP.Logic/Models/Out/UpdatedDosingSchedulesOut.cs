using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
	public class UpdatedDosingSchedulesOut
	{

		public List<DosingScheduleOut> ListaPlanoTomas { get; set; }

		public class DosingScheduleOut
		{
			public int PlanoTomasID { get; set; }
			public int MedID { get; set; }
			public string NomeUser { get; set; }
			public string Nota { get; set; }
			public DateTime DataHoraUpdate { get; set; }
			public bool EnviadoFarmacia { get; set; }
			public int IntervaloQtd { get; set; }
			public int IntervaloEscala { get; set; }
			public int DuracaoQtd { get; set; }
			public int DuracaoEscala { get; set; }
			public string Qtd { get; set; }
		}

	}
}
