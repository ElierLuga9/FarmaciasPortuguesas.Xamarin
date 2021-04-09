using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	public class UpdateMedsIn
	{

		public string UserID { get; set; }
		public List<MedIn> Medicamentos { get; set; }

		public class MedIn
		{

			public int MedID { get; set; }
			public string Nota { get; set; }
			public string CNP { get; set; }
			public string FormaFarmaceutica { get; set; }
			public string DataHoraUpdate { get; set; }
			public bool AvisoMedicamento { get; set; }
			//public bool Ativo { get; set; }
		}

	}
}
