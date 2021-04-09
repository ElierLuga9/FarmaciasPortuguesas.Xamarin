using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class AddWishlistProdIn
	{
		[JsonProperty("CNP")]
		public int CNP { get; set; }
	}
}

