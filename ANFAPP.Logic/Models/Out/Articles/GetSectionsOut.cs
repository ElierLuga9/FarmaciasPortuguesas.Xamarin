using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
    public class GetSectionsOut 
    {
        [JsonProperty("CATS")]
        public List<SectionsOut> CategoriesList;
        [JsonProperty("INFORESULT")]
        public int code;

    }


}