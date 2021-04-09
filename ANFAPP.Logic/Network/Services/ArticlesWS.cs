using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services.Abstract;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Models.In.Articles;
using ANFAPP.Logic.Models.Authorization;
using System.Collections.Generic;

namespace ANFAPP.Logic.Network.Services
{
    public class ArticlesWS : WordpressWS
    {
        public static async Task<ArticlesHighlightOut> GetArticlesHighlights(AnfAuthorizationBundle authBundle,int quantity)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new ArticlesHighlightIn()
			{
				QTY = quantity
			});

			// Call web service async
            var response = await Client.PostJson(WS_WORDPRESS_HIGHLIGHTS, requestMessage, authBundle);
            return JsonConvert.DeserializeObject<ArticlesHighlightOut>(response.Message);
		}

        public static async Task<ArticleOut> GetArticle(AnfAuthorizationBundle authBundle, int id)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(new ArticleIn()
            {
                ID = id
            });

            // Call web service async
            var response = await Client.PostJson(WS_WORDPRESS_GET_ARTICLE, requestMessage, authBundle);
            return JsonConvert.DeserializeObject<ArticleOut>(response.Message);
        }

        public static async Task<GetSectionsOut> GetSections(AnfAuthorizationBundle authBundle, int catId)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(new CategoriesIn()
            {
                CATID = catId
            });

            // Call web service async
            var response = await Client.PostJson(WS_WORDPRESS_GET_CATEGORIES, requestMessage, authBundle);
            return JsonConvert.DeserializeObject<GetSectionsOut>(response.Message);
        }
      
        public static async Task<SearchArticlesOut> SearchCategory(
        AnfAuthorizationBundle authBundle,
        int catId,
        int qty,
             int page,
        string text
       )
        {

            return await Search(authBundle, catId, qty,  page, text);
        }

        public static async Task<SearchArticlesOut> Search(
            AnfAuthorizationBundle authBundle,
            int catId,
            int qty,
            int page,
            string key = null
            )
        {
            // Build request message
            string requestMessage = "";
            if (catId != -1)
            {
                requestMessage = JsonConvert.SerializeObject(new SearchCategoriesIn()
                {

                    QTY = qty,
                    KEY = key,
                    CATID = catId,
                    RINICIAL=page

                });
            }
            else
            {
                requestMessage = JsonConvert.SerializeObject(new SearchArticlesIn()
                {

                    QTY = qty,
                    SEARCHSTRING = key,
                    RINICIAL=page

           

                });
            }

            // Call web service async

            if ( catId!= -1)
            { 
                var response = await Client.PostJson(WS_WORDPRESS_GET_CATEGORIES_ARTICLES, requestMessage, authBundle); 
                var obj = JsonConvert.DeserializeObject<SearchArticlesOut>(response.Message);
                return obj;
            }
            else 
            { 
                var response = await Client.PostJson(WS_WORDPRESS_GET_SEARCH_ARTICLES, requestMessage, authBundle); 
                var obj = JsonConvert.DeserializeObject<SearchArticlesOut>(response.Message);
                return obj;
            }
            

       
            
  

            
        }
    
    }
}