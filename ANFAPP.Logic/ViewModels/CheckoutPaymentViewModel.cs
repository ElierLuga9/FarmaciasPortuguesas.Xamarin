using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.EventHandlers;
using System.Collections.Generic;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutPaymentViewModel : BaseCheckoutViewModel
	{
		#region Inner Enums

		public enum PaymentMethod { ATM, HIPAY, MBWAY , ON_DELIVERY, FREE_ORDER };

		#endregion
	
		#region

		public OnSuccessEventHandler OnUpdateVoucherSuccess = delegate { };
		public OnSuccessEventHandler OnDeleteVoucherSuccess = delegate { };

		#endregion

		#region Properties

		private ObservableCollection<Voucher> _vouchers;
		public ObservableCollection<Voucher> VouchersInCart
		{
			get { return _vouchers; }
			set
			{
				_vouchers = value;
				OnPropertyChanged("VouchersInCart");
				OnPropertyChanged("ShowVoucherControl");
			}
		}

		public bool ShowVoucherControl 
		{
			get { 
				if (Basket == null)
					return false;

				//return (!Basket.HasMSRM || (VouchersInCart != null && VouchersInCart.Count > 0)); 
				return (VouchersInCart != null && VouchersInCart.Count > 0) || (!Basket.HasMSRM && (Basket.TotalInvoice + Basket.MSRMMaxValue > 0));// && !Basket.HasCNPEM);

			}

		}

		#endregion

		#region Loaders

		public async Task UpdateCheckout()
		{
			if (OnLoadStart != null) await OnLoadStart();

			// XXX: it seems that there is a delay between updating the vouchers and the checkout total.
			await Task.Delay(100);
			await UpdateCheckoutAsync();
		}

		private async Task UpdateCheckoutAsync()
		{
			try
			{
				// Call WS and update the basket
				var result = await ECommerceWS.CheckoutStart(SessionData.UserAuthentication);

				// Clear the taxes with no value
				if (result != null && result.Taxes != null)
				{
					var taxes = new List<IVAOut>();
					foreach (var tax in result.Taxes)
					{
						if (tax.Value > 0) taxes.Add(tax);
					}
					result.Taxes = taxes;
				}

				Basket = result;
				if (null != OnUpdateVoucherSuccess) OnUpdateVoucherSuccess ();
			}
			catch (Exception e)
			{
				// Do not clear the basket!
				// XXX: how do we recover from this?
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}
		}

		public async Task UpdateVouchersInCart()
		{
			try {	
				var result = await ECommerceWS.GetListVouchers(SessionData.UserAuthentication);
				Basket.Vouchers = result.Vouchers;
			} catch (Exception ex) {
				// XXX: Can we safely ignore this? When returning from the Add Page we should
				// have a valid local list of selected vouchers.
				System.Diagnostics.Debug.WriteLine (ex);
			}

			if (null != OnUpdateVoucherSuccess)
				OnUpdateVoucherSuccess ();
		}

		public async Task RemoveVoucherInCart(Voucher v)
		{
			if (null != OnLoadStart) OnLoadStart ();

			try {
				var result = await ECommerceWS.DeleteVoucherFromCheckout(v.Code, SessionData.UserAuthentication);
				if (result.code >= 200 && result.code < 300) {
					// VouchersInCart.Remove(v);
					// We need to update the totals...
					// XXX: it seems that there is a delay between updating the vouchers and the checkout total.
					await Task.Delay(100);
					await UpdateCheckoutAsync();
				}

				if (null != OnLoadSuccess) OnLoadSuccess();

			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			}
		}

		/// <summary>
		/// Changes the payment method. This method does not call the OnLoadSuccess event handler.
		/// </summary>
		/// <param name="paymentMethod"></param>
		public async Task<bool> SetPaymentMode(PaymentMethod paymentMethod)
		{
			bool success = false;
			try
			{
				// XXX - HAMMER TIME!! THis should be paramterized in the WS, not here!!!!!
				//if ((paymentMethod == PaymentMethod.HIPAY || paymentMethod == PaymentMethod.ATM) && Basket.ValueToPay.Value < 2.0m)
				//{
				//	throw new ServiceErrorException(AppResources.CheckoutPaymentHiPayMinLimit);
				//}

				var response = await ECommerceWS.SetCheckoutPaymentMethod(GetPaymentMethod(paymentMethod), SessionData.UserAuthentication);
				success = response.code == 200;
				if (response.code != 200) throw new ServiceErrorException(response.msg);
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
			}

			return success;
		}

		/// <summary>
		/// Returns the code corresponding to the referenced payment method.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		protected string GetPaymentMethod(PaymentMethod paymentMethod)
		{
			switch (paymentMethod)
			{
				case PaymentMethod.ATM: return Settings.CHECKOUT_DELIVERY_ATM;
				case PaymentMethod.HIPAY: return Settings.CHECKOUT_DELIVERY_HIPAY;
				case PaymentMethod.MBWAY: return Settings.CHECKOUT_DELIVERY_MBWAY;
				case PaymentMethod.ON_DELIVERY: return Settings.CHECKOUT_DELIVERY_ON_DELIVERY;
			}

			return null;
		}

		#endregion

	}
}
