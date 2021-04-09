using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class PAListItem
	{
		[JsonProperty("ACTIVEPRINID")]
		public int Id { get; set; }

		[JsonProperty("PANAME")]
		public string Name { get; set; }
	}

	public class SearchPAOut : MagentoOut
	{
		[JsonProperty("LISTAPA")]
		public List<PAListItem> ListaPA { get; set; }
	}
}

