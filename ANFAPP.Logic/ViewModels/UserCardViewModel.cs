using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Helper;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using Xamarin.Forms;

namespace ANFAPP.Logic.ViewModels
{
    public class UserCardViewModel : IViewModel
    {
		#region Properties


        /// Last Updated
        public DateTime LastUpdated
        {
			get { return SessionData.PharmacyUser.LastUpdated; }
        }

        /// Card Number
        public string CardNumber
        {
            get { return SessionData.PharmacyUser.CardNumber; }
        }

        /// Is Family Card
        public bool IsFamilyCard
        {
            get { return SessionData.PharmacyUser.FamilySize > 0; }
        }

		/// <summary>
		/// Client name
		/// </summary>
		public string Name
		{
			get { return SessionData.PharmacyUser.Name; }
		}

        /// Family Size
        public string FamilySize
        {
            get 
            {
				return SessionData.PharmacyUser.FamilySize + string.Empty;
				//return SessionData.PharmacyUser.FamilySize + " " + 
				//	((SessionData.PharmacyUser.FamilySize == 1) ? AppResources.UserCardFamilySingle : AppResources.UserCardFamilyMultiple);
            }
        }

        /// Has saved amount
        public bool HasSavedAmount {
            get { return SavedAmount > 0; }
        }

        /// Saved Amount
        public decimal SavedAmount
        {
            get { return SessionData.PharmacyUser.SavedAmount; }
        }

        /// Points Amount
        public int Points
        {
            get { return SessionData.PharmacyUser.Points; }
        }

        /// Expiring points
        public int ExpiringPoints
        {
            get { return SessionData.PharmacyUser.ExpiringPoints; }
        }

		#region Vouchers

        /// Vouchers Count
        private int _vouchers;
        public int Vouchers
        {
            get { return _vouchers; }
            set
            {
                _vouchers = value;
                OnPropertyChanged("Vouchers");
                OnPropertyChanged("HasVouchersAmount");
            }
        }

        /// Has Vouchers Amount
        public bool HasVouchersAmount
        {
            get { return Vouchers > 0; }
        }

        /// Vouchers Amount
        private double _vouchersAmount;
        public double VouchersAmount
        {
            get { return _vouchersAmount; }
            set
            {
                _vouchersAmount = value;
                OnPropertyChanged("VouchersAmount");
            }
        }

        /// Expiring vouchers
        private int _expiringVouchers;
        public int ExpiringVouchers
        {
            get { return _expiringVouchers; }
            set
            {
                _expiringVouchers = value;
                OnPropertyChanged("ExpiringVouchers");
            }
        }

		#endregion

		#region Gifts

		/// Gifts Count
		private int _gifts;
		public int Gifts
		{
			get { return _gifts; }
			set
			{
				_gifts = value;
				OnPropertyChanged("Gifts");
				OnPropertyChanged("HasGiftsAmount");
			}
		}

		/// Has Gifts Amount
		public bool HasGiftsAmount
		{
			get { return Gifts > 0; }
		}

		/// Gifts Amount
		private double _giftsAmount;
		public double GiftsAmount
		{
			get { return _giftsAmount; }
			set
			{
				_giftsAmount = value;
				OnPropertyChanged("GiftsAmount");
			}
		}

		/// Expiring Gifts
		private int _expiringGifts;
		public int ExpiringGifts
		{
			get { return _expiringGifts; }
			set
			{
				_expiringGifts = value;
				OnPropertyChanged("ExpiringGifts");
			}
		}

		#endregion

        /// Has Family Warning
        private bool _hasFamilyWarning;
        public bool HasFamilyWarning
        {
            get { return _hasFamilyWarning; }
            set
            {
                _hasFamilyWarning = value;
                OnPropertyChanged("HasFamilyWarning");
            }
        }

        #endregion

        #region Event Handlers

        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;

        #endregion

        #region Loaders

        /// <summary>
        /// Loads and updates the user data.
        /// </summary>
        public async void LoadData()
        {
            // Reload the cached data
            await ReloadCachedData();

            try
            {
                // Update user info
                var userInfo = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
                UpdateUserProfile(userInfo);

                // Update the Wallet & Vouchers
                var wallet = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);
                await UpdateVouchers(wallet);

                // Load family info if a family card
                if (SessionData.PharmacyUser.IsFamilyCard)
                {
                    // Get the list of family members
                    var familyMembers = await FamilyAccountWS.GetFamilyMembers(SessionData.PharmacyUser.CardNumber);

                    // Update the family size
                    int size = (familyMembers != null && familyMembers.Value != null) ? familyMembers.Value.Count : 0;
                    SessionData.PharmacyUser.FamilySize = size;
                    SessionData.SaveUser();
                }

                // Reload the data
                await ReloadCachedData();

                // Check for notifications
                //if (SessionData.PharmacyUser.IsFamilyHead)
                //{
                // Get the family alerts
                var alerts = await FamilyAccountWS.GetAssociationPendingRequests(SessionData.PharmacyUser.CardNumber);
                HasFamilyWarning = alerts.Value != null && alerts.Value.Count > 0;
                //}

                // Notify Success
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception) { /* Fail silently */ }
        }

