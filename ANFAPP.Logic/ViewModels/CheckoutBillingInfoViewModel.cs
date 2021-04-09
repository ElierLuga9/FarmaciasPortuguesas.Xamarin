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
	public class CheckoutBillingInfoViewModel : BaseCheckoutViewModel
	{
		
		#region Properties

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		private string _nif;
		public string NIF
		{
			get { return _nif; }
			set
			{
				_nif = value;
				OnPropertyChanged("NIF");
			}
		}

		private long _phone;
		public long Phone
		{
			get { return _phone; }
			set
			{
				_phone = value;
				OnPropertyChanged("Phone");
			}
		}

		private string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged("Address");
			}
		}

		private string _postalCode4;
		public string PostalCode4
		{
			get { return _postalCode4; }
			set
			{
				_postalCode4 = value;
				OnPropertyChanged("PostalCode4");
			}
		}
		private string _postalCode3;
		public string PostalCode3
		{
			get { return _postalCode3 ; }
			set
			{
				_postalCode3 = value;
				OnPropertyChanged("PostalCode3");
			}
		}

		private string _locale;
		public string Locale
		{
			get { return _locale; }
			set
			{
				_locale = value;
				OnPropertyChanged("Locale");
			}
		}

		#endregion

		#region Loaders

		/// <summary>
		/// 
		/// </summary>
		public async Task UpdateBillingAddress()
		{
			long nif = 0;

			// Validate inputs
			if (!InputsAreValid())
			{
				if (OnLoadError != null) OnLoadError(null, AppResources.CheckoutDeliveryEmptyFieldsMessage);
				return;
			}
			else if (!long.TryParse(NIF, out nif) || NIF.Length < 9)
			{
				if (OnLoadError != null) OnLoadError(null, AppResources.CheckoutInvalidNIFErrorMessage);
				return;
			}

			try
			{
				// Update address (if home delivery)
				var addressUpdate = await ECommerceWS.SetBillingAddress(new AddressOut()
				{
					Address1 = Address,
					Address2 = string.Empty,
					County = string.Empty,
					District = string.Empty,
					PostalCode = PostalCode4 + (!string.IsNullOrEmpty(PostalCode3) ? "-" + PostalCode3 : string.Empty),
					Location = Locale != null ? Locale : string.Empty,
					Name = Name,
					NIF = nif,
					Phone = Phone
				}, SessionData.UserAuthentication);

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
		/// 
		/// </summary>
		public void UpdateBindableData()
		{
			if (Basket != null && Basket.BillingAddress != null) 
			{
				Address = Basket.BillingAddress.Address1;
				Locale = Basket.BillingAddress.Location;

				if (!string.IsNullOrEmpty(Basket.BillingAddress.PostalCode))
				{
					var postalCode = Basket.BillingAddress.PostalCode.Split('-');
					if (postalCode == null) return;

					PostalCode4 = postalCode.Length > 0 ? postalCode[0] : null;
					PostalCode3 = postalCode.Length > 1 ? postalCode[1] : null;
				}

				Phone = Basket.BillingAddress.Phone;
				Name = Basket.BillingAddress.Name;
				NIF = Basket.BillingAddress.NIF > 0 ? Basket.BillingAddress.NIF + string.Empty : null;
			}

			// Load the ANF data from CRM if any field if empty.
			LoadANFData();
		}

		/// <summary>
		/// Loads the address from ANF CRM, in case there is any empty field
		/// </summary>
		private void LoadANFData()
		{
			if (SessionData.UserProfileForCheckout == null || SessionData.UserProfileForCheckout.Client == null) return;

			if (string.IsNullOrEmpty(Address)) Address = SessionData.UserProfileForCheckout.Client.Address;
			if (string.IsNullOrEmpty(Locale)) Locale = SessionData.UserProfileForCheckout.Client.Locale;

			if (string.IsNullOrEmpty(PostalCode3))
			{
				var postalCode = !string.IsNullOrEmpty(SessionData.UserProfileForCheckout.Client.PostalCode) ?
					SessionData.UserProfileForCheckout.Client.PostalCode.Split('-') : null;
				if (postalCode != null)
				{
					PostalCode4 = postalCode.Length > 0 ? postalCode[0] : null;
					PostalCode3 = postalCode.Length > 1 ? postalCode[1] : null;
				}
			}

			if (Phone == 0) {
				long auxPhone = 0;
				long.TryParse(SessionData.UserProfileForCheckout.Client.ContactPhone, out auxPhone);
				Phone = auxPhone;
			}

			if (string.IsNullOrEmpty(Name)) Name = SessionData.UserProfileForCheckout.Client.Name;
			if (string.IsNullOrEmpty(NIF)) NIF = SessionData.UserProfileForCheckout.Client.FiscalNumber;
		}

		#endregion

		/// <summary>
		/// Validate if all the inputs are correct.
		/// </summary>
		/// <returns></returns>
		protected bool InputsAreValid()
		{
			// http://issue.innovagency.com/view.php?id=20665
			// Validate name, nif and if NIF has 9 digits
			return !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(NIF));
		}

	}
}
