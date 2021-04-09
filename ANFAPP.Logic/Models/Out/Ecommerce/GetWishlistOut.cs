using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class GetWishlistOut : MagentoOut
	{
		[JsonProperty("FAVLIST")]
		public List<WishlistProductOut> Products { get; set; }
	}
}

