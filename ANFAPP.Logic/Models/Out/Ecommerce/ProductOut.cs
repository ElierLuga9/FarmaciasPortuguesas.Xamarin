using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class ProductOut : ProductBase
	{

		#region Properties

		[JsonProperty("IMAGE")]
		public string Image { get; set; }

		[JsonProperty("NUMLINHA")]
		public int? LineNumber { get; set; }

		[JsonProperty("AVAILABLEINFARM")]
		public bool ExistsInFarmCatalog { get; set; }

		#endregion

		#region Auxiliary Properties

		[JsonIgnore]
		public bool FromCatalog { get; set; }

		[JsonIgnore]
		public UriImageSource ImageSource
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Image) ? 
					new UriImageSource
					{
						Uri = new Uri(Uri.UnescapeDataString(Image)),
						CachingEnabled = false
					} : null;
			}
		}

		[JsonIgnore]
		public string ImageURL
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Image) ? Uri.UnescapeDataString(Image) : null;
			}
		}

		#endregion

	}
}
