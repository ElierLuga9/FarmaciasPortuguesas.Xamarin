using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	public class UpdateDosingSchedulesIn
	{

		public string UserID { get; set; }
		public List<DosingScheduleIn> Planos { get; set; }

		public class DosingScheduleIn
		{

			public int PlanoTomasID { get; set; }
			public string NomeUser { get; set; }
			public string Nota { get; set; }
		}

	}
}
