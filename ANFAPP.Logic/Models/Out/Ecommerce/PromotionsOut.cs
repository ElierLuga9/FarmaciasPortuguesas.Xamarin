using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class PromotionsOut
    {
        #region Properties
        public bool Promo1IsActive { get; set; }
        public bool Promo2IsActive { get; set; }
        public bool Promo3IsActive { get; set; }
        public bool Promo4IsActive { get; set; }

        public string ButtonLabel { get; set; }

        public string DateLabel { get; set; }

        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("PromoTypePar")]
        public string  PromoTypePar { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("FullPathImage")]
        public string FullPathImage { get; set; }
        [JsonProperty("UpdateDate")]
        public DateTime UpdateDate { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("PromoType")]
        public int PromoType { get; set; }
        [JsonProperty("PromoTypeDescription")]
        public string PromoTypeDescription { get; set; }
        [JsonProperty("Order")]
        public int? Order { get; set; }
        [JsonProperty("Lead")]
        public string Lead { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("DateEnd")]
        public DateTime? DateEnd { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("DateStart")]
        public DateTime? DateStart { get; set; }
        [JsonProperty("CNPList")]
        public List<Product> CNPList { get; set; }
		[JsonProperty("OnlyImage")]
		public bool OnlyImage { get; set; }


		public bool Read { get; set; }
        #endregion
    }

    public class Product
    {
        #region Properties
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("CNP")]
        public string CNP { get; set; }
        //public string Name { get; set; }
        [JsonProperty("Order")]
        public int? Order { get; set; }
        //public int ListaId { get; set; }
        #endregion
    }

    public class ProductsList
    {
        #region Properties
        public int ListaId { get; set; }
        public string Title { get; set; }
        #endregion
    }

      public class PromotionsOutList
      {
          public List<PromotionsOut> PromotionsList { get; set; }

          public bool hasPromotionsList
          {
              get
              {
                  return (PromotionsList.Count == 0) ? false : true;
              }

          }
      }
}
