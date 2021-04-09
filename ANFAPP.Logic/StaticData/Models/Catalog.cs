using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ANFAPP.Logic.StaticData.Models
{
	public class Catalog
	{
		[JsonProperty("families")]
		public List<CatalogFamily> Families;
		[JsonProperty("categories")]
		public List<CatalogCategory> Categories;
		[JsonProperty("products")]
		public List<CatalogProduct> Products;

		public async void SetProductColors()
		{
			// XXX: better ways to implement this?
			foreach (CatalogProduct product in Products) {
				CatalogCategory category = Categories.Find(x => x.Id.Equals(product.CategoryId));
				CatalogFamily family = Families.Find(x => x.Id.Equals(category.FamilyId));
				product.ProductFamilyColor = Color.FromHex("#" + family.ColorHex);
			}

		}
	}
}

