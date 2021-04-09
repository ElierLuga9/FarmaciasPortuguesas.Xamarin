using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class PutImageInCartIn
	{
		[JsonProperty("IMAGEBASE64")]
		public string Encoded { get; set; }

		[JsonProperty("IMAGEFILETYPE")]
		public string MimeType { get; set; }

		[JsonProperty("IMAGENAME")]
		public string ImageName { get; set; }

		[JsonProperty("IMAGEDOCTYPE")]
		public int Type { get; set; }
	}
}

