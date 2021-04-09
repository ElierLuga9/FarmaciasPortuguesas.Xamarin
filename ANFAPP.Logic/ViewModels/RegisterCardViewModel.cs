using System;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{

    public class RegisterCardViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;

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

        private bool IsGenderInitialized = false;

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
//                return DateTime.Now.AddYears(-18);
				return new DateTime(DateTime.Now.Year - 18, 12, 31);
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
                OnPropertyChanged("IsBISelected");
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

        private bool IsIDTypeInitialized = false;

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
                _familySize = value;
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

        private bool _isTermsChecked;
        public bool IsTermsChecked
        {
            get { return _isTermsChecked; }
            set
            {
                _isTermsChecked = value;
                OnPropertyChanged("IsTermsChecked");
            }
        }

		private bool _allowPromotions;
		public bool AllowPromotions
		{
			get { return _allowPromotions; }
			set
			{
				_allowPromotions = value;
				OnPropertyChanged("AllowPromotions");
			}
		}

        #endregion

        #region Loaders

        /// <summary>
        /// Validate form and try to submit it to the web service.
        /// </summary>
        public async Task SubmitForm()
        {
            // Validate form
            if (!ValidateForm()) return;

			if (DateUtils.IsUnderage(Birthday)) {
				if (OnError != null) OnError(null, AppResources.UserFormUnderageError);
				return;
			}

            try
            {
                // Call WS to create a new card
                var cardObj = await LoginWS.RegisterCard(BuildCardRegistrationObject());

                // Associate the register
                await LoginWS.AssociateCard(BuildCardAssociationObject(cardObj.Number));
            }
            catch (Exception e)
            {
                // Service error - Post back to UI and show error
                if (OnError != null) OnError(AppResources.RegisterCardMessageTitle, e.Message);
				return;
            }

			try
			{
				// Attempt to get the updated profile
				var userProfile = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
				
				// Update the user's session info
				SessionData.SaveUser(userProfile, null);

				// Gamification Event! - TYPE_1 - Card Association
				GameCenterWS.PostEventAsync(Settings.GAMECENTER_EVENT_CARD_ASSOCIATION);
			}
			catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}

			// Success
			if (OnSuccess != null) OnSuccess();
        }

        /// <summary>
        /// Validate if all the required fields are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateForm()
        {
            if (!IsGenderInitialized || !IsIDTypeInitialized)
            {
                // Validate gender and ID type
                if (OnError != null) OnError(AppResources.RegisterCardErrorEmptyFieldsTitle,
                    AppResources.RegisterCardErrorEmptyFieldsMessage);
                return false;
            }
            else if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(IDNumber) || 
                string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Locale) || 
                string.IsNullOrEmpty(PostalCode4) || string.IsNullOrEmpty(PostalCode3))
            {
                // Validate Required Fields
                if (OnError != null) OnError(AppResources.RegisterCardErrorEmptyFieldsTitle,
                    AppResources.RegisterCardErrorEmptyFieldsMessage);
                return false;
            }
            else if (PostalCode4.Length != 4 || PostalCode3.Length != 3)
            {
                // Validate Postal Code
                if (OnError != null) OnError(AppResources.RegisterCardErrorPostalCodeTitle,
                    AppResources.RegisterCardErrorPostalCodeMessage);
                return false;
            }
            else if (Phone.Length != 9)
            {
                // Validate Phone
				var msg = Phone.Length == 0 ? AppResources.RegisterCardErrorPhoneEmpty : AppResources.RegisterCardErrorPhoneMessage;

				if (OnError != null) OnError(AppResources.RegisterCardErrorPhoneTitle, msg);
                return false;
            }
            else if (!IsTermsChecked)
            {
                // Validate Terms and Conditions
                if (OnError != null) OnError(AppResources.RegisterCardErrorTermsConditionsTitle,
                    AppResources.RegisterCardErrorTermsConditionsMessage);
                return false;
            }
			else if ((IDNumber.Length < 6 || IDNumber.Length > 8) && (IsBISelected))
            {
				if (OnError != null) OnError(AppResources.AssociateCardDocumentTypeLabel,
                    AppResources.RegisterCardErrorIDNumberMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all the view forms.
        /// </summary>
        public void ClearForm()
        {
            Name = IDNumber = FamilyName = Address = Locale = PostalCode4 = PostalCode3 = FamilySize = Phone = string.Empty;

            Birthday = MaxDate;
            IsTermsChecked = AllowPromotions = false;
            IsIDTypeInitialized= IsGenderInitialized = false;
        }

        #endregion

        #region Object Builders

        /// <summary>
        /// Build an input object for a card registration.
        /// </summary>
        /// <returns></returns>
        private CardRegistrationIn BuildCardRegistrationObject()
        {
            return new CardRegistrationIn()
            {
                Name = Name,
				AllowPromotions = AllowPromotions,
                Client = new CardRegistrationIn.ClientIn()
                {
                    Name = Name,
                    Gender = IsMale ? Settings.GENDER_MALE : Settings.GENDER_FEMALE,
                    BirthDate = Birthday.ToString("yyyy-MM-dd"),
                    DocumentType = IsBISelected ? Settings.DOCUMENT_TYPE_NATIONAL_ID : Settings.DOCUMENT_TYPE_PASSPORT,
                    DocumentNumber = IDNumber,
                    HouseholdSize = !string.IsNullOrEmpty(FamilySize) ? FamilySize : null,
                    Address = Address,
                    Locale = Locale,
                    PostalCode = PostalCode4 + "-" + PostalCode3,
                    ContactPhone = Phone,
                    Email = SessionData.PharmacyUser.Username
                }
            };
        }

        /// <summary>
        /// Build a card association input object.
        /// </summary>
        /// <returns></returns>
        private CardAssociationIn BuildCardAssociationObject(string cardNumber)
        {
            return new CardAssociationIn()
            {
                User = new CardAssociationIn.UserIn() 
                {
                    UserName = SessionData.PharmacyUser.Username,
                    DocumentNumber = IDNumber,
                    DocumentType = IsBISelected ? Settings.DOCUMENT_TYPE_NATIONAL_ID : Settings.DOCUMENT_TYPE_PASSPORT
                },
				Client = new CardAssociationIn.ClientIn()
				{
					PharmacyCard = new CardAssociationIn.CardIn { Number = cardNumber }
				}
            };
        }

        #endregion

    }
}
