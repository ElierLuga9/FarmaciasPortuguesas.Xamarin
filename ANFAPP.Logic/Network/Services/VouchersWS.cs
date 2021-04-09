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
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Logic.Network.Services
{
    public class VouchersWS : AnfWS
    {

        /// <summary>
        /// Get the user profile
        /// </summary>
        /// <param name="username"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
        public static async Task<VoucherPointsMatrixOut> GetVoucherConvertionMatrix(AnfAuthorizationBundle authBundle)
        {
            // Call web service async
            var response = await Client.Get(Settings.WS_VOUCHERS_POINTS_MATRIX, authBundle);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<VoucherPointsMatrixOut>(response.Message);
        }
		//USERCARD_PROMO_ENDPOINT
		/// <summary>
		/// Get promo voucher
		/// </summary>
		/// <param name="cardId"></param>
		/// <returns></returns>
		public static async Task<ClientePromoCrmOut> GetVoucherPromo(string cardId, AnfAuthorizationBundle authBundle)
		{
			
			var response = await Client.Get(string.Format(Settings.PROMO_VOUCHER, cardId), SessionData.UserAuthentication);

			SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<ClientePromoCrmOut>(response.Message);



		}

		/// <summary>
		/// Get promo voucher
		/// </summary>
		/// <param name="cardId"></param>
		/// <returns></returns>
		public static async Task<CardPromoOut> GetUserCardPromo()
		{

			var response = await Client.Get(string.Format(Settings.USERCARD_PROMO_ENDPOINT), null);

			//SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<CardPromoOut>(response.Message);



		}
        /// <summary>
        /// Request a voucher aquisition
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="voucherReference"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
        public static async Task<VoucherPurchaseOut> AquireVoucher(string cardNumber, string voucherReference, AnfAuthorizationBundle authBundle)
        {
            // Build request and target card
            var requestcard = new VoucherAquisitionIn.CardIn()
            {
                Number = cardNumber
            };

            // Build request object
            var requestObject = new VoucherAquisitionIn()
            {
                Pharmacy = new VoucherAquisitionIn.PharmacyIn() 
                {
                    Code = Settings.VOUCHER_AQUISITION_PHARMACY_CODE
                },
                SourceCard = requestcard,
                TargetCard = requestcard,
                VoucherReference = voucherReference,
                PaymentType = Settings.VOUCHER_AQUISITION_PAYMENT_TYPE
            };

            // Call web service async
            var response = await Client.PostJson(Settings.WS_VOUCHER_AQUISITION, JsonConvert.SerializeObject(requestObject), authBundle);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<VoucherPurchaseOut>(response.Message);
        }
    }
}
