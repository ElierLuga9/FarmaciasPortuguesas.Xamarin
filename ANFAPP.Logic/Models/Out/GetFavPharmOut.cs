using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	public class GetFavPharmOut
	{
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("pharmacy")]
		public Pharmacy pharmacy { get; set;}

		public class Pharmacy { 
			[JsonProperty("code")]
			public string Code { get; set;}
			[JsonProperty("name")]
			public string Name { get; set; }
		}
	}

}
