using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ANFAPP.Logic.Resources;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class CategoryOut
	{
        
		[JsonProperty("CATID")]
		public long CatId { get; set; }
		[JsonProperty("CATNAME")]
		public string CatName { get; set; }
		[JsonProperty("CATLAST")]
		public bool CatLast { get; set; }

        //testes
        [JsonProperty("CATDESTAQUE")]
        public bool CatDestaque { get; set; }
       
        
        public Color TextColor { get; set; }

        public Color BackgroundColor { get; set; }
	}

	public class CategoriesOut : MagentoOut
	{
		[JsonProperty("CATLIST")]
		public List<CategoryOut> CatList { get; set; }
	}
}

