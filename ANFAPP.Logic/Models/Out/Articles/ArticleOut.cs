using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Ecommerce;


namespace ANFAPP.Logic.Models.Out.Articles
{
    public class ArticleOut : HighlightOut
    {
        [JsonProperty("RESUMO")]
        public string summary { get; set; }
        [JsonProperty("CONTENT")]
        public string content { get; set; }
        [JsonProperty("URL")]
        public string url { get; set; }
        [JsonProperty("MAGENTOPRODUTOS")]
        public List<ProductOut> productsList { get; set; }
        [JsonProperty("ARTIGOSWPRELACIONADOS")]
        public List<HighlightOut> articlesList { get; set; }
        [JsonProperty("VIDEOURL")]
        public string video { get; set; }
        [JsonProperty("INFORESULT")]
        public string code { get; set; }

        [JsonIgnore]
        public bool hasVideo
        {
            get
            {
				return !string.IsNullOrEmpty(video);
            }
        }
        [JsonIgnore]
        public bool hasImage
        {
            get 
            {
				return !string.IsNullOrEmpty (Image) && !hasVideo;
            }
        }

        [JsonIgnore]
        public bool hasProducts
        {
            get
            {
                return (productsList.Count == 0) ? false : true;
            }
        }

        [JsonIgnore]
        public bool hasArticles
        { 
            get 
            { 
                return (articlesList.Count == 0) ? false : true; 
            } 
        }
    }
}