using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class ProductVoucherIn
	{
		[JsonProperty("FARMID")]
		public string FarmId { get; set; }

		[JsonProperty("VOUID")]
		public string idVale { get; set; }

		[JsonProperty("QTY")]
		public int Quantity { get; set; }


		[JsonProperty("RINICIAL")]
		public int registoInicial { get; set; }
	}
}

