using System;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Wrappers;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.In.Ecommerce;
using ANFAPP.Logic.Exceptions;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Logic.ViewModels
{
	public class StorePrescriptionViewModel : IViewModel
	{

		#region Event Handlers

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;
		public OnSuccessEventHandler OnCartChangeSuccess;

		#endregion


		#region Properties

		private ObservableCollection<PAListItem> _paQueryItems;
		public ObservableCollection<PAListItem> PAQueryItems 
		{
			get { return _paQueryItems; }
			set { 
				_paQueryItems = value;
				OnPropertyChanged ("PAQueryItems");
			}
		}

		private string _paQuery;
		public string PAQuery { 
			get { return _paQuery; } 
			set { 
				_paQuery = value;
				OnPropertyChanged ("PAQuery");
			}
		}

		private ObservableCollection<PrescriptionItem> _items;
		public ObservableCollection<PrescriptionItem> Items 
		{
			get { return _items; }
			set { 
				_items = value;
				OnPropertyChanged ("Items");
				OnPropertyChanged("HasItems");
			}
		}

		private bool _searchByActivePrinciple;
		public bool SearchByActivePrinciple
		{
			get { return _searchByActivePrinciple; }
			set 
			{ 
				_searchByActivePrinciple = value;
				OnPropertyChanged ("SearchByActivePrinciple");
			}
		}

		public bool HasItems
		{
			get { return Items != null && Items.Count > 0; }
		}

		#endregion

		private void AddWithCNPorCNPEM(SearchCNPEMOut searchResult, int code) 
		{
			if (null == Items) { Items = new ObservableCollection<PrescriptionItem>(); }

			var trimmed =  searchResult.Item.CNPPrinciple != null ? searchResult.Item.CNPPrinciple.Trim () : null;

			var wrapper = new PrescriptionItem () {
				Code = code,
				Type = searchResult.Type,
				Name = !string.IsNullOrEmpty(searchResult.Item.Name) ? searchResult.Item.Name : searchResult.Item.ActivePrinciple,
				Dosage = searchResult.Item.Dosage,
				FF = searchResult.Item.FF,
				Pack = searchResult.Item.Pack,
				// http://issue.innovagency.com/view.php?id=20828
				CNPPrinciple = trimmed != null ? trimmed.Substring(0, Math.Min(140, trimmed.Length)) : null,
				Presentation = searchResult.Item.PackDescription, // XXX: replace me after the WS is changed to return the PRESENTATION
			};

			Items.Add(wrapper);
			OnPropertyChanged("HasItems");
		}

		/// <summary>
		/// Adds the result of a search by active principle to the list.
		/// 
		/// The GetPAOut item must have a single element on the Dosages list, that is,
		/// it must be the result of calling GetPA with a DOSID.
		/// 
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddWithPA(GetPAOut item, int cnpem)
		{
			if (null == Items) { Items = new ObservableCollection<PrescriptionItem>(); }

			var wrapper = new PrescriptionItem()
			{
				ActivePrincipleId = item.ActivePrincipleId,
				Name = item.ActivePrinciple,
				Dosage = item.Dosages[0].Name,
				Code = cnpem,
				Type = "CNPEM"
			};

			foreach (PackItem pack in item.Packs)
			{
				if (pack.CNPEM == cnpem)
				{
					wrapper.PackName = pack.Name;
					break;
				}
			}

			Items.Add(wrapper);
			OnPropertyChanged("HasItems");
		}

		/// <summary>
		/// Update a product's quantity
		/// </summary>
		/// <param name="qty"></param>
		/// <param name="obj"></param>
		public void UpdateQuantity(int qty, object obj)
		{
			var item = obj as PrescriptionItem;

			if (null != item) 
			{
				if (qty == 0) 
				{
					Items.Remove (item);
					OnPropertyChanged("HasItems");
				} 
				else 
				{
					item.Quantity = qty;
				}
			}
		}

		/// <summary>
		/// Add the products to the basket
		/// </summary>
		/// <returns></returns>
		public async Task AddProductsToBasket()
		{
			var products = new List<BasketProductIn> ();
			foreach (PrescriptionItem item in Items) 
			{
				products.Add (new BasketProductIn 
				{
					CNP = string.Equals("CNP", item.Type) ? item.Code : null,
					CNPEM = string.Equals("CNPEM", item.Type) ? item.Code : null,
					Quantity = item.Quantity,
					Type = item.Type
				});
			}

			try 
			{
				if (OnLoadStart != null) await OnLoadStart();
				await ECommerceWS.AddProductsToBasket(SessionData.UserAuthentication, products);

				var mixpanelWidget = DependencyService.Get<IMixPanel>();
				foreach (var item in products) {
					var props = new Dictionary<string, string>();
					props.Add("CNP", (item.CNP.HasValue && item.CNP > 0) ? item.CNP.ToString() : "CNPEM-" + item.CNPEM);
					props.Add("Quantity", item.Quantity.ToString());

					props.Add("Price", string.Empty);
					props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
					props.Add("PharmacyID", SessionData.StorePharmacyId);

					mixpanelWidget.TrackProperties("AddToCart", props);	
				}


				// Clear the items on success.
				Items.Clear();
				Items = null;
				if (OnLoadSuccess != null) OnLoadSuccess();
				if (OnCartChangeSuccess != null) OnCartChangeSuccess();
			} 
			catch (Exception e) 
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}
		}

		/// <summary>
		/// Search by Active Principle
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public async Task<List<PAListItem>> SearchPA(string query)
		{
			if (OnLoadStart != null) await OnLoadStart();

			try {
				var result = await ECommerceWS.SearchPA (SessionData.UserAuthentication, query, SessionData.StorePharmacyId);

				if (result != null && result.ListaPA != null && result.ListaPA.Count > 0)
				{
					if (OnLoadSuccess != null) OnLoadSuccess();
				}
				else
				{
					throw new ServiceErrorException("Não foram encontrados resultados.");
				}

				return result.ListaPA;
			} 
			catch (Exception e) 
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}
	
			return null;
		}

		/// <summary>
		/// Search by CNP or CNPEM
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public async Task Search(string query)
		{

			try { 
				// Validate if the query is either a CNP or a CNPEM		
				int code = 0;	
				if (string.IsNullOrEmpty(query) || query.Length < 7 || query.Length > 8 || !int.TryParse(query, out code))
				{ 
					throw new ArgumentException(AppResources.StorePrescriptionBadInputErrorMessage); 
				}

				if (OnLoadStart != null) await OnLoadStart();

				var result = await ECommerceWS.SearchCNPorCNPEM(SessionData.UserAuthentication, SessionData.StorePharmacyId, code);
				if (result.code != 200 || result.Item == null)
				{
					throw new ServiceErrorException(AppResources.StorePrescriptionNoResultsError);
				}
				else
				{
					AddWithCNPorCNPEM(result, code);
				}

				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}
		}
	}
}

