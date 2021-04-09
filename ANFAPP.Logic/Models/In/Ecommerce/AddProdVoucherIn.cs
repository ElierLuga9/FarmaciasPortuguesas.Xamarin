using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class AddProdVoucherIn
	{
		[JsonProperty("FARMID")]
		public string FarmId { get; set; }

		[JsonProperty("VOUID")]
		public string idVale { get; set; }

		[JsonProperty("CNP")]
		public int CNP { get; set; }



	}
}

