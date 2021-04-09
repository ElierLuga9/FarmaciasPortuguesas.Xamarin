using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
    public class MedicineSearchOut
    {
        public List<MedicineInOut> Drugs;

        /// <summary>
        /// A Medicine retrieved from the product search web service, with 
        /// additional fields for user data that might be submitted to the 
        /// scheduler web service. 
        /// </summary>
		public class MedicineInOut
		{
			public string CNP { get; set; }
			public string Name { get; set; }
			public string Dosage { get; set; }
			public string Qty { get; set; }
			public string Form { get; set; }
            public string Notes { get; set; }
            public bool WarnUser { get; set; }

			public string Description
			{
				get
				{
					return string.Format("{0} - {1}", Dosage, Qty);
				}
			}
		}
    }

    
}
