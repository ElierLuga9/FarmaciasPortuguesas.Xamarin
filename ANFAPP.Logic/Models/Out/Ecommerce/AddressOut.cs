using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class AddressOut
	{

		[JsonProperty("ADDRESS1")]
		public string Address1 { get; set; }

		[JsonProperty("ADDRESS2")]
		public string Address2 { get; set; }

		[JsonProperty("DISTRICT")]
		public string District { get; set; }

		[JsonProperty("COUNTY")]
		public string County { get; set; }

		[JsonProperty("LOCALE")]
		public string Location { get; set; }

		[JsonProperty("POSTALCODE")]
		public string PostalCode { get; set; }

		[JsonProperty("NOME")]
		public string Name { get; set; }

		[JsonProperty("NIF")]
		public long NIF { get; set; }

		[JsonProperty("TELEPHONE")]
		public long Phone { get; set; }

	}
}
