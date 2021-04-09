using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class GetSAFEBannersIn : GetBannersIn
	{
		[JsonProperty("DISTRITOID")]
		public Nullable<int> DistrictId { get; set; }
	}
}
