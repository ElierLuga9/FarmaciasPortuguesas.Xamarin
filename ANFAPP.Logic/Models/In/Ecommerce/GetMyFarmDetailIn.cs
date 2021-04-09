using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic
{
        public class GetMyFarmDetailIn
        {
                [JsonProperty("FARMID")]
                public string FarmId { get; set; }

        }
}

