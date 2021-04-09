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
	public class CheckoutPhoneConfirmationViewModel : BaseCheckoutViewModel
	{
		
		#region Properties

		private string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				_phone = value;
				OnPropertyChanged("Phone");
			}
		}

		#endregion

		#region Loaders

		/// <summary>
		/// Updates the Phone Number
		/// </summary>
		public async Task UpdatePhoneNumber()
		{
			int phoneNumber = 0;
			/// Validates if the user has entered the phone number, and if the number is valid (has 9 digits).
			if (string.IsNullOrEmpty(Phone) || Phone.Length < 9 || !int.TryParse(Phone, out phoneNumber))
			{
				//OnError(null, AppResources.)
				if (OnLoadError != null) OnLoadError(null, AppResources.CheckoutPhoneEmptyFields);
				return;
			}

			if (Basket == null) return;
			if (Basket.BillingAddress == null) Basket.BillingAddress = new AddressOut();
			Basket.BillingAddress.Phone = phoneNumber;

			try
			{
				// Create a PfpClient if the client doesn't exist yet (for instance, a user without card created from the app).
				if (string.IsNullOrWhiteSpace(SessionData.PharmacyUser.ClientNumber)) {
					var result = await LoginWS.CreateCRMClient(SessionData.PharmacyUser.Username, Phone);
					SessionData.PharmacyUser.ClientNumber = result.Number;
					SessionData.PharmacyUser.ContactPhone = result.ContactPhone;
				}

				/// The phone number is connected to the billing address. Update the address with the data from checkout + the phone number.
				var response = await ECommerceWS.SetBillingAddress(Basket.BillingAddress, SessionData.UserAuthentication);
				if (response.code != 210)
				{
					if (OnLoadError != null) OnLoadError(null, response.msg);
					return;
				}
				
				if (OnLoadSuccess != null) OnLoadSuccess();
			}
			catch (Exception e) 
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}
		}

		/// <summary>
		/// Update the phone binding if it was previously set
		/// </summary>
		public void UpdateBindings()
		{
			if (Basket != null && Basket.BillingAddress != null && Basket.BillingAddress.Phone != 0) 
			{
				Phone = Basket.BillingAddress.Phone + string.Empty;
			}
			else if (SessionData.UserProfileForCheckout != null && SessionData.UserProfileForCheckout.Client != null)
			{
				Phone = SessionData.UserProfileForCheckout.Client.ContactPhone;
			}
		}

		#endregion

	}
}
