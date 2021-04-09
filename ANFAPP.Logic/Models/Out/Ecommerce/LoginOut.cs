using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class LoginOut : MagentoOut
	{
		[JsonProperty("TOKENID")]
		public string TokenId { get; set; }
	}
}

