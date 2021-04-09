using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
	public class ArticlesHighlightOut : ANFAPP.Logic.Models.Out.Ecommerce.MagentoOut 
    {
        [JsonProperty("DESTAQUESWP")]
        public List<HighlightOut> HighlightsList { get; set; }

        [JsonIgnore]
        public HighlightOut MainArticle
        {
            get 
            {
				return HasHighlights ? HighlightsList [0] : null;
            }

        }

        [JsonIgnore]
        public List<HighlightOut> MinorArticles 
        {
            get 
            {
                if (HasHighlights && HighlightsList.Count >= 2)
                {
                    List<HighlightOut> temp = HighlightsList.GetRange(1, HighlightsList.Count-1);
                    return temp;
                }
                else 
                {
                    List<HighlightOut> temp = new List<HighlightOut>{};
                    return temp;
                }
            }
        }

        [JsonIgnore]
        public bool HasHighlights
        {
			get { return HighlightsList != null && HighlightsList.Count > 0; }
        }
    }

}