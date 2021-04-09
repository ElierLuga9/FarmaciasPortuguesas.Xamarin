using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class BasketProductIn
	{

		[JsonProperty("CNP")]
		public int? CNP { get; set; }

		[JsonProperty("CNPEM")]
		public int? CNPEM { get; set ; }

		[JsonProperty("VOUCHERID")]
		public int? VoucherId { get; set; }

		[JsonProperty("QTY")]
		public int? Quantity { get; set; }

		[JsonProperty("PRODTYPE")]
		public string Type { get; set; }

		[JsonProperty("TIPOAQ")]
		public string AquisitionType { get; set; }

	}
}
