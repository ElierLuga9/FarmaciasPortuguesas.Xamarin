using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
    public class SearchArticlesOut
    {
        [JsonProperty("NUMRESULT")]
        public int Results { get; set; }
        [JsonProperty("ARTIGOS")]
        public List<HighlightOut> Articles { get; set; }
        [JsonProperty("INFORESULT")]
        public int code { get; set; }
    }

}