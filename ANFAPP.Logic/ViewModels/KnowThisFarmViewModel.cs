using System;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
    public class KnowThisFarmViewModel : IViewModel
    {
        #region

        private string _farmId;
        public string FarmId { get { return _farmId; } set { _farmId = value; OnPropertyChanged(); } }

        private GetMyFarmDetailOut _pharmacyDetail;
        public GetMyFarmDetailOut PharmacyDetail
        {
            get
            {
                return _pharmacyDetail;
            }
            set
            {
                _pharmacyDetail = value;
				OnPropertyChanged ();
            }
        }

        public OnErrorEventHandler OnLoadError;
        public OnLoadStartEventHandler OnLoadStart;
        public OnSuccessEventHandler OnLoadSuccess;

        private KnowThisFarmViewModel() : base() { }

		public KnowThisFarmViewModel(string farmId) : this()
        {
            FarmId = farmId;
        }

        #endregion
        public async Task LoadData()
        {
            if (OnLoadStart != null) await OnLoadStart();
            try
            {
                var result = await ECommerceWS.GetMyFarmDetail(SessionData.UserAuthentication, FarmId);
                PharmacyDetail = result;

            }
            catch (Exception ex)
            {
                if (OnLoadError != null)
                    OnLoadError("", ex.Message);
            }

            if (OnLoadSuccess != null) OnLoadSuccess();
        }
    }
}
