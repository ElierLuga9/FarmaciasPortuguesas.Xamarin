using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.StaticData.Models
{
	public class CatalogFamily
	{
		[JsonProperty("fid")]
		public UInt64 Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("color")]
		public string ColorHex { get; set; }

		[JsonIgnore]
		public Color Color
		{
			get { return Color.FromHex("#" + this.ColorHex); }
		}

		[JsonIgnore]
		public string IconName
		{
			get 
			{ 
				string iconName = null;
				switch (this.Id) 
				{
					case 101:
						iconName = "familia_saude.png";
						break;

					case 102:
						iconName = "familia_bemestar.png";	
						break;

					case 103:
						iconName = "familia_veterinaria.png";
						break;

					default:
						break;
				}

				return iconName;
			}
		}
	}
}
