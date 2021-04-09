using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.Models;
using System.Globalization;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Models.In.Ecommerce;

namespace ANFAPP.Logic.Network.Services
{
    public class UserCardWS : AnfWS
    {

        /// <summary>
        /// Get the user profile
        /// </summary>
        /// <param name="username"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
        public static async Task<UserProfileOut> GetUserProfile(string username, AnfAuthorizationBundle authBundle)
        {
            // Call web service async
            var response = await Client.Get(string.Format(Settings.WS_USER_PROFILE, username), authBundle);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<UserProfileOut>(response.Message);
        }

		public static async Task<UserProfileOut> GetUserProfileNoCard(string username, AnfAuthorizationBundle authBundle)
		{
			// Call web service async
			var response = await Client.Get(string.Format(Settings.WS_USER_PROFILE_NO_CARD, username), authBundle);

			// Save Authorization, if renewed.
			SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<UserProfileOut>(response.Message);
		}

        /// <summary>
        /// Get the points movement for the referenced card number, in the referenced time interval.
        /// </summary>
        public static async Task<PointsMovementOut> GetPointsMovement(string cardNumber, DateTime startDate, DateTime endDate)
        {
            // Call web service async
			var requestUrl =  string.Format(Settings.WS_USER_POINTS_MOVEMENT, cardNumber, startDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture), endDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
            var response = await Client.Get(
				requestUrl,
                SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<PointsMovementOut>(response.Message);
        }

        /// <summary>
        /// Get the wallet for the referenced card number.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public static async Task<WalletOut> GetUserWallet(string cardNumber)
        {
            // Call web service async
            var response = await Client.Get(string.Format(Settings.WS_USER_WALLET, cardNumber), SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<WalletOut>(response.Message);
        }

		/// <summary>
		/// Get the user Detailed voucher data including BurnCondition extanded data
		/// </summary>
		/// <param name="cardNumber"></param>
		/// <returns></returns>
		public static async Task<WalletOut> GetUserVoucherDetails(string cardNumber)
		{
			// Call web service async
			var response = await Client.Get(string.Format(Settings.WS_GET_VOUCHER_EXPANDED_DETAILS, cardNumber), SessionData.UserAuthentication);

			// Save Authorization, if renewed.
			SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<WalletOut>(response.Message);
		}

		/// <summary>
		/// Get the wallet for the referenced card number.
		/// </summary>
		/// <param name="cardNumber"></param>
		/// <returns></returns>
		public static async Task<WalletOut> GetUserVoucher(string cardNumber,string id)
		{
			// Call web service async
			var response = await Client.Get(string.Format(Settings.WS_USER_VOUCHER, cardNumber, id), SessionData.UserAuthentication);

			// Save Authorization, if renewed.
			SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<WalletOut>(response.Message);
		}
	    /// <summary>
	    /// Update User Card Information.
	    /// </summary>
	    /// <param name="PharmacyCard"></param>
	    /// <returns>PharmacyCard</returns>
        public static async Task<HttpResponseModel> UpdatePharmacyCard(CardRegistrationIn card,string number)
        {
			// Call web service async
			var message = JsonConvert.SerializeObject(card, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = await Client.PutJson(string.Format(Settings.WS_USER_CARD_UPDATE,number), message, SessionData.UserAuthentication);

			return response;
        }

		/// <summary>
		/// Update Client Information.
		/// </summary>
		/// <param name="PharmacyCard"></param>
		/// <returns>PharmacyCard</returns>
		public static async Task<HttpResponseModel> UpdateCRMProfile(CardRegistrationIn.ClientIn client, string number)
		{
			// Call web service async
			var message = JsonConvert.SerializeObject(client, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = await Client.PatchJson(string.Format(Settings.WS_GET_CRM_CLIENT, number), message, SessionData.UserAuthentication);

			return response;
		}

        /// <summary>
        /// Get suggested pharmacy.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>PharmacyCard</returns>
        public static async Task<CardOut> GetSuggestedPharm(string cardnumber)
        {
            // Call web service async
            var response = await Client.Get(string.Format(Settings.WS_USER_SUGESTED_FARM, cardnumber), SessionData.UserAuthentication);

            return JsonConvert.DeserializeObject<CardOut>(response.Message);
        }


        /// <summary>
        /// Get User Pharmacy Information.
        /// </summary>
        /// <param name="PharmacyId"></param>
        /// <returns>Pharmacy</returns>
        public static async Task<Pharmacy> GetUserPharmacy(string PharmacyId)
        {

			SessionData.AuthANFavorites = false;
            // Call web service async
            var response = await Client.Get(string.Format(Settings.WS_GET_PHARMACY, PharmacyId), SessionData.UserAuthentication);
            return JsonConvert.DeserializeObject<Pharmacy>(response.Message);
        }
		public static async Task<SetMyFarmOut> SetFavMyFarm(AnfAuthorizationBundle authBundle, string farmId,string Operation, string Type = "Favorite")
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new SetMyFavFarmIn()
			{
				cardNumber = SessionData.PharmacyUser.CardNumber,
				pharmacyCode = farmId,
				type = Type,
				operation = Operation
			});

			// Call web service async
			SessionData.AuthANFavorites = true;

			var response = await Client.PostJson(Settings.WS_SET_FAVORITES_PHARM, requestMessage, authBundle);
			SessionData.AuthANFavorites = false;
			var obj = JsonConvert.DeserializeObject<SetMyFarmOut>(response.Message);
			//SessionData.SaveAuthorization(authBundle, false);
			// Do not update the pharmacy!
			return obj;
		}

		public static async Task<List<GetFavPharmOut>> GetFavPharm(string cardNumber)
		{
			// Call web service async
			var requestObj = new GetFavPharmIn()
			{
				CardNumber = cardNumber
			};
			System.Diagnostics.Debug.WriteLine(SessionData.UserAuthentication.GetAuthorizationString());
			var auth = SessionData.UserAuthentication;
			string b = SessionData.UserAuthentication.GetAuthorizationString();
			SessionData.AuthANFavorites = true;
			var response = await Client.PostJson(Settings.WS_FAVORITES_PHARM,JsonConvert.SerializeObject(requestObj), SessionData.UserAuthentication);
			SessionData.AuthANFavorites = false;
			//var obj = JsonConvert.DeserializeObject<SearchPAOut>(response.Message);
			// Save Authorization, if renewed.
			//SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<List<GetFavPharmOut>>(response.Message);
		}
    }
}
