using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class FeaturedProdOut : MagentoOut
	{
		[JsonProperty("FEATUREDLIST")]
		public List<ProductOut> Products { get; set; }
	}
}

