using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{

    public class HighlightOut
    {
        [JsonIgnore]
        protected static readonly string SEPARATOR = ", ";

        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("TITULO")]
        public string Title { get; set; }
        [JsonProperty("SUBTITULO")]
        public string Subtitle { get; set; }
        [JsonProperty("FOTOPRINCIPAL")]
        public string Image { get; set; }
        [JsonProperty("SECCOES")]
        public List<CatOut> Sections { get; set; }
        [JsonProperty("DATE")]
        public DateTime? CreationDate { get; set; }
        [JsonProperty("MODIFIED")]
        public DateTime? UpdateDate { get; set; }

		[JsonIgnore]
		public UriImageSource ImageSource 
		{
			get
			{ 
				if (string.IsNullOrEmpty (Image))
					return null;

				UriImageSource source;
				if (Device.OS == TargetPlatform.iOS) {
					// Fix missing images on iOS 7.
					source = new UriImageSource { Uri = new Uri (Image), CachingEnabled = false };
				} else {
					source = new UriImageSource { Uri = new Uri (Image), CachingEnabled = true };
				}
				return source;
			}
		}

        [JsonIgnore]
        public string GetCategoryName
        {
            get
            {
                string result = null;
                if (Sections != null)
                {
                    int total = Sections.Count;
                    for (int i = 0; i < total; i++)
                    {
                        result += (i < (total - 1)) ? (Sections[i].Name + SEPARATOR) : (Sections[i].Name);
                    }
                  
                }
                return result;
            }

        }

		[JsonIgnore]
		public string DecodedTitle
		{
			get { 
				if (Title != null) {
					return System.Net.WebUtility.HtmlDecode (Title);
				}
				return null;
			}
		}
    }

}
