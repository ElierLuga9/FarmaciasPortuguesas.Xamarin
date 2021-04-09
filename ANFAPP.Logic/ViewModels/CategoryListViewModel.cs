using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Newtonsoft.Json;
using ANFAPP.Logic.StaticData.Models;

namespace ANFAPP.Logic.ViewModels
{
	public class CategoryListViewModel : IViewModel
	{
		public CategoryListViewModel() : base() {}

		public CategoryListViewModel(Catalog catalog) : base() {
			_catalog = catalog;
		}

		#region Properties

		private Catalog _catalog;
		public Catalog Catalog
		{
			get { return _catalog; }
			set
			{
				_catalog = value;
				OnPropertyChanged("Catalog");
				OnPropertyChanged("Families");
				OnPropertyChanged("Categories");
				OnPropertyChanged("Products");

				LoadQuery();
			}
		}

		public List<CatalogCategory> Categories
		{
			get { return _catalog == null ? null : _catalog.Categories.OrderBy(x => x.Name).ToList(); }
		}

		public List<CatalogFamily> Families
		{
			get { return _catalog == null ? null : _catalog.Families; }
		}

		public List<CatalogProduct> Products
		{
			get { return _catalog == null ? null : _catalog.Products; }
		}

		private List<CatalogProductPair> _results;

		private CatalogCategory _selectedCategory;
		public CatalogCategory SelectedCategory { 
			get { return _selectedCategory; }
		}

		private string _queryString;
		public string QueryString 
		{
			get { return _queryString; }
			set
			{ 
				_queryString = value;
				OnPropertyChanged("QueryString");
			}
		}

		public List<CatalogProductPair> SearchResults
		{

			get { return _results; }
			set { 
				_results = value; 
				OnPropertyChanged("SearchResults");
			}
		}

		public CatalogFamily SelectedFamily
		{
			get { return null == _selectedCategory ? null : Families.Find (x => x.Id == _selectedCategory.FamilyId); }
		}

		private bool _noResults;
		public bool NoResults
		{
			get { return _noResults; }
			set
			{
				_noResults = value;
				OnPropertyChanged("NoResults");
			}
		}

		#endregion

		public void SelectCategory(CatalogCategory category)
		{
			_selectedCategory = category;

			if (null != _selectedCategory)
				QueryString = _selectedCategory.Name;

			OnPropertyChanged("SelectedCategory");
			OnPropertyChanged("SelectedFamily");

			ReloadQuery();
		}

		#region Load/Unload

		private void UnloadCatalog()
		{
			this.Catalog = null;
		}

		private void LoadCatalog()
		{
			if (_catalog == null) 
			{
				try 
				{
					string raw = "";

					var assembly = typeof(CategoryListViewModel).GetTypeInfo().Assembly;
					string[] resources = assembly.GetManifestResourceNames();
					foreach (string resource in resources)
					{
						if(resource.EndsWith(".catalog.json"))
						{
							Stream stream = assembly.GetManifestResourceStream(resource);
							if (stream != null)
							{
								using (var reader = new System.IO.StreamReader(stream)) {
									raw = reader.ReadToEnd ();
								}
							}

							break;
						}
					}

					this.Catalog = JsonConvert.DeserializeObject<Catalog>(raw);
					this.Catalog.SetProductColors();
				}
				catch (Exception e)
				{
					// Service error
					this.NoResults = true;
				}
			}
		}

		private void LoadQuery()
		{
			if (null != _catalog && null != _selectedCategory) 
			{
				var results = from product in Products
					where
					product.CategoryId.Equals(_selectedCategory.Id, StringComparison.CurrentCultureIgnoreCase)
					orderby product.Description ascending
					select product;

				List<CatalogProduct> tmp = results.ToList();
				List<CatalogProductPair> tuples = new List<CatalogProductPair> ();

				for (int idx = 0; idx < tmp.Count; idx += 2) {
					var pp = new CatalogProductPair ();
					pp.Product1 = tmp[idx];

					if (idx + 1 < tmp.Count) 
						pp.Product2 = tmp[idx + 1];

					tuples.Add (pp);
				}

                // Mark last tuple as last
                if (tuples != null && tuples.Count > 0) tuples[tuples.Count - 1].IsLast = true;
				SearchResults = tuples;
			}
			else if (null != _catalog && !String.IsNullOrEmpty(_queryString)) 
			{
				var results = from product in Products
				              where
				                  product.Brand.IndexOf (_queryString, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
				                  product.Description.IndexOf (_queryString, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
				                  product.Text.IndexOf (_queryString, StringComparison.CurrentCultureIgnoreCase) >= 0
				              orderby product.Description ascending
				              select product;

				List<CatalogProduct> tmp = results.ToList();
				List<CatalogProductPair> tuples = new List<CatalogProductPair> ();

				for (int idx = 0; idx < tmp.Count; idx += 2) {
					var pp = new CatalogProductPair ();
					pp.Product1 = tmp[idx];

					if (idx + 1 < tmp.Count) 
						pp.Product2 = tmp[idx + 1];

					tuples.Add (pp);
				}

                // Mark last tuple as last
                if (tuples != null && tuples.Count > 0) tuples[tuples.Count - 1].IsLast = true;
				SearchResults = tuples;
			} 
			else 
			{
				SearchResults = null;
			}

            // Make sure the list is null when there are no results.
            if (SearchResults != null && SearchResults.Count == 0) SearchResults = null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void LoadData()
		{
			LoadCatalog();
			LoadQuery();
		}

		public void ReloadQuery()
		{
			LoadQuery();
		}

		#endregion
	}
}

