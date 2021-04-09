using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Network.Services
{
    public class FamilyAccountWS : AnfWS
    {

        /// <summary>
        /// Requests a card association, or family account creation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<AssociationRequestOut> RequestCardAssociation(string request)
        {
            // Call web service async
            var response = await Client.PostJson(Settings.WS_FAMILY_REQUEST_CARD_ASSOCIATION, request, SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<AssociationRequestOut>(response.Message);
        }

        /// <summary>
        /// Respond to a card association request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> RespondToCardAssociation(string cardNumber, string request)
        {
            // Call web service async
			var response = await Client.PatchJson(
                string.Format(Settings.WS_FAMILY_RESPOND_TO_CARD_ASSOCIATION, cardNumber), 
                request, 
                SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return response.Message;
        }

        /// <summary>
        /// Requests the list of family members for the referenced card number.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public static async Task<FamilyMembersOut> GetFamilyMembers(string cardNumber)
        {
            // Call web service async
			var response = await Client.Get(
                string.Format(Settings.WS_FAMILY_MEMBERS, cardNumber), 
                SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            return JsonConvert.DeserializeObject<FamilyMembersOut>(response.Message);
        }

        /// <summary>
        /// Requests the list of card association requests.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public static async Task<AssociationRequestsOut> GetAssociationPendingRequests(string cardNumber)
        {
            // Call web service async
			var response = await Client.Get(
                string.Format(Settings.WS_FAMILY_ASSOCIATION_REQUESTS, cardNumber), 
                SessionData.UserAuthentication);

            // Save Authorization, if renewed.
            SessionData.SaveAuthorization(response.Authorization);
            var requests = JsonConvert.DeserializeObject<AssociationRequestsOut>(response.Message);
            if (requests == null || requests.Value == null) return requests;

            // Filter out the non-pending
            requests.Value = requests.Value
                .Where(r => 
                    string.Equals(r.Status, Settings.FAMILY_ASSOCIATION_REQUEST_STATUS_PENDING, StringComparison.CurrentCultureIgnoreCase)
                        && r.Requester != null
                        && !string.Equals(r.Requester.Number, SessionData.PharmacyUser.CardNumber))
                .ToList();

            return requests;
        }
    }
}
