using System;
using ANFAPP.Logic.Network.Invokers;

namespace ANFAPP.Logic.Network.Services.Abstract
{
	public abstract class MagentoWS
	{

		#region Endpoints

		/// <summary>
		/// The Magento Login endpoint.
		/// </summary>
		public static readonly string WS_MG_LOGIN = Settings.ENDPOINT_MAGENTO + "Login";

		/// <summary>
		/// The Magento SetMyFarm endpoint.
		/// </summary>
		public static readonly string WS_MG_SETPH = Settings.ENDPOINT_MAGENTO + "SetMyFarm";

		/// <summary>
		/// The Magento SetMyFarm endpoint.
		/// </summary>
		public static readonly string WS_MG_SETFAVPH = Settings.ENDPOINT_MAGENTO + "SetFavoriteFarm";
		/// <summary>
		/// The Magento GetMyFarmDetail endpoint.
		/// </summary>
		public static readonly string WS_MG_GETPHDETAIL = Settings.ENDPOINT_MAGENTO + "GetMyFarmDetail";

		/// <summary>
		/// The Magento Search endpoint.
		/// </summary>
		public static readonly string WS_MG_SEARCH = Settings.ENDPOINT_MAGENTO + "Search";

		public static readonly string WS_MG_SEARCHPA = Settings.ENDPOINT_MAGENTO + "SearchPA";

		public static readonly string WS_MG_GETPA = Settings.ENDPOINT_MAGENTO + "GetPA";

		public static readonly string WS_MG_SEARCHCNPEM = Settings.ENDPOINT_MAGENTO + "SearchCNPEM";

		public static readonly string WS_MG_SEARCHCNPorCNPEM = Settings.ENDPOINT_MAGENTO + "SearchProdRecMed";

		/// <summary>
		/// The Magento endpoint to get a product detail.
		/// </summary>
		public static readonly string WS_MG_PRODDETAIL = Settings.ENDPOINT_MAGENTO + "ProdDetail";

		/// <summary>
		/// The Magento categories list.
		/// </summary>
		public static readonly string WS_MG_CATEGORIES = Settings.ENDPOINT_MAGENTO + "Categories";

        //testes
        /// <summary>
        /// The Magento promotions list.
        /// </summary>
        public static readonly string WS_MG_PROMOTIONS = Settings.ENDPOINT_MAGENTO + "GetPromotions";

        /// <summary>
        /// The Magento promotions list by id.
        /// </summary>
        public static readonly string WS_MG_PROMOTIONS_ID = Settings.ENDPOINT_MAGENTO + "GetPromotionsById";

        /// <summary>
        /// The Magento promotions list by promotion type.
        /// </summary>
        public static readonly string WS_MG_PROMOTIONS_TYPE = Settings.ENDPOINT_MAGENTO + "GetPromotionsByPromoType";
        

		/// <summary>
		/// The Magento endpoint to get the categories from the points catalog.
		/// </summary>
		public static readonly string WS_MG_CATCATEGORIES = Settings.ENDPOINT_MAGENTO + "PointsCatalogCategories";

		/// <summary>
		/// The Magento endpoint to get the products with points.
		/// </summary>
		public static readonly string WS_MG_CATPRODS = Settings.ENDPOINT_MAGENTO + "PointsCatalogProds";

		/// <summary>
		/// The Magento web service that lists featured products.
		/// </summary>
		public static readonly string WS_MG_FEATURED = Settings.ENDPOINT_MAGENTO + "FeaturedProd";

		/// <summary>
		/// The Magento endpoint that lists a user favorite prodcuts.
		/// </summary>
		public static readonly string WS_MG_GETWISHLIST = Settings.ENDPOINT_MAGENTO + "GetWishlist";

		/// <summary>
		/// The Magento endpoint to clear a user favorite list.
		/// </summary>
		public static readonly string WS_MG_CLEARWISHLIST = Settings.ENDPOINT_MAGENTO + "ClearWishlist";

		/// <summary>
		/// The Magento endpoint to add a product to a user wishlist.
		/// </summary>
		public static readonly string WS_MG_ADDTOWISHLIST = Settings.ENDPOINT_MAGENTO + "AddWishlistProd";

		/// <summary>
		/// The Magento endpoint to delete a product form a user wishlist.
		/// </summary>
		public static readonly string WS_MG_DELETEFROMWISHLIST = Settings.ENDPOINT_MAGENTO + "ItemWishlistDelete";

		/// <summary>
		/// Add a product to the basket.
		/// </summary>
		public static readonly string WS_MG_ADD_BASKET_PRODUCT = Settings.ENDPOINT_MAGENTO + "AddProductsCart";

		/// <summary>
		/// Edit a product from the basket.
		/// </summary>
		public static readonly string WS_MG_EDIT_BASKET_PRODUCT = Settings.ENDPOINT_MAGENTO + "ItemCartEdit";

		/// <summary>
		/// Remove a product from the basket.
		/// </summary>
		public static readonly string WS_MG_DELETE_BASKET_PRODUCT = Settings.ENDPOINT_MAGENTO + "ItemCartDelete";

		/// <summary>
		/// Remove all products from the basket.
		/// </summary>
		public static readonly string WS_MG_CLEAR_CART = Settings.ENDPOINT_MAGENTO + "ClearCart";

		/// <summary>
		/// Get the basket.
		/// </summary>
		public static readonly string WS_MG_BASKET_PRODUCTS = Settings.ENDPOINT_MAGENTO + "ItemsCart";

		/// <summary>
		/// Get the number of products in the basket.
		/// </summary>
		public static readonly string WS_MG_BASKET_SUMMARY = Settings.ENDPOINT_MAGENTO + "MiniCart";

