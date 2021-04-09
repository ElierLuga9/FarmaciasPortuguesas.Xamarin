using System.Collections.Generic;
using ANFAPP.Logic.EventHandlers;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Network.Services;
using System;



namespace ANFAPP.Logic.ViewModels
{
    public class ArticleDetailViewModel : IViewModel
    {
        #region Properties

        private ArticleOut _articledetail;
        public ArticleOut ArticleDetail
        {
            get { return _articledetail; }
            set
            {
                _articledetail = value;
                OnPropertyChanged();
            }
        }

        private int _id;
        public int Id
        {
            get 
            {
                return _id;
            }
            set 
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public ArticleDetailViewModel(int articleId) : base() 
        {
            Id = articleId;   
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
                var result = await ArticlesWS.GetArticle(SessionData.UserAuthentication, Id);
                ArticleDetail = result;
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(null, e.Message);
            }

            if (OnSuccess != null) OnSuccess();
        }

        #region Event Handlers

        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnSuccess;
        public OnErrorEventHandler OnError;

        #endregion
    }
}
