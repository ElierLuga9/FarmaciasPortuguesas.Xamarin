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

namespace ANFAPP.Logic.ViewModels
{

    public class UserOrdersViewModel : IViewModel
    {

        #region Properties

		private bool _hasOrders = true;
		public bool HasOrders 
		{
			get { return _hasOrders; }
			set
			{
				_hasOrders = value;
				OnPropertyChanged("HasOrders");
			}
		}

        private UserOrderOut _OrdersList { get; set; }
        public UserOrderOut OrdersList
        {
            get { return _OrdersList; }
            set
            {
                _OrdersList = value;
                OnPropertyChanged();

				// Update has orders
				HasOrders = _OrdersList != null && _OrdersList.DeliveryList != null && _OrdersList.DeliveryList.Count > 0;
            }
        }

        #endregion

        #region Events

        public OnErrorEventHandler OnLoadError;
        public OnSuccessEventHandler OnLoadSuccess;

        #endregion

        #region Loader
        public async void LoadData()
        {
            try
            {
                var result = await ECommerceWS.GetOrders(SessionData.UserAuthentication);
                OrdersList = result;

            }
            catch (Exception ex)
            {
				HasOrders = false;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (OnLoadError != null)
					OnLoadError("", AppResources.GenericErrorMessage);
            }

            if (OnLoadSuccess != null) OnLoadSuccess();

        }


        #endregion
    }
}
