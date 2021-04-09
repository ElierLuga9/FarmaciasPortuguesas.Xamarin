using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public enum BannerFilter {
		Undefined = 0,
		Product = 1,
		Category = 2,
		Brand = 3,
	}

	public class GetBannersIn
	{
		[JsonProperty("ZONAID")]
		public int ZoneId { get; set; }

		[JsonProperty("QTD")]
		public int Quantity { get; set; }

		[JsonProperty("IMAGEH")]
		public int? Height { get; set; }

		[JsonProperty("IMAGEW")]
		public int Width { get; set; }

		[JsonProperty("FILTERID")]
		public int FilterId { get; set; }

		[JsonProperty("FILTERTYPE")]
		public int FilterType { get; set; }
	}
}

