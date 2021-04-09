using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class FeaturedProdIn
	{
		[JsonProperty("FARMID")]
		public string FarmId { get; set; }

		[JsonProperty("QTY")]
		public int Quantity { get; set; }

		[JsonProperty("CATALOG")]
		public bool Catalog { get; set; }

		[JsonProperty("ORDERTYPE")]
		public long? OrderFilter { get; set; }

		[JsonProperty(PropertyName="RINICIAL")]
		public int? PageStart { get; set; }
	}
}

