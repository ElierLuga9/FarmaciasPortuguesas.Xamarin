using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{

	public static class BannerExtensions
	{        
		public static int ToAppArea(this BannerDestination bannerDestination)
		{
			// See AppArea in App.cs. At the moment the AppArea values
			// match the BannerDestination values. Eventually, this should
			// be refactored... maybe extract a single enum.
			return (int)bannerDestination;
		}
	}

	public enum BannerDestination
	{
		Undefined = 0,
		ProductDetail = 1,
		ProductsInCategory = 2,
		ArticleDetail = 3,
		ProductsWithBrand = 4,
		DrugAlertsHome = 5,
		BiometricHome = 6,
		ArticlesHome = 7,
		GamificationHome = 8,
	}

	public class BannerOut
	{
		[JsonProperty("IMAGEM")]
		public string Url { get; set; }

		[JsonProperty("TITULO")]
		public string Title { get; set; }

		[JsonProperty("DESTINOTIPO")]
		public int DestinationType { get; set; }

		[JsonProperty("DESTINOID")]
		public int DestinationId { get; set; }

		[JsonIgnore]
		public UriImageSource ImageSource
		{
			get {
				return !string.IsNullOrWhiteSpace(Url) ? 
					new UriImageSource
					{
						Uri = new Uri(Uri.UnescapeDataString(Url)),
						CachingEnabled = false
					} : null;
			}
		} 
	}

	public class GetBannersOut : MagentoOut
	{
		[JsonProperty("BANNERS")]
		public List<BannerOut> Banners { get; set; }
	}
}

