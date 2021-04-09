using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Exceptions
{
	public class PharmacyChangedException : Exception 
	{

		public string NewFarmID { get; set; }

		public PharmacyChangedException(string newID)
		{
			NewFarmID = newID;
		}

	}
}
