using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class SearchIn
	{
		[JsonProperty(PropertyName="FARMID", Required=Required.Always)]
		public string FarmID { get; set; }

		[JsonProperty(PropertyName="QTY", Required=Required.Always)]
		public int Qty { get; set; }

		[JsonProperty(PropertyName="KEY")]
		public string Key { get; set; }

		[JsonProperty("CNPEM")]
		public int? CNPEM { get; set; }

		[JsonProperty(PropertyName="ACTIVEPRIN")]
		public string ActivePrin { get; set; }

		[JsonProperty(PropertyName="RINICIAL")]
		public int? PageStart { get; set; }

		[JsonProperty("FILPOINTS")]
		public long? FilPoints { get; set; }

		[JsonProperty("FILBRAND")]
		public long? FilBrand { get; set; }

		[JsonProperty("FILDOS")]
		public long? FilDos { get; set; }

		[JsonProperty("FILFF")]
		public long? FilFF { get; set; }

		[JsonProperty("FILCAT")]
		public long? FilCat { get; set; }

		[JsonProperty("FILPRICE")]
		public long? FilPrice { get; set; }

		[JsonProperty("ORDERTYPE")]
		public long? OrderType { get; set; }

		[JsonProperty("ONLYPOINTSCAT")]
		public string FilterPointsCatalogProducts { get; set; }

		// http://issue.innovagency.com/view.php?id=20949
		[JsonProperty("ALLPRODS")]
		public string SearchAllProducts { get; set; }
	}
}

