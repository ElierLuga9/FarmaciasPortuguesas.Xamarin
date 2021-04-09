using System;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out;
using System.Threading.Tasks;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
    public class PharmacySelectionViewModel : IViewModel
    {
		private string _pharmacyPhone;
		private string _pharmacyAddress;

        #region Properties

		private CardOut _card;
        private CardOut Card
        {
            get { return _card; }
            set
            {
                _card = value;
                OnPropertyChanged();
            }
        }

        private string _pharmacyname;
        public string PharmacyName 
        {
            get { return _pharmacyname; }
            set
            {
                _pharmacyname = value;
                OnPropertyChanged();
            }
        }

		private bool _haspharmacy = false;
        public bool HasPharmacy
        {
            get { return _haspharmacy; }
            set
            {
                _haspharmacy = value;
                OnPropertyChanged();

            }
        }

		public string PickPharmacyText 
		{
			get { return HasPharmacy ? "ESCOLHER OUTRA FARMÁCIA" : "ESCOLHER UMA FARMÁCIA"; }
		}

        #endregion
        #region Event handlers

        public event OnLoadStartEventHandler OnStart;
        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;

        #endregion

        public async Task LoadSuggestedPharmacy() 
		{
            if (OnStart != null) await OnStart();

            try 
            {
                Card = await UserCardWS.GetSuggestedPharm(SessionData.PharmacyUser.CardNumber);
				var preferredPharmacy = Card.Client.PreferentialPharmacy;
				var hasPharmacy = preferredPharmacy != null;

				if (hasPharmacy) 
                {
					var result = await ECommerceWS.GetMyFarmDetail(SessionData.UserAuthentication, preferredPharmacy);
					PharmacyName = result.Name;
					_pharmacyPhone = result.Phone;
					_pharmacyAddress = result.Address;

					hasPharmacy = result.IsOnlineStore;
                }
				HasPharmacy = hasPharmacy;
            }
            catch(Exception e) 
            {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnError != null) OnError(null, message);
            }

            if (OnSuccess != null) OnSuccess(); 
        }

        public async Task SetUserFarm()
        {
            if (OnStart != null) await OnStart();

            try
            {
				var result = await ECommerceWS.SetMyFarm(SessionData.UserAuthentication, Card.Client.PreferentialPharmacy);

				if (result == null || result.code != 210 && result.code != 205)
				{
					throw new Exception ("De momento não é possivel alterar a sua farmácia.");
				} else {
					SessionData.UpdatePharmacy(Card.Client.PreferentialPharmacy, PharmacyName, _pharmacyPhone, _pharmacyAddress, true);
				}

            }
            catch (Exception e)
            {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnError != null) OnError(null, message);
            }

            if (OnSuccess != null) OnSuccess();
        }
    }
}
