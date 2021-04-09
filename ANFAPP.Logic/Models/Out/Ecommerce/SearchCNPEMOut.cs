using System;
using Newtonsoft.Json;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class SearchCNPEMOut : MagentoOut
	{
		
		#region Properties

		[JsonProperty("TIPO")]
		public string Type { get; set; }

		[JsonProperty("INFO")]
		public SearchItemCNPOut Item { get; set; }

		#endregion

	}

	public class SearchItemCNPOut
	{
		#region Properties

		[JsonProperty("NAME")]
		public string Name { get; set; }

		[JsonProperty("ACTIVEPRIN")]
		public string ActivePrinciple { get; set; }

		[JsonProperty("PRINC")]
		public string CNPPrinciple { get; set; }

		[JsonProperty("DOSE")]
		public string Dosage { get; set; }

		[JsonProperty("PACK")]
		public Nullable<int> Pack { get; set; }

		[JsonProperty("FF")]
		public string FF { get; set; }

		[JsonIgnore]
		public string PackDescription
		{
			get { return Dosage + " - " + Pack + " " + FF; }
		}

		#endregion
	}

}

