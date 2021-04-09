using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Invokers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.Network.Services.Abstract
{
	public abstract class I9WS
	{
			
		#region 

		protected static RestInvoker Client { get { return I9RestClient.Instance; } }

		#endregion

		#region Authentication

		/// <summary>
		/// I9 WebServices authentication.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="deviceToken"></param>
		/// <param name="authBundle"></param>
		/// <returns></returns>
		public static async Task<GetTokenOut> Authenticate(string email, string deviceToken, AnfAuthorizationBundle authBundle)
		{
			string requestUrl = Settings.TOMAS_ENDPOINT + "GetTokenAzure";

			// Build request message
			string requestBody = JsonConvert.SerializeObject(new GetTokenIn()
			{
				UserID = email,
				Device = deviceToken,
				DeviceType = Device.OnPlatform("ios", "android", "winphone"),
				Card = SessionData.HasPharmacyCard ? SessionData.PharmacyUser.CardNumber : null
			});

			// Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, authBundle);
			var responseOut = JsonConvert.DeserializeObject<GetTokenOut>(response.Message);

			return responseOut;
		}

		public static async Task<UpdateNotificationTokenOut> UpdateDeviceToken(string email, string NewToken, string OldToken)
		{
			string requestUrl = Settings.TOMAS_ENDPOINT + "UpdateDeviceToken";

			// Build request message
			string requestBody = JsonConvert.SerializeObject(new UpdateNotificationTokenIn() {
				UserID = email,
				NewToken = NewToken,
				OldToken = OldToken,
				DeviceType = Device.OnPlatform("ios", "android", "winphone"),
				Card = SessionData.HasPharmacyCard ? SessionData.PharmacyUser.CardNumber : null
			});

			// Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
			var responseOut = JsonConvert.DeserializeObject<UpdateNotificationTokenOut>(response.Message);

			return responseOut;
		}

		#endregion

	}
}
