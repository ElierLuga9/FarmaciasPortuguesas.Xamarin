using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Database.Models;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutFinalStepViewModel : BaseCheckoutViewModel 
    {

		#region Properties

		public string OrderId { get; set; }

		private ObservableCollection<Voucher> _vouchers;
		public ObservableCollection<Voucher> VouchersInCart
		{
			get { return _vouchers; }
			set
			{
				_vouchers = value;
				OnPropertyChanged("VouchersInCart");
			}
		}

		#endregion

		public async Task<CheckoutConfOut> CheckoutConf(string MBWAYPhone = null, bool isMBWAY = false)
		{
			CheckoutConfOut result = null;

			try {
				System.Diagnostics.Debug.WriteLine("MBWAY PHONE:" + MBWAYPhone);
				if (isMBWAY)
				{
					result = await ECommerceWS.CheckoutConf(SessionData.UserAuthentication, MBWAYPhone);
					if (result != null && result.Success)
					{
						OnLoadSuccess();
					}
					else
					{
						if (OnLoadError != null) OnLoadError("",result.msg);
					}
					OrderId = result.OrderId;
				}
				else
				{
					result = await ECommerceWS.CheckoutConf(SessionData.UserAuthentication, null);
				}
				// Update the point's counter with the updated amount
				var userInfo = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
				if (userInfo != null && userInfo.Client != null && userInfo.Client.PharmacyCard != null && userInfo.Client.PharmacyCard.Wallet != null)
				{
					SessionData.PharmacyUser.Points = userInfo.Client.PharmacyCard.Wallet.PointsBalance;
					SessionData.SaveUser();
				}

			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("Conf", message);
			}

			return result;
		}
    }
}
