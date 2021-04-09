using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class CheckoutDeliveryMethodIn
	{

		[JsonProperty("MODOENTREGA")]
		public string Code { get; set; }

	}
}
