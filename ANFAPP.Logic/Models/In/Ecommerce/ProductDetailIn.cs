using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ProductDetailIn
	{
		[JsonProperty("FARMID")]
		public string FarmId { get; set; }
		[JsonProperty("CNP")]
		public int CNP { get; set; }
	}
}

