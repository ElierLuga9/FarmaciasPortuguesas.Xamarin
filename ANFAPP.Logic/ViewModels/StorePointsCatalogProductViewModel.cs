using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using System.Collections.ObjectModel;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StorePointsCatalogProductViewModel : IViewModel
	{
		private long _catId;
		private string _catName;

		public StorePointsCatalogProductViewModel(long catId, string catName)
			: base()
		{
			_catId = catId;
			_catName = catName;
		}

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Properties

		public enum GroupType { Catalog };

		public class ProductGroup : ObservableCollection<ProductOut>
		{
			public string Name { get; set; }
			public GroupType Type { get; set; }

			public ProductGroup(string name, GroupType type)
			{
				Name = name;
				Type = type;
			}
		}

		private ObservableCollection<ProductGroup> _products { get; set; }
		public ObservableCollection<ProductGroup> Products
		{ 
			get { return _products; }
			set 
			{ 
				_products = value; 
				OnPropertyChanged("Products");
			}
		}

		#endregion

		public async Task LoadData() {
			if (OnLoadStart != null) await OnLoadStart();

			try {
				var result = await ECommerceWS.PointsCatalogProds(SessionData.UserAuthentication, _catId);


				var group = new ProductGroup(_catName, GroupType.Catalog);
				foreach (ProductOut p in result.Products) {
					group.Add(p);
				}

				Products = new ObservableCollection<ProductGroup>();
				Products.Add(group);
				
			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			} 

			if (OnLoadSuccess != null) OnLoadSuccess();
		}
	}
}

