using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services.Abstract;
using ANFAPP.Logic.Models.In.Ecommerce;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Models.Authorization;
using System.Collections.Generic;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Wrappers;
using System.IO;
using System.Threading;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Models.Out;

namespace ANFAPP.Logic.Network.Services
{
	public class ECommerceWS : MagentoWS
	{
		public static async Task<LoginOut> Login(AnfAuthorizationBundle authBundle, string email)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new LoginIn()
			{
				EMAIL = email,
			});

			// Call web service async
			var response = await Client.PostJson(WS_MG_LOGIN, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<LoginOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}
	
        //testes
        public static async Task<PromotionsOutList> GetPromotions(AnfAuthorizationBundle authBundle)
        {
            try
            {
                // Build request message
                string requestMessage = JsonConvert.SerializeObject(new PromotionsIn());

                //Call web service async
                var response = await Client.Get(Settings.PROMOTIONS_ENDPOINT);
                var obj = JsonConvert.DeserializeObject<List<PromotionsOut>>(response.Message);
              
                var list = new PromotionsOutList()
                {
                    PromotionsList = obj
                };

                //SessionData.SaveAuthorization(authBundle, false);

                return list;
            }
            catch (Exception ex)
            {
                
                throw;
            }
         
        }

        //public static async Task<PromotionsOut>GetPromotionsById(AnfAuthorizationBundle authBundle, int id) 
        //{
        //    // Build request message
        //    string requestMessage = JsonConvert.SerializeObject(new PromotionsIn() { Id = id, });

        //    //Call web service async
        //    var response = await Client.PostJson(WS_MG_PROMOTIONS_ID, requestMessage, authBundle);
        //    var obj = JsonConvert.DeserializeObject<PromotionsOut>(response.Message);
        //    SessionData.SaveAuthorization(authBundle, false);
        //    return obj;
        //}

        //public static async Task<PromotionsOutList> GetPromotionsByPromoType(AnfAuthorizationBundle authBundle, int promoType)
        //{
        //    // Build request message
        //    string requestMessage = JsonConvert.SerializeObject(new PromotionsIn() { PromoType = promoType, });

        //    //Call web service async
        //    var response = await Client.PostJson(Settings.PROMOTIONS_ENDPOINT, requestMessage, authBundle);
        //    var obj = JsonConvert.DeserializeObject<PromotionsOutList>(response.Message);
        //    SessionData.SaveAuthorization(authBundle, false);
        //    return obj;
        //}

