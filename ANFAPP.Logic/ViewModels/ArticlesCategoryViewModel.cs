using System.Collections.Generic;
using ANFAPP.Logic.EventHandlers;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.ObjectModel;



namespace ANFAPP.Logic.ViewModels
{
    public class ArticlesCategoryViewModel : IViewModel
    {
        #region Properties

        class CategoryNode
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public Collection<SectionsOut> Categories { get; set; }
			public SectionsOut Context { get; set; }
        }

        private ObservableCollection<SectionsOut> _categories;
        public ObservableCollection<SectionsOut> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

		private SectionsOut _current;
		public SectionsOut Current
		{
			get { return _current; }
			set
			{
				_current = value;
				OnPropertyChanged();
				OnPropertyChanged("HasHighlightsOverflow");
			}
		}

		public bool HasHighlightsOverflow
		{
			get
			{
				return (Current != null && Current.HasMore);
			}
		}

        private int _catId;
        public int CatId
        {
            get
            {
                return _catId;
            }
            set
            {
                _catId = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name 
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private Stack<CategoryNode> _nav;
        public int NavCount { get { return _nav == null ? 0 : _nav.Count; } }

        public string _selectedCategoryName = "";
        public string SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
                OnPropertyChanged();
            }
        }


        public ArticlesCategoryViewModel()
            : base()
        {
            CatId = 0;
        }

        public ArticlesCategoryViewModel(int catId)
            : base()
        {
            CatId = catId;
        }



        private int _selectedCategoryId;
        public int SelectedCategoryId 
        {
            get 
            {
                return _selectedCategoryId;   
            }
            set 
            {
                _selectedCategoryId = value;
                OnPropertyChanged();
            }
        }

        #endregion


        /// <summary>
        /// Load the list of categories from WS
        /// </summary>
        private async Task LoadData(int catId, string catName, CategoryNode current)
        {
            if (OnLoadStart != null) await OnLoadStart();

            try
            {
                var result = await ArticlesWS.GetSections(SessionData.UserAuthentication, catId);

                // Append this category as the last node, which will be used as a shortcup to all the products 
                // under the current category.
                result.CategoriesList.Insert(0, new SectionsOut
                {
                    Name = "Destaques",
                    Id = -2,
                    LastSection = true
                });

                Categories = new ObservableCollection<SectionsOut>(result.CategoriesList);
                SelectedCategoryId = catId;
                SelectedCategoryName = catName;

                if (current != null)
                    _nav.Push(current);
            }
            catch (Exception ex)
            {
                if (OnError != null) OnError("", ex.Message);
            }
   

            if (OnSuccess != null)
                OnSuccess();
        }

        //public async Task SetCategory(int catId = 0, string catName = "Todas")
		public async Task SetCategory(SectionsOut selected = null)
        {
			int catId = (selected != null) ? selected.Id : 0;
			string catName = (selected != null) ? selected.Name : "Todas";
			
            if (Categories != null && SelectedCategoryId == catId)
                return;

            if (null == _nav)
                _nav = new Stack<CategoryNode>();

            CategoryNode node = null;
            if (Categories != null)
            {
                node = new CategoryNode
                {
                    Name = SelectedCategoryName,
                    Id = SelectedCategoryId,
                    Categories = Categories,
					Context = selected,
                };
            }

            await LoadData(catId, catName, node);

			Current = selected;
        }

        public async Task<int> PopCategory()
        {
            if (OnLoadStart != null) await OnLoadStart();

			var prevCount = _nav.Count;
			if (prevCount > 0)
            {
                CategoryNode node = _nav.Pop();
                Categories = new ObservableCollection<SectionsOut>(node.Categories);
                SelectedCategoryId = node.Id;
                SelectedCategoryName = node.Name;

				Current = _nav.Count > 0 ? _nav.Peek().Context : null;
            }

            if (OnSuccess != null) OnSuccess();

			return prevCount;
        }

        #region Event Handlers

        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnSuccess;
        public OnErrorEventHandler OnError;

        #endregion

    }
}

