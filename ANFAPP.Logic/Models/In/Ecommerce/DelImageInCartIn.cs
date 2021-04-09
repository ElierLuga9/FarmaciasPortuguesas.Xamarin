using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class DelImageInCartIn
	{
		[JsonProperty("IMAGENAME")]
		public string ImageName { get; set; }
	}
}
