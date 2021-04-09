using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using ANFAPP.Logic.EventHandlers;

using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
    public class ArticlesSearchViewModel : IViewModel
    {
        #region Constants

        protected static readonly int DEFAULT_PAGE_SIZE = 10;

        #endregion

        #region Inner Classes

        public class SearchFilter
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public SearchFilter(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public class ArticleGroup : ObservableCollection<HighlightOut>
        {
            public string Description { get; set; }

            public ArticleGroup(string description)
            {
                Description = description;
            }
        }


        #endregion

        #region Unbindable Properties

        public bool InvalidSearchResults = true;
        public bool IsLoading = false;

        #endregion

        #region Properties

        #region Search Filters

        private string _searchValue;
        public string SearchValue
        {
            get { return _searchValue; }
            set
            {
                _searchValue = value;

                InvalidSearchResults = true;
                OnPropertyChanged();
            }
        }

        #endregion

        private ArticleGroup _searchResults;
        public ArticleGroup SearchResults
        {
            get { return _searchResults; }
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ArticleGroup> _searchGroups;
        public ObservableCollection<ArticleGroup> SearchGroups
        {
            get { return _searchGroups; }
            set
            {
                _searchGroups = value;
                OnPropertyChanged();
            }
        }

        private int _totalResults;
        public int TotalResults
        {
            get { return _totalResults; }
            set
            {
                _totalResults = value;
                OnPropertyChanged();
            }
        }

        private int _catId;
        public int CatId
        {
            get { return _catId; }
            set 
            {
                _catId = value;
                OnPropertyChanged();
            }
        
        }
        private string _catName;
        public string CategoryName
        {
            get 
            {
                return _catName;
            }
            set 
            {
                _catName = value;
                OnPropertyChanged();
            }

        }
        public bool hasResults 
        {
            get 
            {
                return (SearchResults == null || SearchResults.Count == 0 || TotalResults == 0)? false : true;
            }
        }

        public bool HasMore
        {
            get
            {
                if (_searchResults == null || _searchResults.Count == 0 || TotalResults == 0) return false;
                return _searchResults.Count < TotalResults;
            }
        }
        #endregion
        #region Events

        public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Constructors

		public ArticlesSearchViewModel()
		{
	        
		}

        public ArticlesSearchViewModel(string searchValue, int CategoryId, string CatName )
            : this()
		{
			SearchValue = searchValue;
            CatId = CategoryId;
            CategoryName = CatName;
		}

		#endregion

        #region Search

        /// <summary>
        /// Perform a new search.
        /// </summary>
        public async void PerformSearch()
        {
            if (OnLoadStart != null) await OnLoadStart();
            IsLoading = true;

            // Reset the search

            SearchGroups = new ObservableCollection<ArticleGroup>();
            SearchGroups.Add(SearchResults = BuildProductGroup());

            TotalResults = 0;
            OnPropertyChanged("HasMore");

            // Set search results as valid
            InvalidSearchResults = false;

            // Call Webservice
            SearchArticlesOut result = null;
            try
            {
                result = await InvokeSearch();
                if (result == null || result.Articles == null || result.Articles.Count == 0)
                {
                    if (OnLoadSuccess != null) OnLoadSuccess();
                    return;
                }
            }
            catch (Exception e)
            {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
                IsLoading = false;
                return;
            }

            // Init Articles
            TotalResults = result.Results;
            foreach (var prod in result.Articles)
            {
                SearchResults.Add(prod);
            }

            // Set search results as valid
            IsLoading = false;
            InvalidSearchResults = false;
            OnPropertyChanged("HasMore");
            if (OnLoadSuccess != null) OnLoadSuccess();
        }

        public async void LoadNextPage()
        {
            IsLoading = true;

            // Call Webservice
            SearchArticlesOut result = null;
            try
            {
                result = await InvokeSearch();
            }
            catch (Exception ex)
            {
                IsLoading = false;
                return;
            }

            if (result == null || result.Articles == null || result.Articles.Count == 0) return;

            // Add products
            foreach (var prod in result.Articles)
            {
                SearchResults.Add(prod);
            }

            IsLoading = false;
            OnPropertyChanged("HasMore");
        }

        protected virtual ArticleGroup BuildProductGroup()
        {
            if (CatId == -1) { return new ArticleGroup(SearchValue); } 
            return new ArticleGroup(CategoryName);             
        }

        protected virtual async Task<SearchArticlesOut> InvokeSearch()
        {
           // var page = TotalResults / DEFAULT_PAGE_SIZE - (TotalResults - SearchResults.Count) / DEFAULT_PAGE_SIZE;
            var page = SearchResults.Count;
            var result=new SearchArticlesOut();
            if (CatId ==-1) 
            { 
                result = await ArticlesWS.Search(SessionData.UserAuthentication,-1, DEFAULT_PAGE_SIZE,  page,SearchValue); 
            }
            else
            {
                result = await ArticlesWS.SearchCategory(SessionData.UserAuthentication, CatId, DEFAULT_PAGE_SIZE,  page,SearchValue);
            }
            return result;
        }

        #endregion
    }
}
