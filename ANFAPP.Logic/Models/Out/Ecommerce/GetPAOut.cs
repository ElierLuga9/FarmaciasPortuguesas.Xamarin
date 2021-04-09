using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class DosageItem
	{
		[JsonProperty("DOSID")]
		public int Id { get; set; }
		[JsonProperty("DOSNAME")]
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	public class PackItem
	{
		[JsonProperty("EMBID")]
		public int Id { get; set; }
		[JsonProperty("EMBNAME")]
		public string Name { get; set; }

		public int CNPEM { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	public class GetPAOut : MagentoOut
	{
		[JsonProperty("ACTIVEPRINID")]
		public int ActivePrincipleId { get; set; }
		[JsonProperty("PANAME")]
		public string ActivePrinciple { get; set; }
		[JsonProperty("LISTDOS")]
		public List<DosageItem> Dosages { get; set; }
		[JsonProperty("LISTEMB")]
		public List<PackItem> Packs { get; set; }
	}
}

