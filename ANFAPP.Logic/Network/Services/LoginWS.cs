using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Network.Services
{
    public class LoginWS : AnfWS
    {

        /// <summary>
        /// Call the login web service.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public static async Task<AuthorizationOut> Login(string email, string password)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(new AuthorizationIn()
            {
                ApplicationId = Settings.APPLICATION_ID,
                UserName = email,
                Password = password
            });

            // Call web service async
            var response = await Client.PostJson(Settings.WS_AUTHORIZATION, requestMessage);
            return JsonConvert.DeserializeObject<AuthorizationOut>(response.Message);
        }

		/// <summary>
		/// Call the login web service.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		public static async Task<AuthorizationOut> FacebookLogin(string email, string providerToken)
		{
			// Build request message
			string requestMessage = JsonConvert.SerializeObject(new FacebookAuthorizationIn()
			{
				ApplicationId = Settings.APPLICATION_ID,
				UserName = email,
				ProviderToken = providerToken,
				Provider = Settings.FACEBOOOK_PROVIDER
			});

			// Call web service async
			var response = await Client.PostJson(Settings.WS_FACEBOOK_AUTHORIZATION, requestMessage);
			return JsonConvert.DeserializeObject<AuthorizationOut>(response.Message);
		}

        /// <summary>
        /// Register a new card.
        /// </summary>
        /// <param name="inModel"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
        public static async Task<CardRegistrationOut> RegisterCard(CardRegistrationIn inModel)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(inModel,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Call web service async
			var response = await Client.PostJson(Settings.WS_PHARMACY_CARD, requestMessage);
            return JsonConvert.DeserializeObject<CardRegistrationOut>(response.Message);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="inModel"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
		public static async Task<bool> RegisterUser(UserRegistrationIn inModel)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(inModel, 
                Formatting.None, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Call web service async
			await Client.PostJson(Settings.WS_API_USER, requestMessage);
            return true;
        }

		public static async Task<ClientOut> GetCRMClient(string clientNumber, AnfAuthorizationBundle authBundle)
		{
			var requestUrl =  string.Format(Settings.WS_GET_CRM_CLIENT, clientNumber);
			var response = await Client.Get(
				requestUrl,
				authBundle);

			// Save Authorization, if renewed.
			SessionData.SaveAuthorization(response.Authorization);
			return JsonConvert.DeserializeObject<ClientOut>(response.Message);
		}

		public static async Task<ClientOut> CreateCRMClient(string email, string contactPhone)
		{
			// Build request message
			// {
			//		"Email":"ut19_b2c_local@teste.pt",
			//		"ContactPhone": "212121234"
			// }
			var dict = new Dictionary<string, string> {{"Email", email}, {"ContactPhone", contactPhone}};
			string requestMessage = JsonConvert.SerializeObject(dict, 
				Formatting.None, 
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			// Call web service async
			var response = await Client.PostJson(Settings.WS_CREATE_CRM_CLIENT, requestMessage);
			return JsonConvert.DeserializeObject<ClientOut>(response.Message);
		}

		public static async Task<UserProfileOut> CreateCRMProfile(CardRegistrationIn.ClientIn client)
		{
			// Build request message
			//		{  
			//			"User":{  
			//				"UserName":"ut19_b2c_local@teste.pt"
			//			},
			//
			//			"Client" : {
			//				"Email":"ut19_b2c_local@teste.pt",
			//				"ContactPhone": "212121234"
			//			}
			//		}


			var dict = new Dictionary<string, IDictionary<string, object>> ();
			dict.Add ("User", new Dictionary<string,object>{ {"UserName", client.Email} });
			dict.Add ("Client", new Dictionary<string,object> { 
				{"Email", client.Email},
				{"ContactPhone", client.ContactPhone}, 
				{"Name", client.Name},
				{"BirthDate", client.BirthDate},
				{"Address", client.Address},
				{"PostalCode", client.PostalCode},
				{"DocumentType", client.DocumentType}, 
				{"DocumentNumber", client.DocumentNumber},
				{"HouseholdSize", client.HouseholdSize},
				{"FiscalNumber", ""},
				{"Gender", client.Gender}
			});
			string requestMessage = JsonConvert.SerializeObject(dict, 
				Formatting.None, 
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			// Call web service async
			var response = await Client.PostJson(Settings.WS_CREATE_CRM_PROFILE, requestMessage);
			return JsonConvert.DeserializeObject<UserProfileOut>(response.Message);
		}

        /// <summary>
        /// Associate a card to a user
        /// </summary>
        /// <param name="inModel"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
		public static async Task<bool> AssociateCard(CardAssociationIn inModel)
        {
            // Build request message
            string requestMessage = JsonConvert.SerializeObject(inModel);

            // Call web service async
			await Client.PostJson(Settings.WS_CARD_ASSOCIATION, requestMessage);
            return true;
        }

        /// <summary>
        /// Password recover request.
        /// </summary>
        /// <param name="inModel"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
		public static async Task<bool> RecoverPassword(string email)
        {
            // Call web service async
			await Client.PostJson(string.Format(Settings.WS_PASSWORD_RECOVERY, email), null);
            return true;
        }

        /// <summary>
        /// Change the user password.
        /// </summary>
        /// <param name="inModel"></param>
        /// <param name="authBundle"></param>
        /// <returns></returns>
		public static async Task<bool> ChangePassword(string username, string oldPassword, string newPassword, AnfAuthorizationBundle authBundle)
        {
            // Build request object
            var requestObj = new ChangePasswordIn()
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword
            };

            // Call web service async
			await Client.PostJson(
                string.Format(Settings.WS_CHANGE_PASSWORD, username), 
                JsonConvert.SerializeObject(requestObj), 
                authBundle);

            return true;
        }
    }
}
