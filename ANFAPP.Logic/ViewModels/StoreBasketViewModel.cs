using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Utils;
using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreBasketViewModel : IViewModel
	{
		
		#region Properties

		private BasketOut _basket;
		public BasketOut Basket
		{
			get { return _basket; }
			set
			{
				_basket = value;
				OnPropertyChanged("Basket");
				OnPropertyChanged("ProductsCount");
			}
		}

		public int ProductsCount
		{
			get { return _basket != null ? _basket.DistinctProductsInCart : 0; }
		}

		public bool HasProducts
		{
			get { return Basket != null && (Basket.Products != null || Basket.DistinctProductsInCart > 0); }
		}
		private bool  _hasProducts = false;
		public bool hasProducts
		{
			get { return _hasProducts; }
			set
			{
				_hasProducts = value;
				OnPropertyChanged("hasProducts");
			}
		}
		private bool _hasVouchers = false;
		public bool hasVouchers
		{
			get { return _hasVouchers; }
			set
			{
				_hasVouchers = value;
				OnPropertyChanged("hasVouchers");
			}
		}


		private string _vouchersValueDescriptio;
		public string VouchersValueDescription
		{
			get { return _vouchersValueDescriptio; }
			set
			{
				_vouchersValueDescriptio = value;
				OnPropertyChanged("VouchersValueDescription");
			}
		}

		private string _totalValueDescription;
		public string TotalValueDescription
		{
			get { return _totalValueDescription; }
			set
			{
				_totalValueDescription = value;
				OnPropertyChanged("TotalValueDescription");
			}
		}
		public bool IsLoaded = false;

		#endregion

		#region Event Handlers

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Service Calls

		/// <summary>
		/// Loads the basket from the webservice
		/// </summary>
		public async Task LoadBasket()
		{
			if (OnLoadStart != null) await OnLoadStart();

			await LoadBasketAsync();
			if (Basket != null && (Basket.Products != null || Basket.DistinctProductsInCart > 0))
			{
				hasProducts = false;

			}
			else 
			{
				hasProducts = true;
				if (Basket.TotalVouchers > 0 || Basket.TotalVouchers == null)
				{
					hasVouchers = true;
				}
				else 
				{
					hasVouchers = false;
				}
			}
			//VouchersValueDescription = "" + (decimal)Basket.TotalVouchers + "€";
			VouchersValueDescription = "" + (decimal)Basket.TotalVouchers + "€";

		}

		public async Task PeekBasket()
		{
			if (OnLoadStart != null) await OnLoadStart();

			try
			{
				// Call WS and update the basket
				var result = await ECommerceWS.GetBasketQuantities(SessionData.UserAuthentication);
			
				if (Basket == null) {
					Basket = new BasketOut { DistinctProductsInCart = result.DistinctProducts, TotalQuantityInCart = result.TotalProducts};
				} else {
					Basket.DistinctProductsInCart = result.DistinctProducts;
					Basket.TotalQuantityInCart = result.TotalProducts;
				}
					
				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}

			IsLoaded = true;
			OnPropertyChanged("HasProducts");
		}

		/// <summary>
		/// Call the webservice and load the basket.
		/// </summary>
		/// <returns></returns>
		private async Task LoadBasketAsync()
		{
			try
			{
				// Call WS and update the basket
				var result = await ECommerceWS.GetBasket(SessionData.UserAuthentication);

				// Clear the basket
				WipeBasketCache();

				// Clear the taxes with no value
				if (result != null && result.Taxes != null)
				{
					var taxes = new List<IVAOut>();
					foreach (var tax in result.Taxes)
					{
						if (tax.Value > 0) taxes.Add(tax);
					}
					result.Taxes = taxes;
				}

				if (result != null && result.Products != null && result.Products.Count == 0) result.Products = null;

				Basket = result;
				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e)
			{
				// Clear the basket
				WipeBasketCache();

				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}

			IsLoaded = true;
			OnPropertyChanged("HasProducts");
		}

		/// <summary>
		/// Wipes the basket's cache.
		/// </summary>
		public void WipeBasketCache()
		{
			IsLoaded = false;
			Basket = new BasketOut()
			{
				Products = null
			};
		}

		/// <summary>
		/// Determines whether this instance can toggle product aquisition state.
		/// </summary>
		/// <returns><c>true</c> if this instance can toggle product aquisition state; otherwise, <c>false</c>.</returns>
		public bool CanToggleProductAquisitionState() 
		{
			if (Basket == null || Basket.Products == null) return false;

			foreach (var prod in Basket.Products)
			{
				if (prod.CanToggleAquisition) return true;
			}

			return false;
		}

		#region Add & Remove Products
        /// <summary>
        /// Add N products to the basket
        /// </summary>
		public async Task AddProductToBasket(ProductOut product, int quantity, string aquisitionType = "V")
        {
            // Can't add products to the basket is the pharmacy is not selected.
            if (!SessionData.IsPharmacySelected) return;

            if (OnLoadStart != null) await OnLoadStart();

            try
            {
				//var result = await ECommerceWS.AddProductToBasket(SessionData.UserAuthentication, product.CNP.Value,product.CNPEM, quantity,product.HasCNPEM, aquisitionType);
				var result = await ECommerceWS.AddProductToBasket(SessionData.UserAuthentication, product, quantity, aquisitionType);
                if (result.code != ECommerceWS.CODE_MG_CREATED)
                {
                    // Operation failed
                    throw new ServiceErrorException(result.msg);
                }

				var mixpanelWidget = DependencyService.Get<IMixPanel>();
				var props = new Dictionary<string, string>();

				if (product != null) {
					props.Add("CNP", product.CNP.ToString());
//					props.Add("CNPEM", product.CNPEM.ToString());
					props.Add("Quantity", quantity.ToString());

					var price = aquisitionType == "V" ? product.Price.Value + "€" : product.Points + " Points";
					props.Add("Price", price.Replace(",", "."));
				}
				props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
				props.Add("PharmacyID", SessionData.StorePharmacyId);

				mixpanelWidget.TrackProperties("AddToCart", props);

				// Update the basket totals.
				Basket.DistinctProductsInCart = result.DistinctProducts;
				Basket.TotalQuantityInCart = result.TotalProducts;
            }
            catch (Exception e)
            {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
            }

            if (OnLoadSuccess != null) OnLoadSuccess();
        }


		/// <summary>
		/// Add a product to the basket.
		/// </summary>
		/// <param name="product"></param>
		private async Task AddProductToBasketWithType(ProductOut product, string aquisitionType) 
		{
			// Can't add products to the basket is the pharmacy is not selected.
			if (!SessionData.IsPharmacySelected) return;

            if (OnLoadStart != null) await OnLoadStart();

			try
			{
				var result = await ECommerceWS.AddProductToBasket(SessionData.UserAuthentication, product, 1, aquisitionType);
				if (result.code != ECommerceWS.CODE_MG_CREATED) 
				{
					// Operation failed
					throw new ServiceErrorException(result.msg);
				}

				var mixpanelWidget = DependencyService.Get<IMixPanel>();
				var props = new Dictionary<string, string>();

				if (product != null) {
					props.Add("CNP", product.CNP.ToString());
//					props.Add("CNPEM", product.CNPEM.ToString());
					props.Add("Quantity", "1");

					var price = aquisitionType == "V" ? product.Price.Value + "€" : product.Points + " Points";
					props.Add("Price", price.Replace("," , "."));
				}
				props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
				props.Add("PharmacyID", SessionData.StorePharmacyId);

				mixpanelWidget.TrackProperties("AddToCart", props);


				// Update the basket totals.
				Basket.DistinctProductsInCart = result.DistinctProducts;
				Basket.TotalQuantityInCart = result.TotalProducts;
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		//public async Task AddProductToBasket(ProductOut product) 
		//{
		//	await AddProductToBasketWithType (product, "V");
		//}

		//public async Task AddCatalogProductToBasket(ProductOut product) 
		//{
		//	await AddProductToBasketWithType (product, "P");
		//}

		public async Task AddProductToBasketWithCatalogRules(ProductOut product, bool forceCatalog, int quantity = 1)
		{
			if (product == null) return;

			if (forceCatalog || (product.HasPoints && product.Points > 0 && product.Price.HasValue && product.Price == 0))
			{
				await AddProductToBasket(product, quantity, "P");
			}
			else
			{
				await AddProductToBasket(product, quantity, "V");
			}
		}

		/// <summary>
		/// Edit a product's quantity.
		/// </summary>
		public async Task EditProductQuantity(BasketProductOut product, int quantity) 
		{
			if (product.Quantity == quantity) return;
			if (OnLoadStart != null) await OnLoadStart();

			try
			{
				int diff = quantity - product.Quantity;
				product.Quantity = quantity;

				var result = await ECommerceWS.EditBasketProduct(SessionData.UserAuthentication, product.LineNumber.Value, quantity);
				if (result.code != ECommerceWS.CODE_MG_CREATED)
				{
					// Operation failed
					throw new ServiceErrorException(result.msg);
				}
					
				if (diff != 0) {
					var mixpanelWidget = DependencyService.Get<IMixPanel>();
					var props = new Dictionary<string, string>();

					if (product != null) {
						props.Add("CNP", product.CNP.ToString());
						props.Add("Quantity", Math.Abs(diff).ToString());

						var price = product.AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY ? product.Price.Value + "€" : product.Points + " Points";
						props.Add("Price", price.Replace(",", "."));
					}
					props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
					props.Add("PharmacyID", SessionData.StorePharmacyId);

					mixpanelWidget.TrackProperties((diff < 0) ? "RemoveFromCart" : "AddToCart", props);	
				}

			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			// Reload the basket
			await LoadBasketAsync();
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		/// <summary>
		/// Remove a product from the basket
		/// </summary>
		public async Task RemoveProductFromBasket(BasketProductOut product) 
		{
			if (OnLoadStart != null) await OnLoadStart();

			try
			{
				var result = await ECommerceWS.DeleteBasketProduct(SessionData.UserAuthentication, product.LineNumber.Value);
				if (result.code != ECommerceWS.CODE_MG_CREATED)
				{
					// Operation failed
					throw new ServiceErrorException(result.msg);
				}
				else
				{
					_basket.DistinctProductsInCart = _basket.DistinctProductsInCart  -1;
				}
				if (_basket.DistinctProductsInCart.Equals(0)) 
				{
					hasProducts = true;
					if (Basket.TotalVouchers > 0)
					{
						hasVouchers = true;
					}
					else 
					{
						hasVouchers = false;
					}
				}
				var mixpanelWidget = DependencyService.Get<IMixPanel>();
				var props = new Dictionary<string, string>();

				if (product != null) {
					props.Add("CNP", product.CNP.ToString());
//					props.Add("CNPEM", product.CNPEM.ToString());
					props.Add("Quantity", product.Quantity.ToString());

					var price = product.AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY ? product.Price.Value + "€" : product.Points + " Points";
					props.Add("Price", price.Replace(",", "."));
				}
				props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
				props.Add("PharmacyID", SessionData.StorePharmacyId);

				mixpanelWidget.TrackProperties("RemoveFromCart", props);


			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			// Reload the basket
			await LoadBasketAsync();
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		/// <summary>
		/// Removes all products from the basket.
		/// </summary>
		public async Task ClearBasket() 
		{
			if (OnLoadStart != null) await OnLoadStart();

			try
			{
				await ECommerceWS.ClearCart(SessionData.UserAuthentication);
				hasProducts = true;
				hasVouchers = false;

				if (Basket != null && Basket.Products != null) {
					var mixpanelWidget = DependencyService.Get<IMixPanel>();
					foreach (var prod in Basket.Products) {
						var props = new Dictionary<string, string>();

						props.Add("CNP", prod.CNP.ToString());
						props.Add("Quantity", prod.Quantity.ToString());

						var price = prod.AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY ? prod.Price.Value + "€" : prod.Points + " Points";
						props.Add("Price", price.Replace(",", "."));
						props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
						props.Add("PharmacyID", SessionData.StorePharmacyId);

						mixpanelWidget.TrackProperties("RemoveFromCart", props);		
					}
				}


					

			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			// Reload the basket
			await LoadBasketAsync();
			if (OnLoadSuccess != null) OnLoadSuccess();
		}
		public async Task ClearBasketWithOnlyVouchers() 
		{
			try
			{
				await ECommerceWS.ClearCart(SessionData.UserAuthentication);

			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}
			await LoadBasketAsync();

		}
		#endregion

		#endregion

	}
}