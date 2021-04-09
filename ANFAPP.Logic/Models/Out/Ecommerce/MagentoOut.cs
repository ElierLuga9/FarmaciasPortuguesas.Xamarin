using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class MagentoOut
	{
		public string msg { get; set; }
		[JsonProperty("INFORESULT")]
		public int code { get; set; }

		[JsonProperty("FARMID")]
		public string FarmId { get; set; }
	}
}

