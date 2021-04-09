using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class SearchOut : MagentoOut
	{

		#region Properties

		[JsonProperty("RESULTS")]
		public int Results { get; set; }

		[JsonProperty("PRODUCTS")]
		public List<ProductOut> Products { get; set; }

		[JsonProperty(PropertyName="FILCAT", NullValueHandling=NullValueHandling.Ignore)]
		public List<CategoryOut> FilCat { get; set; }

		[JsonProperty("FILDOS")]
		public List<DosageFilterOut> DosageFilters { get; set; }

		[JsonProperty("FILLFF")]
		public List<FFFilterOut> FFFilters { get; set; }

		[JsonProperty("BRANDS")]
		public List<BrandFilterOut> BrandFilters { get; set; }

		#endregion

	}
}
