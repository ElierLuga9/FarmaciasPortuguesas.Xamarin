using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.EventHandlers;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StorePointsCatalogViewModel : IViewModel
	{
		class CategoryNode {
			public string Name { get; set; }
			public int Id { get; set; }
			public Collection<CategoryOut> Categories { get; set; }
		}

		private Stack<CategoryNode> _nav;
		public int NavCount { get { return _nav == null ? 0 : _nav.Count; }}

		public string _selectedCategoryName = "";
		public string SelectedCategoryName
		{
			get { return _selectedCategoryName; }
			set { 
				_selectedCategoryName = value;
				OnPropertyChanged ("SelectedCategoryName");
			}

		}

		public int _selectedCategoryId;
		public int SelectedCategoryId
		{
			get { return _selectedCategoryId; }
			set { 
				_selectedCategoryId = value;
				OnPropertyChanged ("SelectedCategoryId");
			}

		}

		private ObservableCollection<CategoryOut> _categories { get; set; }
		public ObservableCollection<CategoryOut> Categories 
		{ 
			get { return _categories; }
			set 
			{ 
				_categories = value; 
				OnPropertyChanged("Categories");
			}
		}

		public bool IsLoading { get; set; }

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		public async Task<int> PopCategory()
		{
			if (OnLoadStart != null) await OnLoadStart();

			if (_nav.Count > 0) {
				CategoryNode node = _nav.Pop ();
				Categories = new ObservableCollection<CategoryOut> (node.Categories);
				SelectedCategoryId = node.Id;
				SelectedCategoryName = node.Name;
			}

			if (OnLoadSuccess != null) OnLoadSuccess();

			return _nav.Count;
		}

		public async Task SetCategory(int catId = 0, string catName = "Todas") 
		{
			if (Categories != null && SelectedCategoryId == catId)
				return;

			if (null == _nav)
				_nav = new Stack<CategoryNode> ();

			CategoryNode node = null;
			if (Categories != null) {
				node = new CategoryNode {
					Name = SelectedCategoryName,
					Id = SelectedCategoryId,
					Categories = Categories,
				};
			}

			await LoadData(catId, catName, node);
		}

		private async Task LoadData(int catId, string catName, CategoryNode current)
		{
			if (OnLoadStart != null) await OnLoadStart();
			IsLoading = true;

			try {
				var result = await ECommerceWS.PointsCatalogCategories(SessionData.UserAuthentication);

				// http://issue.innovagency.com/view.php?id=20921
				if (current == null) {
					// Add "Ver todos" to the root.
					result.CatList.Insert(0, new CategoryOut {
						CatName = "Ver Todos",
						CatId = -1,
						CatLast = true
					});
				}

				Categories = new ObservableCollection<CategoryOut>(result.CatList);
				SelectedCategoryId = catId;
				SelectedCategoryName = catName;

				if (current != null)
					_nav.Push (current);
			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			} finally {
				IsLoading = false;
			}

			if (OnLoadSuccess != null) 
				OnLoadSuccess();
		}
	}
}
