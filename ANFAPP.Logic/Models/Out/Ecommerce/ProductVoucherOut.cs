using System;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ProductVoucherOut
	{
		[JsonProperty("RESULTS")]
		public int numeroDeProdutos { get; set; }

		[JsonProperty("FARMID")]
		public string farmID { get; set; }

		[JsonProperty("PRODUCTS")]
		public List<ProductOut> Products { get; set; }


		[JsonProperty("INFORESULT")]
		public int infoResult { get; set; }
	}
}

