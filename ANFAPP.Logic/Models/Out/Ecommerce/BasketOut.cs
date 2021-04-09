using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	public class BasketOut : BasketOutBase
	{
		#region Properties

		[JsonProperty("PRODUCTLIST")]
		public List<BasketProductOut> Products { get; set; }

		#endregion

		[JsonIgnore]
		public bool HasMSRM
		{
			get
			{
				if (Products == null || Products.Count == 0) return false;
				foreach (var prod in Products)
				{
					if (prod.MSRM.GetValueOrDefault() || (prod.HasCNPEM && !prod.HasCNP)) return true;
				}

				return false;
			}
		}

	}
}
