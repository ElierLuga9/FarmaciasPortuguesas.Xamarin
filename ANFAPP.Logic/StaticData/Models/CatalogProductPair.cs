using System;
using ANFAPP.Logic.StaticData.Models;

namespace ANFAPP.Logic.StaticData.Models
{
	public class CatalogProductPair
	{
		public CatalogProduct Product1 { get; set; }
		public CatalogProduct Product2 { get; set; }

        public bool IsLast { get; set; }
	}
}

