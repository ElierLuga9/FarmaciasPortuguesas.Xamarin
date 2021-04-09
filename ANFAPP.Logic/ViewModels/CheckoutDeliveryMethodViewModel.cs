using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Models.Wrappers;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.EventHandlers;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutDeliveryMethodViewModel : BaseCheckoutViewModel
	{
		#region Events

		public OnSuccessEventHandler OnDeliveryChangeSuccess;
		public OnSuccessEventHandler OnImageRemovedSuccess;

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (string.Equals(propertyName, "Basket"))
			{
				OnPropertyChanged("IsHomeDelivery");
				OnPropertyChanged("HasHomeDelivery");
				OnPropertyChanged("AvailabilityText");
			}
		}
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

		public bool IsHomeDelivery
		{
			get
			{
				return Basket != null && Basket.SelectedDeliveryType != null && Basket.SelectedDeliveryType.Equals(Settings.CHECKOUT_DELIVERY_HOME);
			}
		}

		public bool HasHomeDelivery
		{
			get
			{
				//return Basket != null && (!Basket.HasMSRM || Basket.HasMSRMHomeDelivery);
				return Basket != null && Basket.HasHomeDelivery;
			}
		}

		private ObservableCollection<PhotoFile> _recipeFiles;
		public ObservableCollection<PhotoFile> RecipeFiles
		{
			get { return _recipeFiles; }
			set
			{
				_recipeFiles = value;
				OnPropertyChanged("RecipeFiles");
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
		private string _bindingAddress;
		public string BindingAddress
		{
			get { return _bindingAddress; }
			set
			{
				_bindingAddress = value;
				OnPropertyChanged("BindingAddress");
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
			get { return _postalCode3; }
			set
			{
				_postalCode3 = value;
				OnPropertyChanged("PostalCode3");
			}
		}
		private string _bilingPostalCode4;
		public string BilingPostalCode4
		{
			get { return _bilingPostalCode4; }
			set
			{
				_bilingPostalCode4 = value;
				OnPropertyChanged("BilingPostalCode4");
			}
		}

		private string _bilingPostalCode3;
		public string BilingPostalCode3
		{
			get { return _bilingPostalCode3; }
			set
			{
				_bilingPostalCode3 = value;
				OnPropertyChanged("BilingPostalCode3");
			}
		}

		private string _location;
		public string Location
		{
			get { return _location; }
			set
			{
				_location = value;
				OnPropertyChanged("Location");
			}
		}
		private string _bilingLocation;
		public string BilingLocation
		{
			get { return _bilingLocation; }
			set
			{
				_bilingLocation = value;
				OnPropertyChanged("BilingLocation");
			}
		}
		private bool _showPhotoInput;
		public bool ShowPhotoInput
		{
			get { return _showPhotoInput; }
			set
			{
				_showPhotoInput = value;
				OnPropertyChanged("ShowPhotoInput");
			}
		}

		#endregion

		#region Loaders

		public override async void RefreshBasket()
		{
			base.RefreshBasket();

			// Make sure Home delivery is off, if its not available
			if (!HasHomeDelivery) await SetHomeDelivery(false);
		}

		public async Task SetHomeDelivery(bool deliver)
		{
			if (string.IsNullOrEmpty(Basket.SelectedDeliveryType) || (IsHomeDelivery != deliver))
			{

				if (OnLoadStart != null)
					await OnLoadStart();

				try
				{
					var response = await ECommerceWS.SetCheckoutDeliveryMethod(deliver ? Settings.CHECKOUT_DELIVERY_HOME : Settings.CHECKOUT_DELIVERY_PHARMACY, SessionData.UserAuthentication);
					if (response.code != 200) throw new ServiceErrorException(response.msg);

					var updatedBasket = await ECommerceWS.CheckoutStart(SessionData.UserAuthentication);
					Basket = updatedBasket;

					if (OnDeliveryChangeSuccess != null) OnDeliveryChangeSuccess();

				}
				catch (Exception e)
				{
					string message = e.Message;
					if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
					if (OnLoadError != null) OnLoadError(null, message);
				}
			}
		}

		/// <summary>
		/// Changes the delivery method via the WS.
		/// </summary>
		public async Task ConfirmDeliveryMethod()
		{
			if (string.IsNullOrEmpty(Basket.SelectedDeliveryType))
			{
				// Non-selected so select the default delivery option
				await SetHomeDelivery(IsHomeDelivery);
			}

			// Validate inputs
			if (IsHomeDelivery)
			{
				if (!InputsAreValid())
				{
					OnLoadError(null, AppResources.CheckoutDeliveryEmptyFieldsMessage);
					return;
				}

				// Does the basket have products that require prescription? If so, we must upload a photo.
				//bool msrm = false;
				//foreach (BasketProductOut item in Basket.Products)
				//{
				//	if (item.IsMSRM)
				//	{
				//		msrm = true;
				//		break;
				//	}
				//}

				ShowPhotoInput = Basket.HasMSRM;
			}

			try
			{
				// Set/Change the Delivery Method
				// XXX: The code is hardcoded. The app (which presents a toggle to select the delivery) has to map one of MODOSENTREGAPOSSIVEIS to the home delivery.
				// var response = await ECommerceWS.SetCheckoutDeliveryMethod(IsHomeDelivery ? "toogas_anfcarrier" : "flatrate", SessionData.UserAuthentication);
				// if (response.code != 200) throw new ServiceErrorException(response.msg);

				if (IsHomeDelivery)
				{
					if (RecipeFiles != null && RecipeFiles.Count > 0)
					{
						foreach (PhotoFile pf in RecipeFiles)
						{
							try 
							{ 
								var result = await ECommerceWS.PutImageInCart(pf, SessionData.UserAuthentication);
								if (result.code == 200)
								{
									System.Diagnostics.Debug.WriteLine("Did upload image named {0}", pf.Filename);
								}
								else
								{
									System.Diagnostics.Debug.WriteLine("Failed to upload image named {0}", pf.Filename);
								}
							}
							catch { System.Diagnostics.Debug.WriteLine("Failed to upload image named {0}", pf.Filename); }
						}
					}

					// Update address (if home delivery)
					var addressUpdate = await ECommerceWS.SetShippingAddress(new AddressOut()
					{
						Name = Basket.BillingAddress.Name,
						Address1 = Address,
						Address2 = string.Empty,
						County = string.Empty,
						District = string.Empty,
						PostalCode = PostalCode4 + (!string.IsNullOrEmpty(PostalCode3) ? "-" + PostalCode3 : string.Empty),
						Location = Location
					}, SessionData.UserAuthentication);
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
		/// Take a Photo and add it to the list.
		/// </summary>
		public async void TakePhoto()
		{
			var mediaPicker = DependencyService.Get<IMediaPicker>() ?? Resolver.Resolve<IDevice>().MediaPicker;
			if (mediaPicker == null || !mediaPicker.IsCameraAvailable)
			{
				OnLoadError(null, AppResources.CheckoutDeliveryNoCameraSupportMessage);
				return;
			}

			try
			{
				// Take photo and add it to the list
				var file = await mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions() { 
					DefaultCamera = CameraDevice.Rear, MaxPixelDimension = 600 });
				if (file != null)
				{
					if (RecipeFiles == null) RecipeFiles = new ObservableCollection<PhotoFile>();
					RecipeFiles.Add(new PhotoFile(file));

					var aux = RecipeFiles;
					RecipeFiles = null;
					RecipeFiles = aux;
				}
			}
			catch (TaskCanceledException) { }
		}

		/// <summary>
		/// Remove Photo
		/// </summary>
		public async Task RemovePhoto(PhotoFile file)
		{
			if (RecipeFiles == null) return;

			var idx = RecipeFiles.IndexOf(file);
			

			try
			{
				var result = await ECommerceWS.DelImageInCart(file.Filename, SessionData.UserAuthentication);
				if (result.code >= 300)
				{
					throw new ServiceErrorException(result.msg);
					
				}

				RecipeFiles.Remove(file);
				if (OnImageRemovedSuccess != null) OnImageRemovedSuccess();
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}
		}

		/// <summary>
		/// Validate if all the inputs are correct.
		/// </summary>
		/// <returns></returns>
		protected bool InputsAreValid()
		{
			return !(string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(PostalCode4) || string.IsNullOrEmpty(PostalCode3) || string.IsNullOrEmpty(Location));
		}

		/// <summary>
		/// Update the address values
		/// </summary>
		public void UpdateAddress()
		{
			if (Basket != null && Basket.DeliveryAddress != null)
			{
				var address = Basket.DeliveryAddress;

				// Update inputs
				Address = address.Address1;
				Location = address.Location;

				var postalCode = !string.IsNullOrEmpty(address.PostalCode) ? address.PostalCode.Split('-') : null;
				if (postalCode != null)
				{
					PostalCode4 = postalCode.Length > 0 ? postalCode[0] : null;
					PostalCode3 = postalCode.Length > 1 ? postalCode[1] : null;
				}

				// Does the basket have products that require prescription? If so, we must upload a photo.
				//bool msrm = false;
				//foreach (BasketProductOut item in Basket.Products)
				//{
				//	if (item.IsMSRM)
				//	{
				//		msrm = true;
				//		break;
				//	}
				//}
				ShowPhotoInput = Basket.HasMSRM;
			}

			// Load the ANF address if any field is empty.
			LoadANFAddress();
		}

		/// <summary>
		/// Loads the address from ANF CRM, in case there is any empty field
		/// </summary>
		/// 
		private void LoadANFAddress()
		{
			if (SessionData.UserProfileForCheckout == null || SessionData.UserProfileForCheckout.Client == null) return;

			// Address
			if (string.IsNullOrEmpty(Address)) Address = SessionData.UserProfileForCheckout.Client.Address;
			if (string.IsNullOrEmpty(Location)) Location = SessionData.UserProfileForCheckout.Client.Locale;

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
		}
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
				if (string.IsNullOrWhiteSpace(SessionData.PharmacyUser.ClientNumber))
				{
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
					Address1 = BindingAddress,
					Address2 = string.Empty,
					County = string.Empty,
					District = string.Empty,
					PostalCode = BilingPostalCode4 + (!string.IsNullOrEmpty(BilingPostalCode3) ? "-" + BilingPostalCode3 : string.Empty),
					Location = BilingLocation != null ? Locale : string.Empty,
					Name = Name,
					NIF = nif,
					Phone = long.Parse(Phone)
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
				BindingAddress = Basket.BillingAddress.Address1;
				BilingLocation = Basket.BillingAddress.Location;

				if (!string.IsNullOrEmpty(Basket.BillingAddress.PostalCode))
				{
					var postalCode = Basket.BillingAddress.PostalCode.Split('-');
					if (postalCode == null) return;

					BilingPostalCode4 = postalCode.Length > 0 ? postalCode[0] : null;
					BilingPostalCode3 = postalCode.Length > 1 ? postalCode[1] : null;
				}

				Phone = Basket.BillingAddress.Phone.ToString();
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

			if (string.IsNullOrEmpty(BindingAddress)) BindingAddress = SessionData.UserProfileForCheckout.Client.Address;
			if (string.IsNullOrEmpty(BilingLocation)) BilingLocation = SessionData.UserProfileForCheckout.Client.Locale;

			if (string.IsNullOrEmpty(BilingPostalCode3))
			{
				var postalCode = !string.IsNullOrEmpty(SessionData.UserProfileForCheckout.Client.PostalCode) ?
					SessionData.UserProfileForCheckout.Client.PostalCode.Split('-') : null;
				if (postalCode != null)
				{
					BilingPostalCode4 = postalCode.Length > 0 ? postalCode[0] : null;
					BilingPostalCode3 = postalCode.Length > 1 ? postalCode[1] : null;
				}
			}

			if (int.Parse(Phone) == 0)
			{
				long auxPhone = 0;
				long.TryParse(SessionData.UserProfileForCheckout.Client.ContactPhone, out auxPhone);
				Phone = auxPhone.ToString();
			}

			if (string.IsNullOrEmpty(Name)) Name = SessionData.UserProfileForCheckout.Client.Name;
			if (string.IsNullOrEmpty(NIF)) NIF = SessionData.UserProfileForCheckout.Client.FiscalNumber;
		}
		#endregion

		#region Auxiliary Properties
		public string AvailabilityText
		{
			get
			{
				/*
				string text = "";

				LocalProductAvailability lavail;

				// Here we need to use local time, to take into account daylight savings and timezones in different regions.
				DateTime now = DateTime.Now;

				if (now.TimeOfDay.Hours < 12 && now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday)
				{
					lavail = LocalProductAvailability.Today;
				}
				else if (now.TimeOfDay.Hours >= 12 && now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Thursday)
				{
					lavail = LocalProductAvailability.Tomorrow;
				}
				else
				{
					lavail = LocalProductAvailability.NextWeek;
				}

				switch (lavail)
				{
					case LocalProductAvailability.NextWeek:
						text = AppResources.CheckoutAvailabilityRule1;
						break;
					case LocalProductAvailability.Today:
						text = AppResources.CheckoutAvailabilityRule2;
						break;
					case LocalProductAvailability.Tomorrow:
						text = AppResources.CheckoutAvailabilityRule3;
						break;

					default:
						break;
				}
				*/
				string text = Basket != null ? Basket.BasketAvailability : "";

				var tmp = string.Format(AppResources.CheckoutPharmacyDeliveryDisclamer, text.ToLower(), SessionData.StorePharmacyName, SessionData.StorePharmacyAddress);
				// Fix https://consulting.glintt.com/mantis/view.php?id=36631

				return text;
				//return tmp + "\n ";
			}


		}
		#endregion

	}
}
