using System;
using System.Globalization;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{

    public class EditUserDataViewModel : IViewModel
    {
		private bool _profileNotFound = true;
		private bool _allowPromotions = false;

        #region Properties

        private string _cardNumber;
        private string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;

                OnPropertyChanged("CardNumber");
            }
        }

		private string _clientNumber;
		private string ClientNumber
		{
			get { return _clientNumber; }
			set
			{
				_clientNumber = value;

				OnPropertyChanged("ClientNumber");
			}
		}

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

        private bool _isMale;
        public bool IsMale
        {
            get { return _isMale && IsGenderInitialized; }
            set
            {
                // Mark gender as initialized - used to have a state of "non-selection" at start.
                IsGenderInitialized = true;

                _isMale = value;

                OnPropertyChanged("IsMale");
                OnPropertyChanged("IsFemale");
            }
        }

        public bool IsFemale
        {
            get { return !_isMale && IsGenderInitialized; }
        }

        private bool IsGenderInitialized = true;

        private DateTime _birthday;
        public DateTime Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public DateTime MaxDate
        {
            get
            {
                // Returns the current date minus 18 years.
				return new DateTime(DateTime.Now.Year - 18, 12, 31);
//                return DateTime.Now.AddYears(-18);
            }
        }

        public DateTime MinDate
        {
            get
            {
                // Returns the current date minus 150 years.
//                return DateTime.Now.AddYears(-150);
				return new DateTime(DateTime.Now.Year - 150, 12, 31);
            }
        }

        private bool _isBISelected;
        public bool IsBISelected
        {
            get { return _isBISelected && IsIDTypeInitialized; }
            set
            {
                // Mark ID Type as initialized - used to have a state of "non-selection" at start.
                IsIDTypeInitialized = true;

                _isBISelected = value;
                OnPropertyChanged();
                OnPropertyChanged("IsPassportSelected");
				OnPropertyChanged("IDKeyboardType");
            }
        }

        public bool IsPassportSelected
        {
            get { return !_isBISelected && IsIDTypeInitialized; }
        }

		public object IDKeyboardType {
			get { return IsBISelected ? Keyboard.Numeric : Keyboard.Text; }
		}

        private bool IsIDTypeInitialized = true;

        private string _familyName;
        public string FamilyName
        {
            get { return _familyName; }
            set
            {
                _familyName = value;
                OnPropertyChanged("FamilyName");
            }
        }

        private string _idNumber;
        public string IDNumber
        {
            get { return _idNumber; }
            set
            {
                _idNumber = value;
                OnPropertyChanged("IDNumber");
            }
        }

        private string _familySize;
        public string FamilySize
        {
            get { return _familySize; }
            set
            {
				int size = 0;
				if (int.TryParse (value, out size) && size > 0) {
					_familySize = value;
				} else {
					_familySize = null;
				}
				OnPropertyChanged("FamilySize");

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

        #region Event Handlers

        public OnErrorEventHandler OnLoadError;
        public OnSuccessEventHandler OnLoadSuccess;

        public OnErrorEventHandler OnUpdateError;
        public OnSuccessEventHandler OnUpdateSuccess;

        #endregion

        #region Loaders

        /// <summary>
        /// Loads and updates the user data.
        /// </summary>
        public async void LoadData()
        {
            // Reload the cached data
            //       await ReloadCachedData();

            try
            {
                // Update user info
                var userInfo = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
				_profileNotFound = false;

				// Reload the data
				UpdateFields(userInfo);

                if (SessionData.PharmacyUser.IsFamilyCard)
                {
                    // Get the list of family members
                    var familyMembers = await FamilyAccountWS.GetFamilyMembers(SessionData.PharmacyUser.CardNumber);

                    // Update the family size
                    int size = (familyMembers != null && familyMembers.Value != null) ? familyMembers.Value.Count : 0;
                    SessionData.PharmacyUser.FamilySize = size;
                    SessionData.SaveUser();
                }

                // Notify Success
                if (OnLoadSuccess != null) OnLoadSuccess();
            }
            catch (Exception e)
            {
				if (null != OnLoadError) OnLoadError("Erro", e.Message);
            }
        }

        private void UpdateFields(UserProfileOut profileOut)
        {
            if (profileOut == null) return;

			_allowPromotions = profileOut.Client != null && profileOut.Client.PharmacyCard != null && profileOut.Client.PharmacyCard.AllowPromotions;

            // Update values on View
			DateTime bday = new DateTime(1);
			if (null != profileOut.Client && null != profileOut.Client.BirthDate) {
				bday = DateTime.Parse(profileOut.Client.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
				bday = bday.ToUniversalTime();
			}
            Name = profileOut.Client.Name;
			ClientNumber = profileOut.Client.Number;
			
			//CardNumber = profileOut.Client.PharmacyCard.Number;
			if (profileOut.Client.PharmacyCard != null) 
				CardNumber = profileOut.Client.PharmacyCard.Number;

            Phone = profileOut.Client.ContactPhone;
            Address = profileOut.Client.Address;
			Birthday = bday;
            FamilySize = profileOut.Client.HouseholdSize.ToString();

            if (profileOut.Client.Gender == "Male") { IsMale = true; } else { IsMale = false; }
            if (profileOut.Client.DocumentType == Settings.DOCUMENT_TYPE_NATIONAL_ID) { IsBISelected = true; } else { IsBISelected = false; }

            IDNumber = profileOut.Client.DocumentNumber;
            Locale = profileOut.Client.Locale;

            var PostalCodeArray = profileOut.Client.PostalCode.Split('-');

            PostalCode4 = PostalCodeArray[0];
            PostalCode3 = PostalCodeArray[1];

            // Save updated user
            SessionData.SaveUser(profileOut, null);
        }


        /// <summary>
        /// Update the view.
        /// </summary>
        /// <param name="profileOut"></param>
        #endregion

        #region Setters

        /// <summary>
        /// Uddate Data to Card WS.
        /// </summary>
        public async void UpdateCardData()
        {
            try
            {
				if (DateUtils.IsUnderage(Birthday)) {
					if (OnUpdateError != null) OnUpdateError(null, AppResources.UserFormUnderageError);
					return;
				}

				// If the user has no pharmacy card we need to create the profile.
				if (_profileNotFound && !SessionData.HasPharmacyCard) {

					var result = await LoginWS.CreateCRMProfile(BuildClientObject());

				} 
				else if (!SessionData.HasPharmacyCard) 
				{
					await UserCardWS.UpdateCRMProfile(BuildClientObject(), ClientNumber);
				} 
				else 
				{
					await UserCardWS.UpdatePharmacyCard(BuildCardRegistrationObject(), CardNumber);
				}

				if (OnUpdateSuccess != null) OnUpdateSuccess();
            }
            catch (Exception e)
            {
                if (OnUpdateError != null) OnUpdateError(null, e.Message);

            }
        }

        /// <summary>
        /// Build an input object for a card registration.
        /// </summary>
        /// <returns></returns>
        private CardRegistrationIn BuildCardRegistrationObject()
        {
			var cardObj = new CardRegistrationIn()
            {
                Name = Name,
				Client = BuildClientObject(),
				AllowPromotions = _allowPromotions
            };

			return cardObj;
        }

		/// <summary>
		/// Build an input object for a card registration.
		/// </summary>
		/// <returns></returns>
		private CardRegistrationIn.ClientIn BuildClientObject()
		{
			var clientObj = new CardRegistrationIn.ClientIn()
			{
				Name = Name,
				Gender = IsMale ? Settings.GENDER_MALE : Settings.GENDER_FEMALE,
				BirthDate = Birthday.ToString("yyyy-MM-dd"),
				DocumentType = IsBISelected ? Settings.DOCUMENT_TYPE_NATIONAL_ID : Settings.DOCUMENT_TYPE_PASSPORT,
				DocumentNumber = IDNumber,
				Address = Address,
				Locale = Locale,
				PostalCode = PostalCode4 + "-" + PostalCode3,
				ContactPhone = Phone,
				Email = SessionData.PharmacyUser.Username
			};

			int size = 0;
			int.TryParse(FamilySize, out size);
			clientObj.HouseholdSize = size + string.Empty;

			return clientObj;
		}

        #endregion

    }
}
