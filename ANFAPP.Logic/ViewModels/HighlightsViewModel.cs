using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using System.Collections.ObjectModel;
using ANFAPP.Logic.EventHandlers;

namespace ANFAPP.Logic.ViewModels
{
	/// <summary>
	/// Highlights view model.
	/// 
	/// This view model drives the highlights widget and the highlights list.
	///
	/// The Products property is default items source. However, if the list is grouped,
	/// the ProductsGrouped property should be used. When the list is grouped the 
	/// ProductN properties return null.
	/// 
	/// </summary>
    public class HighlightsViewModel : StoreSearchableViewModel
    {
		private bool _fromCatalog;
		private bool _grouped = true;
		private int _max = 20; // The page size. 

		public enum GroupType { Highlights, Campaigns };

		#region Unbindable Properties

		//public bool Invalid = true;
		public bool IsLoading = false;

		#endregion

		public HighlightsViewModel(bool fromCatalog, string title) : base()
		{
			_fromCatalog = fromCatalog;
			HeaderTitle = title;
		}

		public HighlightsViewModel(bool fromCatalog, string title, int max, bool grouped) : this(fromCatalog, title)
		{
			_max = max;
			_grouped = grouped;
		}

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Properties

		private bool _hasMore = true;
		public bool HasMore 
		{ 
			get { return _hasMore; }
			set
			{
				_hasMore = value;
				OnPropertyChanged("HasMore");
			}
		}

		public class ProductGroup : ObservableCollection<ProductOut>
		{
			public string Name { get; set; }
			public GroupType Type { get; set; }

			public ProductGroup(string name, GroupType type)
			{
				Name = name;
				Type = type;
			}
		}
			
        private string _headerTitle;
        public string HeaderTitle {
            get { return _headerTitle; }
            set { 
                _headerTitle = value;
                OnPropertyChanged ("HeaderTitle");
            }
        }
			
        private ObservableCollection<ProductOut> _products;
		public ObservableCollection<ProductOut> Products {
            get { return _products; }
            set { 
                _products = value;
                OnPropertyChanged ("Products");
				OnPropertyChanged ("Product0");
				OnPropertyChanged ("Product1");
                OnPropertyChanged ("Product2");
                OnPropertyChanged ("Product3");
				OnPropertyChanged ("HasProducts");
            }

        }

		private ObservableCollection<ProductGroup> _productsGrouped { get; set; }
		public ObservableCollection<ProductGroup> ProductsGrouped 
		{ 
			get { return _productsGrouped; }
			set 
			{ 
				_productsGrouped = value; 
				OnPropertyChanged("ProductsGrouped");
				OnPropertyChanged("HasProducts");
			}
		}

		public bool HasProducts { get { return _products != null && _products.Count > 0; } }

		#endregion

		#region Grid-like Summary View Support

        public ProductOut Product0 {
            get { 
				if (!_grouped && null !=_products && _products.Count > 0) {
                    return _products [0];
                } else {
                    return null;
                }
            }
        }

        public ProductOut Product1 {
            get { 
				if (!_grouped && null != _products && _products.Count > 1) {
                    return _products [1];
                } else {
                    return null;
                }
            }
        }

        public ProductOut Product2
        {
            get
            {
                if (!_grouped && null != _products && _products.Count > 2)
                {
                    return _products[2];
                }
                else
                {
                    return null;
                }
            }
        }

        public ProductOut Product3
        {
            get
            {
                if (!_grouped && null != _products && _products.Count > 3)
                {
                    return _products[3];
                }
                else
                {
                    return null;
                }
            }
        }
		#endregion

		public async Task LoadData() 
		{
			if (OnLoadStart != null) await OnLoadStart();

			try {

				if (_grouped) {
					var group = new ProductGroup(HeaderTitle, _fromCatalog ? GroupType.Campaigns : GroupType.Highlights);
					ProductsGrouped = new ObservableCollection<ProductGroup>();
					ProductsGrouped.Add(group);
				}
				Products = new ObservableCollection<ProductOut>();

				await LoadNextPage();

			} catch (Exception ex) {
				if (OnLoadError != null) OnLoadError ("", ex.Message);
			} 

			HasMore = true;
			InvalidSearchResults = false;
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		public async Task LoadNextPage()
		{
			if (!HasMore) return;
			IsLoading = true;

			try {
				var result = await LoadFeaturedProducts();

				if (_grouped) {
					var group = ProductsGrouped[0];
					foreach (ProductOut p in result.Products) {
						group.Add(p);
					}

					//ProductsGrouped = new ObservableCollection<ProductGroup>();
					//ProductsGrouped.Add(group);
				}

				foreach (var prod in result.Products) {
					Products.Add(prod);
				}

				// Because the observable collection items are not observable themselves.
				var pg = ProductsGrouped;
				ProductsGrouped = null;
				ProductsGrouped = pg;

				var pp = Products;
				Products = null;
				Products = pp;

				if (result.Products != null && result.Products.Count == 0) {
					HasMore = false;
				}

			} catch (Exception ex) {
			
			} finally {
				IsLoading = false;
			}
		}

		private async Task<FeaturedProdOut> LoadFeaturedProducts()
		{
			var pageStart = Products.Count;

			var order = SelectedOrder != null ? SelectedOrder.Id : (long?)null;
			var result = await ECommerceWS.FeaturedProd(SessionData.UserAuthentication, _max, _fromCatalog, order, pageStart);

			return result;
		}
    }
}

