using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.In.Ecommerce;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.EventHandlers;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Objects;

namespace ANFAPP.Logic.ViewModels
{
    public class BannerViewModel : IViewModel
    {
        private double _widthHint;
        private double _heightHint;

		#region

		private List<BannerOut> _banners;
		public List<BannerOut> Banners { 
            get { return _banners; }
            set { 
                _banners = value; 
                OnPropertyChanged ("Banners");
            }
        }

		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadComplete;
		public OnErrorEventHandler OnError;
            
		#endregion

        private BannerViewModel () : base() { }

        public BannerViewModel(double width, double height) {
            _widthHint = width;
            _heightHint = height;
        }

		public async Task LoadData(double? width = null, double? height = null, Location location = null)
        {
			if (width != null)
				_widthHint = width.GetValueOrDefault ();
			if (height != null)
				_heightHint = height.GetValueOrDefault ();

			if (null != OnLoadStart) await OnLoadStart ();

			try {
				if (location != null) await LoadDistrictWithLocation(location);
				await LoadBannersAsync(SessionData.SAFEDistrictId);

				if (null != OnLoadComplete) OnLoadComplete();

			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex);

				if (null != OnError)
					OnError (null, ex.Message);
			}
        }

		public async Task LoadDistrictWithLocation(Location location) {
			var result = await ECommerceWS.GetSAFEDistrict(location.Latitude, location.Longitude, SessionData.UserAuthentication);
			if (result != null) SessionData.SAFEDistrictId = result.DistrictId;
		}

		public async Task LoadBannersAsync(Nullable<int> districtId = null) 
		{
			//var result = await ECommerceWS.GetBanners(2, (int)_widthHint * 2, null, 3, BannerFilter.Category, 0, SessionData.UserAuthentication);
			var result = await ECommerceWS.GetSAFEBanners(2, (int)_widthHint * 2, null, 3, BannerFilter.Category, 0, districtId, SessionData.UserAuthentication);
			Banners = result.Banners;
		}

    }
}
