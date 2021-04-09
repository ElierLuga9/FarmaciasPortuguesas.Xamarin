using System;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class AddProductVoucherOut
	{
		[JsonProperty("INFORESULT")]
		public int result { get; set; }

		[JsonProperty("MSG")]
		public string msg { get; set; }
	}
}

