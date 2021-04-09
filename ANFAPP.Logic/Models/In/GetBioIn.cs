using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In
{
	public class GetBioIn
	{
		[JsonProperty("CARD")]
		public int Card { get; set; }
		[JsonProperty("DATE")]
		public string Date { get; set; }
	}
}

