using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	public class NewTomaToPlanIn
	{
		public string UserID { get; set; }
		public int PlanoTomasID { get; set; }
		public double Qtd { get; set; }
		public string DataHora { get; set; }
	}
}
