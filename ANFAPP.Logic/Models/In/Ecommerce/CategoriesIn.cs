using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class CategoriesIn
	{
		[JsonProperty("CATID")]
		public long CatId { get; set; }
	}
}