        /// <summary>
        /// Load the vouchers data
        /// </summary>
        private async Task<bool> LoadVouchersData()
        {
            var dao = new VouchersDAO();

			/// Vouchers 

            // Get all actives
            var activeVouchers = await dao.GetAllActiveVouchers();
            var expiringVouchers = await dao.GetAllExpiringVouchers();

			ExpiringVouchers = expiringVouchers.Count;

            /// Initialize counters
            Vouchers = activeVouchers != null ? activeVouchers.Count : 0;
            VouchersAmount = 0;

            if (Vouchers > 0)
            {
                // Calculate vouchers amount
                foreach (var voucher in activeVouchers)
                {
                    if (voucher.IsCash) VouchersAmount += voucher.Value;
                }
            }

			/// Gifts
			activeVouchers = await dao.GetAllActiveGifts();
			expiringVouchers = await dao.GetAllExpiringGifts();

			ExpiringGifts = expiringVouchers.Count;

			/// Initialize counters
			Gifts = activeVouchers != null ? activeVouchers.Count : 0;
			GiftsAmount = 0;

			if (Gifts > 0)
			{
				// Calculate gifts amount
				foreach (var gift in activeVouchers)
				{
					if (gift.IsCash) GiftsAmount += gift.Value;
				}
			}

            return true;
        }

	
		public async Task LoadFavorites()
		{
			Location loc = SessionData.UserLocation;
			try
			{
				var teste = await UserCardWS.GetFavPharm(SessionData.PharmacyUser.CardNumber);
				SessionData.FavoritePharmacies.Clear();
				foreach (var b in teste)
				{
					if (b.Type.Equals("Favorite") ) { 
						
						var p = await PharmacyWS.GetPharmacyDetail(b.pharmacy.Code);
						var getFarmDetail = await ECommerceWS.GetMyFarmDetail(SessionData.UserAuthentication, "" +b.pharmacy.Code);
						DateTime updated = new DateTime(2000, 01, 01).ToUniversalTime();
						try
						{
							updated = DateTime.Parse(p.UpdatedOn, CultureInfo.InvariantCulture);
						}
						catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine(ex.Message);
						}
						Pharmacy farm = new Pharmacy()
						{

							Code = b.pharmacy.Code,
							Name = p.Name,
							PostalCode = p.PostalCode,
							Address = p.Address,
							Phone = p.Phone,
							PFPSubscriber = true,
							Latitude = p.GeoCoordinates.Latitude,
							Longitude = p.GeoCoordinates.Longitude,
							UpdatedOn = updated,
							LastVisitedOn = new DateTime(2000, 01, 01),
							WorkSchedules = p.WorkSchedules,
							NonWorkingPeriod = p.NonWorkingPeriods,
							CheckEC = getFarmDetail.IsOnlineStore,
							HasEC = getFarmDetail.IsOnlineStore,
							IsFavourite = true
						};
						PharmacyHelper.SetPharmStatusTxt(farm);
						//farm.DistanceTxt = "Distancia: " + PharmacyHelper.DistanceTo(loc.Latitude, loc.Longitude, farm.Latitude, farm.Longitude);
						SessionData.FavoritePharmacies.Add(farm);
					}
						
				}
			}
			catch { 
				/* Do nothing on failure */ 
			}
		}

		
	

		public async Task LoadUserProfile()
		{
			try
			{
				var userInfo = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
				UpdateUserProfile(userInfo);

				if (OnSuccess != null) OnSuccess();
			}
			catch { /* Do nothing on failure */ }
		}

        /// <summary>
        /// Update the user profile.
        /// </summary>
        /// <param name="profileOut"></param>
        private void UpdateUserProfile(UserProfileOut profileOut)
        {
            if (profileOut == null) return;

			DateTime date = new DateTime(1);
			if (null != profileOut.Client) {
				date = DateTime.Parse(profileOut.Client.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
				date = date.ToUniversalTime();
			}

            // Update values
			var user = SessionData.PharmacyUser;
			user.CardNumber = profileOut.Client.PharmacyCard.Number;
			user.ExpiringPoints = profileOut.Client.PharmacyCard.Wallet.PointsToPrescribe;
			user.SavedAmount = profileOut.Client.PharmacyCard.Wallet.TotalValueFolded;
			user.Points = profileOut.Client.PharmacyCard.Wallet.PointsBalance;
			user.IsFamilyCard = profileOut.Client.PharmacyCard.IsFamilyCard;
			user.IsFamilyHead = profileOut.Client.PharmacyCard.IsFamilyHead;
			user.LastUpdated = DateTime.Now;
			user.Name = profileOut.Client.Name;
			user.Birthdate = date;
                
            // Save updated user
            SessionData.SaveUser(profileOut, null);
        }

        /// <summary>
        /// Update the vouchers of the current user.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateVouchers(WalletOut wallet)
        {
            if (wallet == null) return;

            // Clear previous cache
            var dao = new VouchersDAO();
            await dao.DeleteAll();

            // Clear local properties

            if (wallet.Vouchers == null || wallet.Vouchers.Count == 0) return;

            // Build list
            var vouchers = new List<Voucher>();
            foreach (var voucher in wallet.Vouchers) 
            {
                // Add new voucher
                vouchers.Add(new Voucher(voucher));
            }

            // Save Vouchers
            await dao.InsertAll(vouchers);
        }

        /// <summary>
        /// Load the data on the local cache.
        /// </summary>
        private async Task<bool> ReloadCachedData()
        {
			NotifyUI ();

            // Load Vouchers
            await LoadVouchersData();
            return true;
        }

		private void NotifyUI()
		{
			// Notify the interface to the change
			OnPropertyChanged("LastUpdated");
			OnPropertyChanged("CardNumber");
			OnPropertyChanged("IsFamilyCard");
			OnPropertyChanged("Name");
			OnPropertyChanged("FamilySize");
			OnPropertyChanged("HasSavedAmount");
			OnPropertyChanged("SavedAmount");
			OnPropertyChanged("Points");
			OnPropertyChanged("ExpiringPoints");
			OnPropertyChanged("ExpiringVouchers");
		}

        #endregion

    }
}
