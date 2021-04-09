using System;
using ANFAPP.Logic.Network.Invokers;

namespace ANFAPP.Logic.Network.Services.Abstract
{
    public abstract class WordpressWS : MagentoWS
    {
        #region Endpoints
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
   
    }
}