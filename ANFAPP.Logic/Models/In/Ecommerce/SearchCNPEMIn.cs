using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class SearchCNPEMIn
	{
		[JsonProperty(PropertyName="FARMID", Required=Required.Always)]
		public string FarmId { get; set; }

		[JsonProperty(PropertyName="CODE", Required=Required.Always)]
		public int Code { get; set; }
	}
}
