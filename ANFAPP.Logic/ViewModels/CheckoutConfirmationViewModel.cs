using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Logic.ViewModels
{
	public class CheckoutConfirmationViewModel : BaseCheckoutViewModel
	{

		#region Event Handlers

		public OnErrorEventHandler OnValidationError;
		public OnSuccessEventHandler OnValidationSuccess;

		#endregion

		#region Service Calls

		/// <summary>
		/// Loads the basket from the webservice
		/// </summary>
		public async Task StartCheckout()
		{
			if (OnLoadStart != null) await OnLoadStart();

			await StartCheckoutAsync();
		}

		/// <summary>
		/// Call the webservice and load the basket.
		/// </summary>
		/// <returns></returns>
		private async Task StartCheckoutAsync()
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
			}
			catch (Exception e)
			{
				// Clear the basket
				Basket = null;

				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			// Load User Info
			if (SessionData.UserProfileForCheckout == null)
			{				
				bool hasProfile = true;
				UserProfileOut userProfile = null;
			
				try
				{
					// Try to load the user profile
					userProfile = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
				}
				catch { hasProfile = false; }

				if (!hasProfile)
				{ 
					try
					{
						// If the user has no card, try to get the profile with no card
						userProfile = await UserCardWS.GetUserProfileNoCard(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
					} catch { }
				}

				SessionData.UserProfileForCheckout = userProfile;
			}

			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		public void TrackMixPanelCheckoutConfirmation() 
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();

			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("PharmacyID", SessionData.StorePharmacyId);

//			string cnps = string.Empty;
//			string cnpems = string.Empty;
//			string quantities = string.Empty;
//			string prices = string.Empty;
//
//			if (Basket.Products != null) {
//				foreach (var product in Basket.Products) {
//					cnps += product.CNP + "_";
//					cnpems += product.CNPEM + "_";
//					quantities += product.Quantity + "_";
//					prices += product.AquisitionType == Settings.CHECKOUT_AQUISITIONTYPE_MONEY ? product.Price + "€_" : product.Points + " points_";
//				}
//
//				// Removes the last underscore
//				cnps = cnps.Length > 0 ? cnps.Substring(0, cnps.Length - 1) : cnps;
//				cnpems = cnpems.Length > 0 ? cnpems.Substring(0, cnpems.Length - 1) : cnpems;
//				quantities = quantities.Length > 0 ? quantities.Substring(0, quantities.Length - 1) : quantities;
//				prices = prices.Length > 0 ? prices.Substring(0, prices.Length - 1) : prices;
//			}

//			props.Add("CNP", cnps);
//			props.Add("CNPEM", cnpems);
//			props.Add("Quantity", quantities);
//			props.Add("Price", prices);
			mixpanelWidget.TrackProperties("CheckoutConfirmed", props);
		}

		/// <summary>
		/// Switch the payment mode for a a product
		/// </summary>
		/// <param name="product"></param>
		public async void SwitchPaymentMode(BasketProductOut product)
		{
			var newVal = !product.MonetaryAquisition;

			try
			{
				// Invert the value
				product.MonetaryAquisition = newVal;

				// Call WS
				var result = await ECommerceWS.EditBasketProduct(
					SessionData.UserAuthentication, 
					product.LineNumber.Value, 
					null, // Null or quantity?
					product.AquisitionType);
				if (result.code != ECommerceWS.CODE_MG_CREATED)
				{
					product.MonetaryAquisition = !newVal;

					// Operation failed
					throw new ServiceErrorException(result.msg);
				}
			}
			catch (Exception e)
			{
				product.MonetaryAquisition = !newVal;

				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError(null, message);
				return;
			}

			// Reload checkout
			await StartCheckoutAsync();
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		/// <summary>
		/// Validate if the user has enough points to proceed with the transaction.
		/// </summary>
		public async void ValidatePoints()
		{
			if (Basket == null) return;

			// Validate if the checkout has any points.
			if (!Basket.TotalUsedPoints.HasValue || Basket.TotalUsedPoints == 0)
			{
				if (OnValidationSuccess != null) OnValidationSuccess();
				return;
			}

			// Validate if the user has a pharmacy card.
			if (!SessionData.HasPharmacyCard) 
			{
				if (OnValidationError != null) OnValidationError(null, AppResources.CheckoutConfirmationNoCardErrorMessage);
				return;
			}

			// Call WS to refresh the points total
			var userInfo = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
			if (userInfo != null && userInfo.Client != null && userInfo.Client.PharmacyCard != null && 
				userInfo.Client.PharmacyCard.Wallet.PointsBalance >= Basket.TotalUsedPoints)
			{
				// User has enough points to complete the transaction
				if (OnValidationSuccess != null) OnValidationSuccess();
			}
			else
			{
				// User doesn't have enough points to complete the transaction.
				if (OnValidationError != null) OnValidationError(null, AppResources.CheckoutConfirmationNoPointsErrorMessage);
			}
		}

		#endregion

	}
}