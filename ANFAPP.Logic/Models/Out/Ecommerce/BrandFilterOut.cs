using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class BrandFilterOut
	{
		[JsonProperty("BRANDID")]
		public long ID { get; set; }

		[JsonProperty("BRANDNAME")]
		public string Name { get; set; }
	}
}
