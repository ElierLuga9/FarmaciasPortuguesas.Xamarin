using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class PostOrderIn
	{
		[JsonProperty("ORDERID")]
		public string OrderNumber { get; set; }
	}
}

