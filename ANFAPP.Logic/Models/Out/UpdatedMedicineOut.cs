using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class UpdatedMedicineOut : WSTomasOut
	{

		public List<MedicineOut> ListaMedicamentosApp { get; set; }

		public class MedicineOut
		{
			public int MedID { get; set; }
			public string NomeMed { get; set; }
			public string CNP { get; set; }
			public string Nota { get; set; }
			public string DataHoraUpdate { get; set; }
			public bool AvisoMedicamento { get; set; }
			public string FormaFarmaceutica { get; set; }
            public string Dosagem { get; set; }
            public int Embalagem { get; set; }

			public bool IsPilula { get; set; }
		}
	}
}
