using ANFAPP.Logic.Models.Out.Ecommerce;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class CheckoutAddressIn
	{

		[JsonProperty("SHIPPINGADDRESS")]
		public List<AddressOut> ShippingAddresses { get; set; }

		[JsonProperty("BILLINGADDRESS")]
		public List<AddressOut> BillingAddresses { get; set; }

	}
}
