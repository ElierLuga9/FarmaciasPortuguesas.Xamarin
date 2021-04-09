using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class PointsCatalogProdsOut : MagentoOut
	{
		[JsonProperty("PRODUCTSLIST")]
		public List<ProductOut> Products { get; set; }
	}
}

