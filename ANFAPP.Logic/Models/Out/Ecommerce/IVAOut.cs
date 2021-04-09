using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class IVAOut
	{

		[JsonProperty("TAXAIVA")]
		public decimal Tax { get; set; }
		[JsonProperty("ValSujIva")]
		public decimal TaxValue { get; set; }
		[JsonProperty("ValIva")]
		public decimal Value { get; set; }
		[JsonProperty("NumeroItens")]
		public int NumItems { get; set; }

		[JsonIgnore]
		public string TaxDescription 
		{
			get
			{
				return string.Format("IVA {0}%: {1}€", Tax, Value); 
			}
		}

	}
}
