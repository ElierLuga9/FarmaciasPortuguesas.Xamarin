using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class AddBasketProductsIn
	{
		[JsonProperty("FARMID")]
		public string PharmacyId { get; set; }

		[JsonProperty("LISTAITEMS")]
		public List<BasketProductIn> Products { get; set; }

	}
}
