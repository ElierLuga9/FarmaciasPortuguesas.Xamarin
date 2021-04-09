using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ClientePromoCrmOut 
	{
		[JsonProperty("value")]
		public List<Promotions> PromotionsEvent { get; set; }
		public class Promotions
		{
			[JsonProperty("Code")]
			public string Code { get; set;}
			[JsonProperty("Name")]
			public string Name { get; set;}
			[JsonProperty("BurnConditions")]
			public List<Conditions> CardCondition { get; set; }


		}
		public class Conditions
		{
			[JsonProperty("Code")]
			public string Code { get; set; }
		}
	}
}


