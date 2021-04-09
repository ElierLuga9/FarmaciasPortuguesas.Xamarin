using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutVouchersViewModel : VouchersViewModel
	{
		#region Properties

		private CheckoutStartOut Basket { get; set; }

		public OnLoadStartEventHandler OnLoadStart;

		#endregion

		public CheckoutVouchersViewModel(CheckoutStartOut basket)
		{
			this.Basket = basket;
		}

		#region Loaders

		/// <summary>
		/// Loads the list of vouchers assigned to a user.
		/// </summary>
		/// <param name="selectedVouchers"></param>
		public async Task LoadData(IList<Voucher> selectedVouchers, bool fetch = true)
		{
			await base.LoadData(fetch);

			// Only initialize the selected vouchers if any exist.
			if (selectedVouchers == null || selectedVouchers.Count == 0 || Vouchers == null || Vouchers.Count == 0) return;

			// Mark previously selected vouchers as so, if they still exist.
			foreach (var voucher in selectedVouchers)
			{
				// Find vouchers by group.
				foreach (var group in Vouchers)
				{
					var selected = group.Where(v => string.Equals(v.Code, voucher.Code)).FirstOrDefault();
					if (selected != null) 
					{
						selected.Selected = true;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Marks a voucher as selected if the conditions apply.
		/// </summary>
		/// <param name="voucher"></param>
		public async void ToggleVoucherSelection(Voucher voucher)
		{
			if (voucher == null) return;
			// Adding or removing a voucher has not returned yet.
			if (voucher.Updating) return;

			// If the voucher isn't selected, a validation is required before it can be set as selected.
			bool conditionsMet = false;


			var voucherOut = await UserCardWS.GetUserVoucherDetails(SessionData.PharmacyUser.CardNumber);
			ANFAPP.Logic.Models.Out.VoucherOut.Condition selectedVoucher = null;
			foreach (var vale in voucherOut.Vouchers) 
			{
				if (vale.Code == voucher.Code) 
				{
					selectedVoucher = vale.BurnCondition;
				}
			}

			if (selectedVoucher.IsPharmacyExclusive && selectedVoucher.ExclusivePharmacies.Count != 0)
			{
				
				foreach (var pharm in selectedVoucher.ExclusivePharmacies)
				{
					if (pharm.Code != (int.Parse(SessionData.StorePharmacyId))) {
						OnError(null, AppResources.CheckoutAddVoucherPharmacyErrorMessage);
						return; 
					}

				}
				if (!string.IsNullOrEmpty(voucher.PharmacyExclusiveId) && !string.Equals(voucher.PharmacyExclusiveId, SessionData.StorePharmacyId))
				{
					OnError(null, AppResources.CheckoutAddVoucherPharmacyErrorMessage);
					return;
				}
			}
			var burnConditions = voucher.BurnConditions;
			if (!string.IsNullOrEmpty (burnConditions)) {

				// The voucher has CNP restrictions, so make sure a product exists in the basket within the allowed CNP range.
				foreach (var prod in Basket.Products) {
					conditionsMet = prod.CNP.HasValue && burnConditions.Contains(prod.CNP.Value + string.Empty);
					if (conditionsMet)
						break;
				}
			} else {
				conditionsMet = true;
			}
			if (!voucher.Selected && !conditionsMet) {
				// Conditions not met, so an error will be thrown.
				OnError(null, AppResources.CheckoutAddVoucherCNPErrorMessage);
				return;
			}


			try {
				voucher.Updating = true;
				if (null != OnLoadStart) OnLoadStart();

				if (voucher.Selected) {
					// Call the WS to delete the voucher.
					await ECommerceWS.DeleteVoucherFromCheckout (voucher.Code, SessionData.UserAuthentication);
				} else {
					// Call the WS to add the voucher. This must be called whenever a voucher is added or removed to ensure that business rules are applied.
					await ECommerceWS.AddVoucherToCheckout (voucher.Code, SessionData.UserAuthentication);
				}

				// Invert the selection state of the voucher if the change was successful.
				voucher.Selected = !voucher.Selected;

				if (null != OnSuccess) OnSuccess();

			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnError != null) OnError(null, message);
			} finally {
				voucher.Updating = false;

				// Force the list to update it's layout.. because Xamarin.
				Vouchers = new ObservableCollection<VoucherGroup>(Vouchers);			
			}
		}

		/// <summary>
		/// Returns the list of selected vouchers
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Voucher> GetSelectedVouchers()
		{
			if (Vouchers == null || Vouchers.Count == 0) return null;

			var collection = new ObservableCollection<Voucher>();
			foreach (var voucherGroup in Vouchers)
			{
				foreach (var voucher in voucherGroup) {
					if (voucher.Selected) collection.Add(voucher);
				}
			}

			return (collection.Count > 0) ? collection : null;
		}

		#endregion
	}
}
