using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class WishlistProductOut : ProductOut
	{
		[JsonProperty("ACTIVEPRINID")]
		public int? ActivePrinid { get; set; }
	}
}

