using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Logic.ViewModels
{

	public class UserOrderDetailsViewModel : IViewModel
	{

		#region Delegates

		public delegate void OnAddToBasketEventHandler(string message);

		#endregion

		#region Properties

		private string _orderNumber { get; set; }
		private DeliveryOut _detail { get; set; }
		public DeliveryOut Detail
		{
			get { return _detail; }
			set
			{
				_detail = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnSuccessEventHandler OnLoadSuccess;
		public OnAddToBasketEventHandler OnAddToBasketSuccess;

		#endregion

		#region Loader
		public async void LoadData(string orderNumber)
		{
			try
			{
				_orderNumber = orderNumber;
				var result = await ECommerceWS.GetOrderDetail(orderNumber, SessionData.UserAuthentication);
				if (result == null || result.DeliveryList == null || result.DeliveryList.Count == 0)
				{
					if (OnLoadError != null) OnLoadError(null, AppResources.GenericErrorMessage);
					return;
				}

				// Fetch the detail
				Detail = result.DeliveryList[0];

				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception ex)
			{
				if (OnLoadError != null) OnLoadError(null, AppResources.GenericErrorMessage);
			}
		}

		public async void AddOrderToCart() 
		{
			try 
			{
				var result = await ECommerceWS.PostReorder(_orderNumber, SessionData.UserAuthentication);
				if (result != null && result.ProductsAddedCount > 0) {

					try 
					{
						var mixpanelWidget = DependencyService.Get<IMixPanel>();


						if (result.ProductsAdded != null) {

							foreach(var prod in result.ProductsAdded) {
								var props = new Dictionary<string, string>();

								props.Add("CNP", prod.CNP);
								props.Add("Quantity", prod.Quantity);
								props.Add("Price", prod.Price);
								props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
								props.Add("PharmacyID", SessionData.StorePharmacyId);

								mixpanelWidget.TrackProperties("AddToCart", props);	
							}
						}
					} catch { }

					if (OnAddToBasketSuccess != null) OnAddToBasketSuccess(result.msg);	
				} else {
					if (OnLoadError != null) OnLoadError(null, result.msg);
				}
			}
			catch (Exception ex)
			{
				if (OnLoadError != null) OnLoadError(null, AppResources.GenericErrorMessage);
			}
		}

		#endregion
	}
}
