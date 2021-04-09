using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Network.Services
{
	public class BioWS : I9WS
	{
		public static async Task<GetBioOut> GetBio(string cardNumber, DateTime date)
		{
			string requestUrl = Settings.BIO_ENDPOINT + "GetBio";

			// Build request message
			string requestBody = JsonConvert.SerializeObject(new GetBioIn()
				{
					Card = int.Parse(cardNumber),
					Date = date.ToString("o"),
				});
					
			// Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
			var responseOut = JsonConvert.DeserializeObject<GetBioOut> (response.Message);

			return responseOut;
		}
	}
}
