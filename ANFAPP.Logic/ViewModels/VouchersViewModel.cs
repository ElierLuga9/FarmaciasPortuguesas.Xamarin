using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class VouchersViewModel : IViewModel
    {

		#region Inner Classes

		public class VoucherGroup : ObservableCollection<Voucher>
		{

			#region Properties

			public string Title { get; set; }

			#endregion

			public VoucherGroup(string title, List<Voucher> vouchers = null)
			{
				Title = title;

				if (vouchers != null && vouchers.Count > 0)
				{
					foreach (var voucher in vouchers)
					{
						this.Add(voucher);
					}
				}
			}
		}

		#endregion

        #region Properties

        protected bool _userIn;
        public bool UserIn
        {
            get { return _userIn; }
            set
            {
                _userIn = UserIn;
                OnPropertyChanged("UserIn");
            }
        }

        protected VouchersDAO _vouchersDAO = new VouchersDAO();

        private List<Voucher> _vouchersList;
		public List<Voucher> VouchersList
        {
            get { return _vouchersList; }
            set
            {
                _vouchersList = value;
                OnPropertyChanged("VouchersList");
                OnPropertyChanged("HasVouchers");
            }
        }
		private List<Voucher> _vouchersValueList;
		public List<Voucher> VouchersValueList
		{
			get { return _vouchersValueList; }
			set
			{
				
				_vouchersValueList = value;
				OnPropertyChanged("VouchersValueList");
				OnPropertyChanged("HasVouchers");
			}

		}

        private ObservableCollection<VoucherGroup> _vouchers;
		public ObservableCollection<VoucherGroup> Vouchers
        {
            get { return _vouchers; }
            set
            {
                _vouchers = value;
                OnPropertyChanged("Vouchers");
                OnPropertyChanged("HasVouchers");
            }
        }

        #endregion

        #region Event Handlers

        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;
        public OnLoadStartEventHandler OnStart;

        #endregion

        #region Loaders

        /// <summary>
        /// Loads all data
        /// </summary>
		public virtual async Task LoadData(bool fetch = true)
        {

			//VouchersList = new List<Voucher>();
            // Load the list from cache
            await LoadDBVouchers(true,true);

			if (fetch) {
				try {
					// Refresh the wallet contents
					var walletOut = await UserCardWS.GetUserWallet (SessionData.PharmacyUser.CardNumber);
					// Clear previous list
					await _vouchersDAO.DeleteAll ();

					if (walletOut != null && walletOut.Vouchers != null) {
						// Build vouchers list
						var vouchers = new List<Voucher> ();
						foreach (var voucher in walletOut.Vouchers) {
							// Ignore the non-defined vouchers
							if (string.Equals(voucher.Type, Settings.VOUCHER_TYPE_NOTDEFINED)) continue;

							// Add new voucher
							vouchers.Add (new Voucher (voucher));
						}
					
						// Insert all vouchers
						await _vouchersDAO.InsertAll (vouchers); //1
					}

					// Re-list the view
					await LoadDBVouchers(true,true);
				} catch (Exception e) {
					// Show service error
					if (OnError != null)
						OnError (AppResources.VouchersMessageTitle, e.Message);
				}
			}

			// Done
			if (OnSuccess != null) OnSuccess();
        }


        /// <summary>
        /// Loads the gift vouchers
        /// </summary>
        public virtual async Task LoadGiftsData(bool isGiftData, bool fetch = true)
        {
                // Load the list from cache
		//		VouchersList.Clear();
		//		Vouchers.Clear();
                await LoadDBVouchers(isGiftData,false);

                if (fetch)
                {
                    try
                    {
                        // Refresh the wallet contents
                        var walletOut = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);
                        // Clear previous list
                        await _vouchersDAO.DeleteAll();

                        if (walletOut != null && walletOut.Vouchers != null)
                        {
                            // Build vouchers list
                            var vouchers = new List<Voucher>();
                            foreach (var voucher in walletOut.Vouchers)
                            {
							// Ignore the non-defined vouchers
							//if (string.Equals(voucher.Type, Settings.VOUCHER_TYPE_NOTDEFINED)) continue
	                                // Add new voucher
                                vouchers.Add(new Voucher(voucher));
                            }

                            // Insert all vouchers
							
                            await _vouchersDAO.InsertAll(vouchers); //2
                        }

                        // Re-list the view
					await LoadDBVouchers(isGiftData, false);
                    }
                    catch (Exception e)
                    {
                        // Show service error
                        if (OnError != null)
                            OnError(AppResources.VouchersMessageTitle, e.Message);
                    }
                
                // Done
                if (OnSuccess != null) OnSuccess();
            }
        }

		public async Task<bool> LoadFromDBVouchersGifts()
		{ 
			var voucherGroups = new ObservableCollection<VoucherGroup>();

			// Re-list active vouchers
			var vouchersList = await _vouchersDAO.GetAllActiveGifts();
			if (vouchersList != null && vouchersList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.GiftsListHeader, vouchersList));

			Vouchers = (voucherGroups.Count > 0) ? voucherGroups : null;
			if (vouchersList.Count.Equals(0))
			{
				vouchersList.Clear();
				return false;
			}
			else 
			{
				VouchersList = vouchersList;
				return true;
			}
		}
		public bool hasVouchers()
		{
			if (VouchersList.Count.Equals(0) || VouchersList == null) return false;
			else return true;
		}
        /// <summary>
        /// Load the list of vouchers from the database
        /// </summary>
        /// <returns></returns>
        protected async Task LoadDBVouchers(bool isGiftVoucher, bool isBoth)
        {
			var voucherGroups = new ObservableCollection<VoucherGroup>();
			if (isBoth)
			{
				var vouchersList = await _vouchersDAO.GetAllActiveGifts();
				var vouchersListPharm = await _vouchersDAO.GetAllActiveGiftsPharmacy();
				foreach (var voucher in vouchersListPharm)
				{
					vouchersList.Add(voucher);
				}
				List<Voucher> SortedList = vouchersList.OrderBy(o => o.EndDate).ToList();
				if (SortedList != null && SortedList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.GiftsListHeader, SortedList));


				SortedList = await _vouchersDAO.GetAllActiveVouchers();
				if (SortedList != null && SortedList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.VouchersListHeader, SortedList));




				Vouchers = (voucherGroups.Count > 0) ? voucherGroups : null;
				VouchersList = SortedList;

			}
			else
			{

				if (isGiftVoucher)
				{
					// Re-list active vouchers
					var vouchersList = await _vouchersDAO.GetAllActiveGifts();
				//	vouchersList.AddRange(await _vouchersDAO.GetAllActiveGiftsPharmacy());
					var vouchersListPharm = await _vouchersDAO.GetAllActiveGiftsPharmacy();
					foreach (var voucher in vouchersListPharm)
					{
						vouchersList.Add(voucher);
					}
					List<Voucher> SortedList = vouchersList.OrderBy(o => o.EndDate).ToList();

					if (SortedList != null && SortedList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.GiftsListHeader, SortedList));
					Vouchers = (voucherGroups.Count > 0) ? voucherGroups : null;
					VouchersList = SortedList;
				}
				else
				{
					var vouchersList = await _vouchersDAO.GetAllActiveVouchers();
					if (vouchersList != null && vouchersList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.VouchersListHeader, vouchersList));
					Vouchers = (voucherGroups.Count > 0) ? voucherGroups : null;
				//	VouchersList = vouchersList;
					List<Voucher> SortedList = vouchersList.OrderBy(o => o.Value).ToList();
					VouchersValueList = SortedList;
				}

			}



        }


        /// <summary>
        /// Load the list of Gifts
        /// </summary>
        /// <returns></returns>
        /*protected async Task LoadDBVouchersGifts()
        {
            var voucherGroups = new ObservableCollection<VoucherGroup>();

            // Re-list active vouchers
			var vouchersList = await _vouchersDAO.GetAllActiveGifts();
            if (vouchersList != null && vouchersList.Count > 0) voucherGroups.Add(new VoucherGroup(AppResources.GiftsListHeader, vouchersList));

            Vouchers = (voucherGroups.Count > 0) ? voucherGroups : null;

            VouchersList = vouchersList;
        }*/
        #endregion

    }
}
