using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ItemWishlistDeleteIn
	{
		[JsonProperty("NUMLINHA")]
		public int LineId { get; set; }
	}
}

