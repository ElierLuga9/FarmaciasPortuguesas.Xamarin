using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class VoucherAquisitionViewModel : IViewModel
    {

		#region Properties

		private bool _vale_20;
		public bool Vale_20
		{
			get { return _vale_20; }
			set
			{
				_vale_20 = value;
				OnPropertyChanged();
			}
		}


		private bool _vale_10;
		public bool Vale_10
		{
			get { return _vale_10; }
			set
			{
				_vale_10 = value;
				OnPropertyChanged();
			}
		}

		private bool _vale_5;
		public bool Vale_5
		{
			get { return _vale_5; }
			set
			{
				_vale_5 = value;
				OnPropertyChanged();
			}
		}
		private bool _vale_2;
		public bool Vale_2
		{
			get { return _vale_2; }
			set
			{
				_vale_2 = value;
				OnPropertyChanged();
			}
		}

		private string _points;
		public string Points
		{
			get { return _points;}
			set
			{
				_points = value;
				OnPropertyChanged();
			}
		}
        private VoucherPointsMatrixOut.VoucherPointsMatrix _voucher;
        public VoucherPointsMatrixOut.VoucherPointsMatrix Voucher
        {
            get { return _voucher; }
            set
            {
                _voucher = value;
                OnPropertyChanged("Voucher");
            }
        }
		public string _voucherValue;
		private string _vale2Euros = "Vale2Euros";
		private string _vale5Euros = "Vale5Euros";
		private string _vale10Euros = "Vale10Euros";
		private string _vale20Euros = "Vale20Euros";
				
        #endregion

        #region Event Handlers

        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;


		#endregion

		#region Loaders

			public void LoadPage(string voucher)
		{
			if (voucher == _vale2Euros)
			{
				_voucherValue = "V_2_50";
				Vale_2 = true;
				Vale_5 = false;
				Points ="50";
				}else if (voucher == _vale5Euros){
				_voucherValue = "V_5_120";
					Points = "120";
					Vale_5 = true;

					}else if (voucher == _vale10Euros){
				_voucherValue = "V_10_230";
							Vale_10 = true;
							Points = "230";

							}else if (voucher == _vale20Euros){
					_voucherValue = "V_20_440";
								Points = "440";
								Vale_20 = true;
			}
			
		}




		/// <summary>
		/// Request the aquisition of the selected voucher
		/// </summary>
		public async void RequestVoucherAquisition()
        {
            try
            {
                // Call webservice
                var request = await VouchersWS.AquireVoucher(
                    SessionData.PharmacyUser.CardNumber,
                   _voucherValue,
                    SessionData.UserAuthentication);

                // Notify of success
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception e)
            {
                // Service error
                if (OnError != null) OnError(AppResources.VoucherAquisitionPageTitle, e.Message);
            }
        }

        #endregion

    }
}
