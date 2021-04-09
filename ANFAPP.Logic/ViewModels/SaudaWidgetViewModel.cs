using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
    public class SaudaWidgetViewModel :IViewModel
    {

		private bool _isInitialized;
		public bool IsInitialized {
			get { return _isInitialized; }
			set {
				_isInitialized = value;
				OnPropertyChanged("IsInitialized");
			}
		}

		private bool _hasSAFEBanner;
		public bool HasSAFEBanner {
			get { return _hasSAFEBanner; }
			set {
				_hasSAFEBanner = value;
				OnPropertyChanged("HasSAFEBanner");
			}
		}

        private bool _hasCard;
        public bool HasCard {
            get { return _hasCard; }
            set { 
                _hasCard = value;
                OnPropertyChanged ("HasCard");
            }
        }

		private bool _hasExpiringPointsOrVouchers;
		public bool HasExpiringPointsOrVouchers {
			get { return _hasExpiringPointsOrVouchers; }
			set { 
				_hasExpiringPointsOrVouchers = value;
				OnPropertyChanged ("HasExpiringPointsOrVouchers");
			}
		}

        private int _points;
        public int Points {
            get { return _points; }
            set { 
                _points = value;
                OnPropertyChanged ("Points");
            }
        }

        private int _vouchers;
        public int Vouchers {
            get { return _vouchers; }
            set { 
                _vouchers = value;
                OnPropertyChanged ("Vouchers");
            }
        }

		public async Task LoadData()
		{			
			HasCard = SessionData.HasPharmacyCard;
			HasSAFEBanner = SessionData.SAFEDistrictId != null;

			if (HasCard) {

				HasExpiringPointsOrVouchers = SessionData.PharmacyUser.ExpiringPoints > 0;
				if (HasExpiringPointsOrVouchers) {
					Points = SessionData.PharmacyUser.ExpiringPoints;
				} else {
					Points = SessionData.PharmacyUser.Points;
				}

				var wallet = await UserCardWS.GetUserWallet(SessionData.PharmacyUser.CardNumber);

				int expiringVouchers = 0, totalVouchers = wallet.Vouchers != null ? wallet.Vouchers.Count : 0;
				foreach (VoucherOut v in wallet.Vouchers) {
					if (v.ExpiryAlert && !string.Equals(v.Status, "Used"))
						expiringVouchers++;
				}

				HasExpiringPointsOrVouchers = SessionData.PharmacyUser.ExpiringPoints > 0 || expiringVouchers > 0;
				if (HasExpiringPointsOrVouchers) {
					Points = SessionData.PharmacyUser.ExpiringPoints;
					Vouchers = expiringVouchers;
				} else {
					Points = SessionData.PharmacyUser.Points;
					Vouchers = totalVouchers;
				}
			}

			IsInitialized = true;
		}
    }
}

