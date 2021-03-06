using System.Collections.Generic;
using ANFAPP.Logic.EventHandlers;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Network.Services;
using System;



namespace ANFAPP.Logic.ViewModels
{
    public class ArticlesHighlightViewModel : IViewModel
    {
        #region Properties
        protected static readonly int DEFAULT_PAGE_SIZE = 10;
		private ArticlesHighlightOut _articlesHighlights;
        public ArticlesHighlightOut ArticlesHighlights
        {
            get { return _articlesHighlights; }
            set
            {
                _articlesHighlights = value;
                OnPropertyChanged();
				OnPropertyChanged ("MainArticle");
				OnPropertyChanged ("HasHighlights");
            }
        }
			 
        public HighlightOut MainArticle
        {
			get { return _articlesHighlights != null ? _articlesHighlights.MainArticle : null; }
        }
			
        public bool HasHighlights
        {
			get { return _articlesHighlights != null ? _articlesHighlights.HasHighlights : false; }
        }

        #endregion

        /// <summary>
        /// Load the list of entries from the database.
        /// </summary>
        public async void LoadData()
        {
            if (null != OnLoadStart) await OnLoadStart();

            try
            {
                var result = await ArticlesWS.GetArticlesHighlights(SessionData.UserAuthentication, DEFAULT_PAGE_SIZE);
                ArticlesHighlights = result;    
              
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(null, e.Message);
            }

            if (OnSuccess!=null) OnSuccess();
        }

        #region Event Handlers

        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnSuccess;
        public OnErrorEventHandler OnError;

        #endregion
    }
}
