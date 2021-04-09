using System;
using System.Threading.Tasks;
using System.Globalization;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Models.Out;

namespace ANFAPP.Logic.ViewModels
{
    public class MyPharmacyWidgetViewModel : IViewModel
    {
        private Pharmacy _pharmacy;
        public Pharmacy Pharmacy {
            get { return _pharmacy; }
            set { 
                _pharmacy = value;
                OnPropertyChanged ("Pharmacy");
            }
        }

        public MyPharmacyWidgetViewModel () : base() {}

		public async Task LoadData()
		{
			// Get the pharmacy details if the pharmacy is not the default pharmacy.
			if (!string.IsNullOrEmpty (SessionData.StorePharmacyId)
			    && !string.Equals (SessionData.StorePharmacyId, ANFAPP.Logic.Settings.ST_MG_PHARMACY_ID_DEFAULT)) {
				
				// Do we need to load the pharmacy info?
				if (string.IsNullOrEmpty(SessionData.StorePharmacyName) || SessionData.StorePharmacyName.Equals(Settings.ST_MG_PHARMACY_NAME_DEFAULT)) {
					
					var p = await ECommerceWS.GetMyFarmDetail (SessionData.UserAuthentication, SessionData.StorePharmacyId);

					SessionData.StorePharmacyName = p.Name;
					SessionData.StorePharmacyPhone = p.Phone;
					SessionData.StorePharmacyAddress = p.Address;

					DateTime updated = new DateTime (2000, 01, 01).ToUniversalTime ();
					var lat = p.Location != null ? p.Location.Latitude : 0;
					var lon = p.Location != null ? p.Location.Longitude : 0;

					Pharmacy = new Pharmacy () {
						
						Code = SessionData.StorePharmacyId,
						Name = p.Name,
						PostalCode = p.PostalCode,
						Address = p.Address,
						Phone = p.Phone,
						PFPSubscriber = true,
						Latitude = lat,
						Longitude = lon,
						UpdatedOn = updated,
						HasEC = p.IsOnlineStore,
						LoadFlag = (Math.Abs (lat) < double.Epsilon && Math.Abs (lat) > -double.Epsilon),
						CheckEC = false,
					};
				} else {
					// If the user view the detail, the pharmacy details must be loaded from ANF.
					Pharmacy = new Pharmacy () {
						Code = SessionData.StorePharmacyId,
						Name = SessionData.StorePharmacyName,
						Phone = SessionData.StorePharmacyPhone,
						Address = SessionData.StorePharmacyAddress,

						PFPSubscriber = true,
						HasEC = true,
						LoadFlag = true,
						CheckEC = false,
					};	
				}
			} else {
				// Reset the pharmacy if the pharmacy has changed to the default pharmacy.
				Pharmacy = null;
			}
		}
    }
}