		/// <summary>
		/// Checkout start.
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_START = Settings.ENDPOINT_MAGENTO + "CheckoutStart";

		/// <summary>
		/// Checkout end.
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_CONF = Settings.ENDPOINT_MAGENTO + "CheckoutConf";

		/// <summary>
		/// Checkout - Set Delivery Mode
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_DELIVERY = Settings.ENDPOINT_MAGENTO + "CheckoutSetModoEntrega";

		/// <summary>
		/// Checkout - Set Payment Mode
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_PAYMENT = Settings.ENDPOINT_MAGENTO + "CheckoutSetMeioPagamento";

		/// <summary>
		/// Checkout - Add Voucher
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_ADD_VOUCHER = Settings.ENDPOINT_MAGENTO + "AddVoucher";

		/// <summary>
		/// Checkout - Delete a Voucher during the checkout.
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_DELETE_VOUCHER = Settings.ENDPOINT_MAGENTO + "DeleteVoucher";

		/// <summary>
		/// Checkout - List vouchers.
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_LIST_VOUCHER = Settings.ENDPOINT_MAGENTO + "GetListVouchers";

        /// <summary>
        /// Get user Orders.
        /// </summary>
        public static readonly string WS_MG_GET_ORDERS = Settings.ENDPOINT_MAGENTO + "GetOrders";


        /// <summary>
        /// Get user PRODUCTS VOUCHER.
        /// </summary>
        public static readonly string WS_MG_GET_PROD_VOUCHER = Settings.ENDPOINT_MAGENTO + "GetProdsVoucher";

		/// <summary>
		/// Get user PRODUCTS VOUCHER.
		/// </summary>
		public static readonly string WS_MG_ADD_PROD_VOUCHER = Settings.ENDPOINT_MAGENTO + "AddProdVoucher";

		/// <summary>
		/// Add to the cart the products from the referenced order
		/// </summary>
		public static readonly string WS_MG_POST_REORDER = Settings.ENDPOINT_MAGENTO + "reorder";

		/// <summary>
		/// WS that sets the billing and/or the delivery address for a checkout.
		/// </summary>
		public static readonly string WS_MG_CHECKOUT_SETADDRESS = Settings.ENDPOINT_MAGENTO + "CheckoutSetAddress";

		public static readonly string WS_MG_CHECKOUT_PUTIMAGE = Settings.ENDPOINT_MAGENTO + "PutImageInCart";

		public static readonly string WS_MG_CHECKOUT_DELIMAGE = Settings.ENDPOINT_MAGENTO + "DelImageInCart";

		public static readonly string WS_MG_GET_BANNERS = Settings.ENDPOINT_MAGENTO + "GetBanners";

		public static readonly string WS_MG_GET_SAFE_BANNERS = Settings.ENDPOINT_MAGENTO + "GetBannersV2";

		public static readonly string WS_MG_GET_SAFE_DISTRICT = Settings.ENDPOINT_MAGENTO + "pointlocationws.php?lat={0}&long={1}"; //"http://work.innovagencyhost.com/safe/ws/pointlocationws.php?lat={0}&long={1}"; //

		public static readonly string WS_MG_GET_PHARMACY_SERVICES = Settings.ENDPOINT_MAGENTO + "wsListServicos";

		/// <summary>
		/// The default pharmacy identifier.
		/// </summary>
		public static readonly string WS_MG_DEFAULT_FARMID = Settings.ST_MG_PHARMACY_ID_DEFAULT;

        #region Wordpress
        /// <summary>
        /// Get Dashboard Highlights 
        /// </summary>
        public static readonly string WS_WORDPRESS_HIGHLIGHTS = Settings.ENDPOINT_MAGENTO + "wpDestaquesEntrada";
        /// <summary>
        /// Get Full Article
        /// </summary>
        public static readonly string WS_WORDPRESS_GET_ARTICLE = Settings.ENDPOINT_MAGENTO + "wpGetArticle";
        /// <summary>
        /// Get Available Categories 
        /// </summary>
        public static readonly string WS_WORDPRESS_GET_CATEGORIES = Settings.ENDPOINT_MAGENTO + "wpGetCategories";
        /// <summary>
        /// Get All Articles from Category 
        /// </summary>
        public static readonly string WS_WORDPRESS_GET_CATEGORIES_ARTICLES = Settings.ENDPOINT_MAGENTO + "wpGetCategoryArticles";
        /// <summary>
        /// Get Search Result 
        /// </summary>
        public static readonly string WS_WORDPRESS_GET_SEARCH_ARTICLES = Settings.ENDPOINT_MAGENTO + "wpSearchArticles";
        #endregion

		#endregion

        #region Constants

        /// <summary>
        /// Magento WS Code for success.
        /// </summary>
        public static readonly int CODE_MG_SUCCESS = 200;

        /// <summary>
        /// Magento WS Code for created.
        /// </summary>
        public static readonly int CODE_MG_CREATED = 210;

        /// <summary>
        /// Magento WS Code for not session pharmacy.
        /// </summary>
        public static readonly int CODE_MG_ERR_PHSESSION = 415;

        #endregion

        #region Web Clients
        //is this needed since this is a non authenticated service?
        protected static RestInvoker Client { get { return MagentoRestClient.Instance; } }

        #endregion

		#region Session Updates

		/// <summary>
		/// Update the session pharmacy
		/// </summary>
		/// <param name="pharmacyId"></param>
		protected static void UpdatePharmacy(string pharmacyId)
		{
			if (string.IsNullOrEmpty(pharmacyId)) return;

			if (!string.Equals(SessionData.StorePharmacyId, pharmacyId))
			{
				SessionData.UpdatePharmacy(pharmacyId);
			}
		}
		#endregion

	}
}
