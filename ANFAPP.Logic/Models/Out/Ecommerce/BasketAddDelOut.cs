using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class BasketAddDelOut : MagentoOut
	{
		[JsonProperty("QTD_PRODS_TOTAL")]
		public int TotalProducts { get; set; }

		[JsonProperty("QTD_PRODS_DISTINTOS")]
		public int DistinctProducts { get; set; }
	}
}

