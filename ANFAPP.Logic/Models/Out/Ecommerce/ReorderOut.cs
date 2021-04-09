using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ReorderOut : MagentoOut
	{
		#region Properties

		[JsonProperty("PRODUCTSUCCESSCOUNT")]
		public int? ProductsAddedCount { get; set; }

		[JsonProperty("PRODUCTSADDED")]
		public List<ReorderProduct> ProductsAdded { get; set; }

		#endregion

		#region Inner Classes

		public class ReorderProduct 
		{
			[JsonProperty("CNP")]
			public string CNP { get; set; }

			[JsonProperty("PRICE")]
			public string Price { get; set; }

			[JsonProperty("QTY")]
			public string Quantity { get; set; }
		}

		#endregion

	}
}

