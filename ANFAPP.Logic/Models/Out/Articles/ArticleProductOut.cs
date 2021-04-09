using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
    public class ArticleProductOut
    {
        [JsonProperty("RESUMO")]
        public string summary { get; set; }
        [JsonProperty("CONTENT")]
        public string content { get; set; }
        [JsonProperty("MAGENTOPRODUTOS")]
        public List<ArticleProductOut> productsList { get; set; }
        [JsonProperty("MAGENTOCATEGORIAS")]
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
                return (string.IsNullOrEmpty(video)) ? false : true;
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