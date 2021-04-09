using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.StaticData.Models
{
	public class CatalogProduct
	{
		// TODO: find me a better place!
		private const string CatalogBaseURL = "http://www.farmaciasportuguesas.pt/cdn/app";

		[JsonProperty("cnp")]
		public Int64 CNP { get; set; }
		[JsonProperty("pts")]
		public Int32 Points { get; set; }
		[JsonProperty("brd")]
		public string Brand { get; set; }
		[JsonProperty("dsc")]
		public string Description { get; set; }
		[JsonProperty("txt")]
		public string Text { get; set; }
		[JsonProperty("sup")]
		public string Supplier { get; set; }
		[JsonProperty("cat")]
		public string CategoryDescription { get; set; }
		[JsonProperty("cah")]
		public string CategoryId { get; set; }

		[JsonIgnore]
		public Color ProductFamilyColor { get; set; }

		[JsonIgnore]
		public string ThumbnailURL
		{
			get
			{ 
				return CatalogBaseURL + "/sml/" + CNP.ToString () + ".jpg";
			}

		}

		[JsonIgnore]
		public UriImageSource ThumbnailSource
		{
			get
			{ 
				return new UriImageSource {
					Uri = new Uri(CatalogBaseURL + "/sml/" + CNP.ToString() + ".jpg"),
					CachingEnabled = true
				};
			}
		}

		[JsonIgnore]
		public string DetailURL
		{
			get
			{ 
				return CatalogBaseURL + "/img/" + CNP.ToString() + ".jpg";
			}
		}

		[JsonIgnore]
		public UriImageSource DetailSource
		{
			get
			{ 
				return new UriImageSource {
					Uri = new Uri(CatalogBaseURL + "/img/" + CNP.ToString() + ".jpg"),
					CachingEnabled = true
				};
			}
		}

	}
}
	