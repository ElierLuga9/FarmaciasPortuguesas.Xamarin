using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutReservationViewModel : BaseCheckoutViewModel
	{

		#region Loaders

		/// <summary>
		/// Confirm the checkout 
		/// </summary>
		public async void ConfirmCheckout()
		{
			try
			{
				// Complete checkout
				var response = await ECommerceWS.CheckoutConf(SessionData.UserAuthentication, null);
				if (response == null || !response.Success) throw new Exception(AppResources.CheckoutGenericError);

				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}
		}

		#endregion
        #region Auxiliary Properties
        public string Disclamer 
        {
            get 
            {
                return string.Format(AppResources.CheckoutReservationDisclamer, "Nome da Farmácia");
            }
        }
        #endregion
    }
}
