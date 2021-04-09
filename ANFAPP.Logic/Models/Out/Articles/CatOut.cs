using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.Models.Out.Articles
{
    public class CatOut
    {
        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("NOME")]
        public string Name { get; set; }
    }
}