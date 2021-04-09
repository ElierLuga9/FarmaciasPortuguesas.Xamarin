using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class SearchPAIn
	{
		[JsonProperty(PropertyName="ACTIVEPRIN", Required=Required.Always)]
		public string ActivePrinciple { get; set; }
	}
}
