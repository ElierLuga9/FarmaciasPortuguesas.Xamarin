using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class AssociateCardViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;

        #endregion

        #region Properties

        private string _cardNumber;
        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                OnPropertyChanged("CardNumber");
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
            }
        }

        public bool IsPassportSelected
        {
            get { return !_isBISelected && IsIDTypeInitialized; }
        }

        private bool IsIDTypeInitialized = false;

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

        #endregion

        #region Loaders

        /// <summary>
        /// Validate form and try to submit it to the web service.
        /// </summary>
        public async void SubmitForm()
        {
            // Validate form
            if (!ValidateForm()) return;

            try
            {
                // Call WS to associate a new card
                await LoginWS.AssociateCard(BuildCardAssociationObject());
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

				// Gamification Achievement! - TYPE_1 - Card Association
				GameCenterWS.PostEventAsync(Settings.GAMECENTER_EVENT_CARD_ASSOCIATION);
			}
			catch (Exception e) { }

			// Success
			if (OnSuccess != null) OnSuccess();
        }

        /// <summary>
        /// Validate if all the required fields are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateForm()
        {
            if (!IsIDTypeInitialized)
            {
                // Validate Required Fields
                if (OnError != null) OnError(AppResources.AssociateCardErrorEmptyFieldsTitle,
                    AppResources.AssociateCardErrorEmptyFieldsMessage);
                return false;
            }
            else if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(IDNumber))
            {
                // Validate Required Fields
                if (OnError != null) OnError(AppResources.AssociateCardErrorEmptyFieldsTitle,
                    AppResources.AssociateCardErrorEmptyFieldsMessage);
                return false;
            }
            else if (!IsTermsChecked)
            {
                // Validate Terms and Conditions
                if (OnError != null) OnError(AppResources.AssociateCardErrorTermsConditionsTitle,
                    AppResources.AssociateCardErrorTermsConditionsMessage);
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Clears all the view forms.
        /// </summary>
        public void ClearForm()
        {
            CardNumber = IDNumber = string.Empty;
            IsTermsChecked = false;
            IsIDTypeInitialized = false;
        }

        /// <summary>
        /// Build an input object for a card association.
        /// </summary>
        /// <returns></returns>
        private CardAssociationIn BuildCardAssociationObject()
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
					PharmacyCard = new CardAssociationIn.CardIn { Number = CardNumber }
                }
            };
        }

        #endregion

    }
}
