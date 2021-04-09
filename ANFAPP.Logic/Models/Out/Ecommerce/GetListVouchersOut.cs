using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class VoucherOut
	{
		[JsonProperty("ID")]
		public string Id { get; set; }

		[JsonProperty("VALOR")]
		public decimal Value { get; set; }
	}

	public class GetListVouchersOut : MagentoOut
	{
		[JsonProperty("LISTVOUCHERS")]
		public List<VoucherOut> Vouchers { get; set; }
	}
}

