using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
    public class UserOrderOut : MagentoOut
    {

        [JsonProperty("LISTENC")]
        public List<DeliveryOut> DeliveryList{get;set;}

        [JsonProperty("INFORESULT")]
        public int InfoResult { get; set;}

        [JsonIgnore]
        public bool hasDeliveryList 
        {
            get 
            {
                return (DeliveryList.Count == 0) ? false : true;
            }

        }
    }
}