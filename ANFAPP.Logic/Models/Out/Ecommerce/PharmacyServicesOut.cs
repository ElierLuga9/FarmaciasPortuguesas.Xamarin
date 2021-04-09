using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class PharmacyService
	{
		[JsonProperty("ID")]
		public int Id { get; set; }

		[JsonProperty("DESCRICAO")]
		public string Description { get; set; }

		[JsonProperty("TIPO")]
		public int Type { get; set; }

		[JsonIgnore]
		public char SortKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Description) || Description.Length == 0)
					return '?';

				return Description[0];
			}
		}
	}

	public class PharmacyServicesOut : MagentoOut
	{
		[JsonProperty("LISTASERVICOS")]
		public List<PharmacyService> Services { get; set; }
	}
}

