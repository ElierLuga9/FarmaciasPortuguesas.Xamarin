using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class DosageFilterOut
	{

		[JsonProperty("DOSID")]
		public long ID { get; set; }

		[JsonProperty("DOSNAME")]
		public string Name { get; set; }

	}
}