		public static async Task<SetMyFarmOut> SetMyFarm(AnfAuthorizationBundle authBundle, string farmId)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new SetMyFarmIn()
			{
				FARMID = farmId,
			});

			// Call web service async
			var response = await Client.PostJson(WS_MG_SETPH, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<SetMyFarmOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			// Do not update the pharmacy!
			return obj;
		}

		//

        public static async Task<GetMyFarmDetailOut> GetMyFarmDetail(AnfAuthorizationBundle authBundle, string farmId)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(new GetMyFarmDetailIn()
            {
                    FarmId = farmId
            });

            // Call web service async
            var response = await Client.PostJson(WS_MG_GETPHDETAIL, requestMessage, authBundle);
            var obj = JsonConvert.DeserializeObject<GetMyFarmDetailOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			// Do not update the pharmacy!
			return obj;
        }

		#region Search

		public static async Task<SearchOut> SearchCategory(
			AnfAuthorizationBundle authBundle, 
			string farmId,
			int qty,
			int page,
			int filCat)
		{
			return await Search (authBundle, farmId, qty, null, 0, null, page, filCat);
		}

		public static async Task<SearchOut> Search(
			AnfAuthorizationBundle authBundle,
			string farmId,
			int qty,
			string key = null,
			int? cnpem = null,
			string activePrin = null,
			int? pageStart = null,
			// TODO: move this to a struct
			long? filCat = null,
			long? filPoints = null,
			long? filBrand = null,
			long? filDos = null,
			long? filFF = null,
			long? filPrice = null,

			long? ordertype = null,
			bool filCatalog = false,
			bool allProds = false
		)			 
		{
			// Build request message
			var requestObj = new SearchIn()
			{
				FarmID = farmId,
				Qty = qty,
				Key = key,
				CNPEM = cnpem,
				ActivePrin = activePrin,
				PageStart = pageStart,
				FilCat = filCat,
				FilPoints = filPoints,
				FilBrand = filBrand,
				FilDos = filDos,
				FilFF = filFF,
				FilPrice = filPrice,
				OrderType = ordertype,
				FilterPointsCatalogProducts = filCatalog ? "S" : "N",
				SearchAllProducts = allProds ? "S" : "N"
			};

			var url = WS_MG_SEARCH;
			HttpResponseModel response = null;
			try 
			{ 
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore}), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmID = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<SearchOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		// XXX: change this to SearchPAOut when the output is fixed.
		public static async Task<SearchPAOut> SearchPA(AnfAuthorizationBundle authBundle, string pa, string farmId = "0") 
		{
			string requestMessage = JsonConvert.SerializeObject (new SearchPAIn () 
				{
					ActivePrinciple = pa
				});

			var response = await Client.PostJson(WS_MG_SEARCHPA, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<SearchPAOut>(response.Message);
			SessionData.SaveAuthorization(authBundle);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<SearchCNPEMOut> SearchCNPEM(AnfAuthorizationBundle authBundle, string farmId, int cnpem)
		{
			var requestObj = new SearchCNPEMIn () 
			{
				FarmId = farmId,
				Code = cnpem
			};

			var url = WS_MG_SEARCHCNPEM;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<SearchCNPEMOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<SearchCNPEMOut> SearchCNPorCNPEM(AnfAuthorizationBundle authBundle, string farmId, int code)
		{
			var requestObj = new SearchCNPEMIn()
			{
				FarmId = farmId,
				Code = code
			};

			var url = WS_MG_SEARCHCNPorCNPEM;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<SearchCNPEMOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#endregion

		#region Products

		public static async Task<GetPAOut> GetPA(AnfAuthorizationBundle authBundle, int paid, int? dosId, string farmId) 
		{
			var requestObj = new GetPAIn () 
			{
				ActivePrincipleId = paid,
				FarmId = farmId,
				DosageId = dosId
			};

			var url = WS_MG_GETPA;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<GetPAOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<ProductDetailOut> ProdDetail(AnfAuthorizationBundle authBundle, string farmId, int cnp)
		{
			// Build request message
			var requestObj = new ProductDetailIn()
			{
                FarmId = farmId,
				CNP = cnp
			};

			var url = WS_MG_PRODDETAIL;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<ProductDetailOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<CategoriesOut> Categories(AnfAuthorizationBundle authBundle, long catId)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new CategoriesIn()
			{
				CatId = catId,
			});

			// Call web service async
			var response = await Client.PostJson(WS_MG_CATEGORIES, requestMessage, authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<CategoriesOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<CategoriesOut> PointsCatalogCategories(AnfAuthorizationBundle authBundle)
		{
			// Call web service async
			// XXX: empty response if the content is null... maybe some header is not set if null is passed?
			string requestMessage = JsonConvert.SerializeObject(null); // ...so that the correct content-type is added.  
			var response = await Client.PostJson(WS_MG_CATCATEGORIES, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<CategoriesOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<PointsCatalogProdsOut> PointsCatalogProds(AnfAuthorizationBundle authBundle, long catId)
		{
			// Build request message. Reuses the Categories in object.
			string requestMessage = JsonConvert.SerializeObject(new CategoriesIn()
				{
					CatId = catId,
				});

			// Call web service async
			var response = await Client.PostJson(WS_MG_CATPRODS, requestMessage, authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<PointsCatalogProdsOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);

			// We can't know for sure if the product is from the catalog just from the product fields. In theory, a product
			// without price but with points should be from the catalog...
			foreach (ProductOut p in obj.Products) {
				p.FromCatalog = true;
			}

			return obj;
		}
		public static async Task<ProductVoucherOut> ProdsVoucher(AnfAuthorizationBundle authBundle, string idVale, int qty,int rInicial)
		{
			// Build request message
			var requestObj = new ProductVoucherIn()
			{
				FarmId = SessionData.StorePharmacyId,
				idVale = idVale,
				Quantity = qty,
				registoInicial = rInicial

			};
			var url = WS_MG_GET_PROD_VOUCHER;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}
			catch (TaskCanceledException e)
			{
				return null;
			}
			catch (NetworkingException e) 
			{
				return null;
			}

			// Repeat the invocation in case of pharmacy change
		//	if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<ProductVoucherOut>(response.Message);

			SessionData.SaveAuthorization(authBundle, false);
			if (obj == null)  return null;



			return obj;

			//7	foreach (ProductOut product in obj)
			//	{
			/*	List<ProductOut> product = new List();
				var list = new ProductVoucherOut()
				{
					Products = product


				

		//	}

			/*var list = new ProductOut()
			{	
				list = 
			};*/
			// We can't know for sure if the product is from the catalog just from the product fields. In theory, a product
			// without price but with points should be from the catalog...


		}

		//ADDPRODSVOUCHER
		public static async Task<AddProductVoucherOut> AddProdsVoucher(AnfAuthorizationBundle authBundle,string idVale, int CNP)
		{
			// Build request message
			var requestObj = new AddProdVoucherIn()
			{
				FarmId = SessionData.StorePharmacyId,
				idVale = idVale,
				CNP = CNP

			};

			var url =  WS_MG_ADD_PROD_VOUCHER;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), authBundle);
				System.Diagnostics.Debug.WriteLine("RESPOSTA HTTP CONNECTION ------------------------ \n " + response.Message);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}
			catch (InvalidRequestException e)
			{
				var a = new AddProductVoucherOut();
				a.result = 400;
				return a;
			}

			// Repeat the invocation in case of pharmacy change
			//if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<AddProductVoucherOut>(response.Message);

			SessionData.SaveAuthorization(authBundle, false);

			return obj;
		}
		//WS_MG_ADD_PROD_VOUCHER


		public static async Task<FeaturedProdOut> FeaturedProd(AnfAuthorizationBundle authBundle, int quantity, bool catalog, long? order = null, int? start = null)
		{
			// Build request message
			var requestObj = new FeaturedProdIn()
			{
				FarmId = SessionData.StorePharmacyId,
				Quantity = quantity,
				Catalog = catalog,
				PageStart = start,
				OrderFilter = order
			};

			var url = WS_MG_FEATURED;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.FarmId = e.NewFarmID;
			}


			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			// Handle pharmacy change before returning the object.
			var obj = JsonConvert.DeserializeObject<FeaturedProdOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);

			// We can't know for sure if the product is from the catalog just from the product fields. In theory, a product
			// without price but with points should be from the catalog...
			foreach (ProductOut p in obj.Products) {
				p.FromCatalog = catalog;
			}

			return obj;
		}

		#endregion

		#region Favorites

		public static async Task<GetWishlistOut> GetWishlist(AnfAuthorizationBundle authBundle)
		{
			// Call web service async
			string requestMessage = JsonConvert.SerializeObject(null);
			var response = await Client.PostJson(WS_MG_GETWISHLIST, requestMessage, authBundle);

			var obj = JsonConvert.DeserializeObject<GetWishlistOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<MagentoOut> AddWishlistProd(AnfAuthorizationBundle authBundle, int cnp)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new AddWishlistProdIn()
				{
					CNP = cnp
				});

			// Call web service async
			var response = await Client.PostJson(WS_MG_ADDTOWISHLIST, requestMessage, authBundle);

			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<MagentoOut> ItemWishlistDelete(AnfAuthorizationBundle authBundle, int lineId)
		{
			System.Diagnostics.Debug.Assert (lineId != 0);

			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new ItemWishlistDeleteIn()
				{
					LineId = lineId
				});

			// Call web service async
			var response = await Client.PostJson(WS_MG_DELETEFROMWISHLIST, requestMessage, authBundle);

			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#endregion

		#region Basket

		/// <summary>
		/// Fetches the basket for the selected pharmacy.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<BasketOut> GetBasket(AnfAuthorizationBundle authBundle)
		{
			var requestObj = new BasketIn() 
			{
				PharmacyId = SessionData.StorePharmacyId
			};

			var url = WS_MG_BASKET_PRODUCTS;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<BasketOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<BasketAddDelOut> GetBasketQuantities(AnfAuthorizationBundle authBundle)
		{
			var requestObj = new BasketIn() 
			{
				PharmacyId = SessionData.StorePharmacyId
			};

			var url = WS_MG_BASKET_SUMMARY;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<BasketAddDelOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}


		/// <summary>
		/// Add a product to the basket.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <param name="product"></param>
		/// <returns></returns>
		public static async Task<BasketAddDelOut> AddProductToBasket(AnfAuthorizationBundle authBundle, ProductOut product, int quantity = 1, string aquisitionType = "V")
		{
			return await AddProductsToBasket(authBundle, new List<ProductOut>() { product }, quantity, aquisitionType);
		}

		/// <summary>
		/// Add N of a given product to the basket.
		/// </summary>
		/// 
		/// <param name="authBundle">Auth bundle.</param>
		/// <param name="cnp">Cnp.</param>
		/// <param name="cnpem">Cnpem.</param>
		/// <param name="quantity">Quantity.</param>
		/// <param name="hasCnpem">If set to <c>true</c> has cnpem.</param>
		public static async Task<BasketAddDelOut> AddProductToBasket(AnfAuthorizationBundle authBundle, int cnp, int? cnpem, int quantity = 1, bool hasCnpem = false, string aquisitionType = "V")
		{
			var requestProducts = new List<BasketProductIn>();	
			requestProducts.Add(new BasketProductIn() {
				CNP = cnp,
				CNPEM = hasCnpem ? cnpem : null,
				Type = hasCnpem ? "CNPEM" : "CNP",
				Quantity = quantity,
				AquisitionType = aquisitionType
			});

			return await AddProductsToBasket(authBundle, requestProducts);
		}

		/// <summary>
		/// Adds a list of products to the basket.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <param name="products"></param>
		/// <returns></returns>
		public static async Task<BasketAddDelOut> AddProductsToBasket(AnfAuthorizationBundle authBundle, List<ProductOut> products, int quantity = 1, string aquisitionType = "V")
		{
			var requestProducts = new List<BasketProductIn>();
			foreach (var prod in products)
			{
				requestProducts.Add(new BasketProductIn() {
					CNP = prod.CNP.HasValue && prod.CNP.Value > 0 ? prod.CNP : null,
					CNPEM = prod.HasCNPEM ? prod.CNPEM : null,
					Type = prod.HasCNPEM && !prod.HasCNP ? "CNPEM" : "CNP",
					Quantity = quantity,
					AquisitionType = aquisitionType
				});
			}

			// Call WS
			return await AddProductsToBasket(authBundle, requestProducts);
		}

		/// <summary>
		/// Adds a list of products to the basket.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <param name="products"></param>
		/// <returns></returns>
		public static async Task<BasketAddDelOut> AddProductsToBasket(AnfAuthorizationBundle authBundle, List<BasketProductIn> products)
		{
			// Build request
			var requestObj = new AddBasketProductsIn()
			{
				PharmacyId = SessionData.StorePharmacyId,
				Products = products
			};

			var url = WS_MG_ADD_BASKET_PRODUCT;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(
					url, 
					JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), 
					authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(
												url,
												JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), 
												authBundle);

			var obj = JsonConvert.DeserializeObject<BasketAddDelOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Edits a product in the basket.
		/// </summary>
		/// <returns></returns>
		public static async Task<MagentoOut> EditBasketProduct(AnfAuthorizationBundle authBundle, int id, int? quantity = null, int? aquisitionType = null)
		{
			var requestObj = new EditBasketProductIn()
			{
				PharmacyId = SessionData.StorePharmacyId,
				Id = id,
				Quantity = quantity,
				AquisitionType = aquisitionType
			};

			var url = WS_MG_EDIT_BASKET_PRODUCT;
			HttpResponseModel response = null;
			try
			{
				var requestMessage = JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

				// Call web service async
				response = await Client.PostJson(url, requestMessage, authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(
												url,
												JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }),
												authBundle);

			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Deletes a basket product.
		/// </summary>
		/// <returns></returns>
		public static async Task<BasketAddDelOut> DeleteBasketProduct(AnfAuthorizationBundle authBundle, int id)
		{
			var requestObj = new DeleteBasketProductIn()
			{
				PharmacyId = SessionData.StorePharmacyId,
				Id = id
			};

			var url = WS_MG_DELETE_BASKET_PRODUCT;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<BasketAddDelOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Clear all products from carts.
		/// </summary>
		/// <returns></returns>
		public static async Task<MagentoOut> ClearCart(AnfAuthorizationBundle authBundle)
		{
			var requestObj = new BasketIn()
			{
				PharmacyId = SessionData.StorePharmacyId,
			};

			var url = WS_MG_CLEAR_CART;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#region Checkout

		/// <summary>
		/// Start the checkout process.
		/// </summary>
		/// <param name="farmId"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<CheckoutStartOut> CheckoutStart(AnfAuthorizationBundle authBundle)
		{
			var requestObj = new CheckoutStartIn() 
			{
				PharmacyId = SessionData.StorePharmacyId
			};

			var url = WS_MG_CHECKOUT_START;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<CheckoutStartOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Sets the Checkout Payment Mode
		/// </summary>
		/// <param name="paymentMode"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> SetCheckoutPaymentMethod(string paymentMode, AnfAuthorizationBundle authBundle)
		{
			var requestObj = new CheckoutPaymentMethodIn()
			{
				Code = paymentMode
			};

			// Call WebService
			HttpResponseModel response = await Client.PostJson(WS_MG_CHECKOUT_PAYMENT, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Sets the Checkout Payment Mode
		/// </summary>
		/// <param name="paymentMode"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> SetCheckoutDeliveryMethod(string deliveryMethod, AnfAuthorizationBundle authBundle)
		{
			var requestObj = new CheckoutDeliveryMethodIn()
			{
				Code = deliveryMethod
			};

			// Call WebService
			HttpResponseModel response = await Client.PostJson(WS_MG_CHECKOUT_DELIVERY, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#region Vouchers

		/// <summary>
		/// Add a Voucher to Checkout
		/// </summary>
		/// <param name="paymentMode"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> AddVoucherToCheckout(string voucherCode, AnfAuthorizationBundle authBundle)
		{
			var requestObj = new AddVoucherToCheckoutIn()
			{
				Id = voucherCode
			};

			// Call WebService
			HttpResponseModel response = await Client.PostJson(WS_MG_CHECKOUT_ADD_VOUCHER, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}
			
		/// <summary>
		/// Remove a Voucher from the Checkout
		/// </summary>
		///
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> DeleteVoucherFromCheckout(string voucherCode, AnfAuthorizationBundle authBundle)
		{
			var requestObj = new AddVoucherToCheckoutIn()
			{
				Id = voucherCode
			};

			// Call WebService
			HttpResponseModel response = await Client.PostJson(WS_MG_CHECKOUT_DELETE_VOUCHER, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// List Vouchers from the Checkout
		/// </summary>
		///
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<GetListVouchersOut> GetListVouchers(AnfAuthorizationBundle authBundle)
		{
			// Call WebService
			HttpResponseModel response = await Client.PostJson(WS_MG_CHECKOUT_LIST_VOUCHER, JsonConvert.SerializeObject(null), authBundle);
			var obj = JsonConvert.DeserializeObject<GetListVouchersOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#endregion

		/// <summary>
		/// Finish the checkout process.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<CheckoutConfOut> CheckoutConf(AnfAuthorizationBundle authBundle, string MBWAYTEL)
		{
			CheckoutConfIn requestObj;
			if (string.IsNullOrEmpty(MBWAYTEL))
			{
				requestObj = new CheckoutConfIn()
				{
					PharmacyId = SessionData.StorePharmacyId,
				};
			}
			else
			{
				requestObj = new CheckoutConfIn()
				{
					PharmacyId = SessionData.StorePharmacyId,
					MBWAYPhone = MBWAYTEL
				};
			}


			var url = WS_MG_CHECKOUT_CONF;
			HttpResponseModel response = null;
			try
			{
				// Call web service async
				response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);
			}
			catch (PharmacyChangedException e)
			{
				UpdatePharmacy(e.NewFarmID);
				requestObj.PharmacyId = e.NewFarmID;
			}

			// Repeat the invocation in case of pharmacy change
			if (response == null) response = await Client.PostJson(url, JsonConvert.SerializeObject(requestObj), authBundle);

			var obj = JsonConvert.DeserializeObject<CheckoutConfOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Sets a shipping address 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> SetShippingAddress(AddressOut address, AnfAuthorizationBundle authBundle)
		{
			return await SetCheckoutAddresses(new List<AddressOut>() { address }, null, authBundle);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> SetBillingAddress(AddressOut address, AnfAuthorizationBundle authBundle)
		{
			return await SetCheckoutAddresses(null, new List<AddressOut>() { address }, authBundle);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<MagentoOut> SetCheckoutAddresses(List<AddressOut> shippingAddresses, List<AddressOut> billingAddresses, AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(new CheckoutAddressIn()
			{
				BillingAddresses = billingAddresses,
				ShippingAddresses = shippingAddresses
			}, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			
			var response = await Client.PostJson(WS_MG_CHECKOUT_SETADDRESS, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}
			
		public static async Task<MagentoOut> PutImageInCart(PhotoFile file, AnfAuthorizationBundle authBundle)
		{
			byte[] bytes;
			Stream input = file.File.Source;

			// WPHone only.. just to be safe
			if (Device.OS == TargetPlatform.WinPhone)
			{
				input.Seek(0, SeekOrigin.Begin);
			}

			byte[] buffer = new byte[16*1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				bytes = ms.ToArray();
			}

			// Compress image, because Magento and Sifarma have a 2MB hard limit
			if (bytes != null) bytes = DependencyService.Get<IBitmapUtils>().ResizeImage(bytes, 600, 600);

			var requestObj = new PutImageInCartIn()
			{
				Encoded = Convert.ToBase64String(bytes),
				MimeType = file.MimeType,
				ImageName = file.Filename,
				Type = 1 // 1- Prescription, 2 - ?, 3 - ?
			};

			var response = await Client.PostJson(WS_MG_CHECKOUT_PUTIMAGE, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<MagentoOut> DelImageInCart(string filename, AnfAuthorizationBundle authBundle)
		{
			var requestObj = new DelImageInCartIn()
			{
				ImageName = filename,
			};

			var response = await Client.PostJson(WS_MG_CHECKOUT_DELIMAGE, JsonConvert.SerializeObject(requestObj), authBundle);
			var obj = JsonConvert.DeserializeObject<MagentoOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#endregion

		#endregion

        /// <summary>
        /// Get user Orders.
        /// </summary>
        /// <param name="authBundle"></param>
        /// <returns></returns>
        public static async Task<UserOrderOut> GetOrders(AnfAuthorizationBundle authBundle)
        {
            string requestMessage = JsonConvert.SerializeObject(new GetOrdersIn() { IncludeDetail = "N" });
            var response = await Client.PostJson(WS_MG_GET_ORDERS, requestMessage,authBundle);
            var obj = JsonConvert.DeserializeObject<UserOrderOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
        }

		/// <summary>
		/// Get user Orders.
		/// </summary>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<UserOrderOut> GetOrderDetail(string orderNumber, AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(new GetOrderDetailIn() { OrderNumber = orderNumber , IncludeDetail = "S" });
			var response = await Client.PostJson(WS_MG_GET_ORDERS, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<UserOrderOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		/// <summary>
		/// Add to the cart the products from the referenced order
		/// </summary>
		/// <returns>The reorder.</returns>
		/// <param name="orderNumber">Order number.</param>
		/// <param name="authBundle">Auth bundle.</param>
		public static async Task<ReorderOut> PostReorder(string orderNumber, AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(new PostOrderIn() { OrderNumber = orderNumber });
			var response = await Client.PostJson(WS_MG_POST_REORDER, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<ReorderOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		#region Banners

		public static async Task<GetBannersOut> GetBanners(int zid, int width, int? height, int qty, BannerFilter filterType, int filterId, AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(new GetBannersIn()
				{
					ZoneId = zid,
					Width = width,
					Height = height,
					Quantity = qty,
					FilterType = (int)filterType,
					FilterId = filterId,
				}, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			var response = await Client.PostJson(WS_MG_GET_BANNERS, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<GetBannersOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;
		}

		public static async Task<GetBannersOut> GetSAFEBanners(int zid, int width, int? height, int qty, BannerFilter filterType, int filterId, Nullable<int> districtId, AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(new GetSAFEBannersIn() {
				ZoneId = zid,
				Width = width,
				Height = height,
				Quantity = qty,
				FilterType = (int)filterType,
				FilterId = filterId,
				DistrictId = districtId
			}, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			var response = await Client.PostJson(WS_MG_GET_SAFE_BANNERS, requestMessage, authBundle);
			var obj = JsonConvert.DeserializeObject<GetBannersOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			UpdatePharmacy(obj.FarmId);
			return obj;	
		}


		public static async Task<SAFEDistrictOut> GetSAFEDistrict(double lat, double lon, AnfAuthorizationBundle authBundle) 
		{
			var url = string.Format(WS_MG_GET_SAFE_DISTRICT, lat, lon).Replace(',', '.');
			var response = await Client.Get(url);
			var obj = JsonConvert.DeserializeObject<SAFEDistrictOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			return obj;
		}

		#endregion

		#region Pharmacy Services (global)

		public static async Task<PharmacyServicesOut> ListPharmacyServices(AnfAuthorizationBundle authBundle)
		{
			string requestMessage = JsonConvert.SerializeObject(null);
			var response = await Client.PostJson(WS_MG_GET_PHARMACY_SERVICES, requestMessage,authBundle);
			var obj = JsonConvert.DeserializeObject<PharmacyServicesOut>(response.Message);
			SessionData.SaveAuthorization(authBundle, false);
			return obj;
		}

		#endregion
	}
}

