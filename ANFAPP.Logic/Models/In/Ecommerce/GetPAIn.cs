using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class GetPAIn
	{
		[JsonProperty(PropertyName="ACTIVEPRINID", Required=Required.Always)]
		public int ActivePrincipleId { get; set; }

		[JsonProperty(PropertyName="FARMID", Required=Required.Always)]
		public string FarmId { get; set; }

		[JsonProperty("DOSID")]
		public int? DosageId { get; set; }
	}
}

