using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
    public class SectionsOut
    {
        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("TITULO")]
        public string Name { get; set; }
        [JsonProperty("LASTCAT")]
        public bool LastSection { get; set; }
		[JsonProperty("ARTIGOS")]
		public List<HighlightOut> AllArticles { get; set; }
        [JsonProperty("NUMARTIGOS")]
        public int Quantity { get; set; }


		[JsonIgnore]
		public bool HasMore
		{
			get
			{
				return (AllArticles != null && AllArticles.Count > 3);
			}
		}

		[JsonIgnore]
		public bool HasArticles
		{
			get
			{
				return (AllArticles != null && AllArticles.Count > 0);
			}
		}

		[JsonIgnore]
		public List<HighlightOut> ArticleHighlights 
		{
			get
			{
				return (AllArticles != null && AllArticles.Count > 3) ? AllArticles.GetRange(0, 3) : AllArticles;
			}
		}

    }
}