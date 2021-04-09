using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class CheckoutConfOut : MagentoOut
	{
		[JsonProperty("ERROR")]
		public bool? Error { get; set; }

		[JsonProperty("ENCSUCCESS")]
		public bool Success { get; set; }
	
		[JsonProperty("PAGAMMB")]
		public string UrlMB { get; set; }

		[JsonProperty("PAGAMHIPAY")]
		public string UrlHiPay { get; set; }

		[JsonProperty("PAGAMMB_ENT")]
		public string MBEntity { get; set; }

		[JsonProperty("PAGAMMB_REF")]
		public string MBReference { get; set; }

		[JsonProperty("PAGAMMB_VAL")]
		public string MBValue { get; set; }

		[JsonProperty("MAGENTOORDERID")]
		public string OrderId { get; set; }

		[JsonIgnore]
		public string MBValueDescription 
		{ 
			get 
			{
				return MBValue + " €"; 
			} 
		}

	}
}
