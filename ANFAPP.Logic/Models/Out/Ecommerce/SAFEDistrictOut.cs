using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class SAFEDistrictOut : MagentoOut
	{
		[JsonProperty("Distrito")]
		public Nullable<int> DistrictId { get; set; }
	}
}
