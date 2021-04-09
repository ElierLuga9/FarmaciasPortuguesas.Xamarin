using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class FFFilterOut
	{
		[JsonProperty("FFID")]
		public long ID { get; set; }

		[JsonProperty("FFNAME")]
		public string Name { get; set; }
	}
}
