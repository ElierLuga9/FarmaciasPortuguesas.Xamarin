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
    public class AquireVoucherListViewModel : IViewModel
    {

		#region Properties
		private bool _button2;
		public bool Button2
		{
			get { return _button2; }
			set
			{
				_button2 = value;
				OnPropertyChanged();
			}
		}

		private bool _button5;
		public bool Button5
		{
			get { return _button5; }
			set
			{
				_button5 = value;
				OnPropertyChanged();
			}
		}

		private bool _button10;
		public bool Button10
		{
			get { return _button10; }
			set
			{
				_button10 = value;
				OnPropertyChanged();
			}
		}

		private bool _button20;
		public bool Button20
		{
			get { return _button20; }
			set
			{
				_button20 = value;
				OnPropertyChanged();
			}
		}

		private bool _cartaoVale20Disable;
		public bool CartaoVale20Disable
		{
			get { return _cartaoVale20Disable; }
			set
			{
				_cartaoVale20Disable = value;
				OnPropertyChanged();
			}
		}

		private bool _cartaoVale20;
		public bool CartaoVale20
		{
			get { return _cartaoVale20; }
			set
			{
				_cartaoVale20 = value;
				OnPropertyChanged();
			}
		}


		private bool _cartaoVale5Disable;
		public bool CartaoVale5Disable
		{
			get { return _cartaoVale5Disable; }
			set
			{
				_cartaoVale5Disable = value;
				OnPropertyChanged();
			}
		}

		private bool _cartaoVale5;
		public bool CartaoVale5
		{
			get { return _cartaoVale5; }
			set
			{
				_cartaoVale5 = value;
				OnPropertyChanged();
			}
		}



		private bool _cartaoVale2Disable;
		public bool CartaoVale2Disable
		{
			get { return _cartaoVale2Disable; }
			set
			{
				_cartaoVale2Disable = value;
				OnPropertyChanged();
			}
		}

		private bool _cartaoVale2;
		public bool CartaoVale2
		{
			get { return _cartaoVale2; }
			set
			{
				_cartaoVale2 = value;
				OnPropertyChanged();
			}
		}
		private bool _cartaoVale10Disable;
		public bool CartaoVale10Disable
		{
			get { return _cartaoVale10Disable;}
			set
			{
				_cartaoVale10Disable = value;
				OnPropertyChanged();
			}
		}
		private bool _cartaoVale10;
		public bool CartaoVale10
		{
			get { return _cartaoVale10;}
			set
			{
				_cartaoVale10 = value;
				OnPropertyChanged();
			}
		}
        private List<VoucherPointsMatrixTuple> _voucherOffers;
        public List<VoucherPointsMatrixTuple> VoucherOffers
        {
            get { return _voucherOffers; }
            set
            {
                _voucherOffers = value;
                OnPropertyChanged("VoucherOffers");
            }
        }

        private bool _atLeastOneAvailable;
        public bool AtLeastOneAvailable
        {
            get { return _atLeastOneAvailable; }
            set 
            { 
                _atLeastOneAvailable = value; 
                OnPropertyChanged ("AtLeastOneAvailable");
            }
        }


        #endregion

        #region Event Handlers

        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;



		#endregion

		#region Loaders

		/// <summary>
		/// Load the list of voucher offers from the web service.
		/// </summary>
		/*    public async void LoadData()
			{
				try
				{
					var matrix = await VouchersWS.GetVoucherConvertionMatrix(SessionData.UserAuthentication);

					// Validate response
					if (matrix == null || matrix.Value == null || matrix.Value.Count == 0) return;

					// Filter by voucher type
					var matrixList = matrix.Value.Where(v => string.Equals(v.Type, Settings.VOUCHER_TYPE_VOUCHER, StringComparison.CurrentCultureIgnoreCase)).ToList();
					if (matrixList == null || matrixList.Count == 0) return;

					// Build a list of tuples
					var vouchersList = new List<VoucherPointsMatrixTuple>();
					for (int i = 0; i < matrixList.Count; i += 2)
					{
						var tuple = new VoucherPointsMatrixTuple();

						// Set first voucher offer
						tuple.LeftVoucher = matrixList[i];
						if (i + 1 < matrixList.Count)
						{
							// Set the right voucher if exists
							tuple.RightVoucher = matrixList[i + 1];
						}

						vouchersList.Add(tuple);
					}

					// Mark tuple as last.
					vouchersList[vouchersList.Count - 1].IsLast = true;

					// Update UI availability.
					bool available = false;
					foreach (VoucherPointsMatrixTuple mt in vouchersList) {

						available = mt.LeftVoucher.IsAvailable || mt.RightVoucher != null && mt.RightVoucher.IsAvailable;

						if (available)
							break;
					}
					AtLeastOneAvailable = available;

					// Update bindings
					VoucherOffers = vouchersList;

					// Notify of success
					if (OnSuccess != null) OnSuccess();
				}
				catch (Exception e)
				{
					// Service error
					if (OnError != null) OnError(AppResources.AquireVoucherListPageTitle, e.Message);
				}
			}*/
		public async void LoadData()
		{

			var points = SessionData.PharmacyUser.Points;

			if (points < 50)
			{
				
				CartaoVale2 = false;
				CartaoVale2Disable = true;
				Button2 = false;
			}
			else {
				
				CartaoVale2 = true;
				CartaoVale2Disable = false;
				Button2 = true;
			}

			if (points < 120)
			{
				CartaoVale5 = false;
				CartaoVale5Disable = true;
				Button5 = false;
			}
			else {
				CartaoVale5 = true;
				CartaoVale5Disable = false;
				Button5 = true;
			}



			if (points < 230)
			{
				CartaoVale10 = false;
				CartaoVale10Disable = true;
				Button10 = false;
			}
			else {
				CartaoVale10 = true;
				CartaoVale10Disable = false;
				Button10 = true;
			}

			if (points < 440)
			{
				CartaoVale20 = false;
				CartaoVale20Disable = true;
				Button20 = false;
			}
			else {
				CartaoVale20 = true;
				CartaoVale20Disable = false;
				Button10 = true;
				Button20 = true;
			}
		
		
		}

        #endregion

        #region Inner Class

        public class VoucherPointsMatrixTuple
        {
            public VoucherPointsMatrixOut.VoucherPointsMatrix LeftVoucher { get; set; }
            public VoucherPointsMatrixOut.VoucherPointsMatrix RightVoucher { get; set; }

            public bool IsLast { get; set; }
        }

        #endregion

    }
}
