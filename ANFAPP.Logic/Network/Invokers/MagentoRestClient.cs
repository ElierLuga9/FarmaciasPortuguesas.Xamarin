using System;
using ANFAPP.Logic.Models.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using ANFAPP.Logic.Exceptions;
using Newtonsoft.Json;
using ANFAPP.Logic.Network.Services.Abstract;
using ANFAPP.Logic.Models.In.Ecommerce;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ANFAPP.Logic.Network.Invokers
{
	public class MagentoRestClient : RestInvoker
	{

		#region Singleton

		private static MagentoRestClient _instance;
		public static MagentoRestClient Instance
		{
			get
			{
				if (_instance == null) _instance = new MagentoRestClient();
				return _instance;
			}
		}

		#endregion

		#region Invokers

		public async Task<HttpResponseModel> RenewAndInvoke(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft = 2)
		{
			// Don't get stuck in an authorization loop.
			System.Diagnostics.Debug.WriteLine("RenewAndInvoke - Attempts Left {0}", attemptsLeft);
			if (attemptsLeft < 1) throw new ServiceErrorException("Servidor Indisponível.");

			HttpResponseModel responseModel = null;

			string messageRequest = JsonConvert.SerializeObject(new LoginIn()
			{
				EMAIL = SessionData.PharmacyUser.Username,
			});
			 
			// Call webservice to renew the token.
			responseModel = await Invoke(MagentoWS.WS_MG_LOGIN, HttpMethod.Post, messageRequest, authorization, attemptsLeft - 1);

			// Validate response message
			if (responseModel != null && !string.IsNullOrEmpty (responseModel.Message)) {
				// Deserialize response
				LoginOut responseOut = JsonConvert.DeserializeObject<LoginOut> (responseModel.Message);
				if (responseOut == null || responseOut.TokenId == null)
					throw new ServiceErrorException ("Servidor Indisponível.");

				// Update the Store Token
				authorization.StoreToken = responseOut.TokenId;

				// Updates the FarmID, if different
				if (!string.IsNullOrEmpty(responseOut.FarmId) && !string.Equals(SessionData.StorePharmacyId, responseOut.FarmId))
				{
					SessionData.UpdatePharmacy(responseOut.FarmId);
				}
			}

			// Invoke the webservice
			return await Invoke(url, method, content, authorization, attemptsLeft - 1);
		}

		#endregion

		#region Builders

		/// <summary>
		/// Initialize the authorization header.
		/// </summary>
		protected override void InitializeAuthorizationHeader(HttpRequestMessage requestMessage, AnfAuthorizationBundle authorization)
		{
			if (authorization == null)
				return;

			var token = authorization.StoreToken;
			var cookie = authorization.StoreCookie;

			if (!string.IsNullOrEmpty(token) && !requestMessage.RequestUri.AbsolutePath.Contains("/Login")) 
			{
				requestMessage.Headers.Add("TOKENID", token);
				if (!string.IsNullOrEmpty(cookie)) requestMessage.Headers.Add("Cookie", cookie);
			} 
			else 
			{
				requestMessage.Headers.Add("AuthorizationBase", authorization.AccessToken);
				requestMessage.Headers.Add("AuthorizationKey", authorization.RefreshToken);
			}
		}

		#endregion

		#region Handlers

		/// <summary>
		/// Handle the response message.
		/// </summary>
		/// <param name="responseMessage"></param>
		/// <param name="authorization"></param>
		protected override void HandleHttpResponse(HttpResponseMessage responseMessage, AnfAuthorizationBundle authorization)
		{
			if (responseMessage == null || authorization == null) return;

			// XXX: We are catching the "Set-Cookie" via Header because the CookieContainer is not storing it properly.
			IEnumerable<string> headers = new List<string>();
			if (!responseMessage.Headers.TryGetValues("Set-Cookie", out headers) || headers == null) return;
			
			List<string> headersList = new List<string>(headers);
			if (headersList.Count == 0) return; 
			
			// XXX: Because WP8 vs Android & iOS have different behaviors when returning a list of Set-Cookie headers, we will group them all into a single string and parse it as that.
			// Explanation: In WP8, we will receive multiple Set-Cookies as a single header string. In android & iOS, we will recieve multiple Set-Cookies as separate headers.
			string cookieHeader = string.Empty;
			foreach (var header in headersList) {
				cookieHeader += header;
			}

			if (string.IsNullOrEmpty(cookieHeader)) return;

			// XXX: Use regex to find the last one, because all the others are garbage.
			string cookie = string.Empty;
			MatchCollection myMatchCollection = Regex.Matches(cookieHeader, "frontend=(?<val>[^;]+)"); // "(?<name>[^=]+)=(?<val>[^;]+)[^,]+,?");
			foreach (Match myMatch in myMatchCollection) {
				cookie = "frontend=" + myMatch.Groups["val"].ToString();
            }
					
			if (string.IsNullOrEmpty(cookie)) return;

			// Save cookie! Magento rocks!
			authorization.StoreCookie = cookie;
		}

		protected override async Task<HttpResponseModel> HandleForbiddenResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			if (authorization != null)
			{
				return await RenewAndInvoke(url, method, content, authorization, attemptsLeft);
			}
			else
			{
				throw new ExpiredTokenException();
			}
		}

		protected override async Task<HttpResponseModel> HandleErrorResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft, string responseMessage)
		{			
			MagentoOut errorObj = null;
			try
			{
				// Attempt a deserialization into a default Magento response.
				errorObj = JsonConvert.DeserializeObject<MagentoOut>(responseMessage);
			} catch { }

			// The error was not formatted as expected, so throw the entire message up.
			if (errorObj == null) throw new InvalidRequestException(AppResources.UnavailableServicesGenericError);

			if (errorObj.code == Settings.MAGENT_EXPIRED_SESSION_ERROR_CODE)
			{
				// Session expired!
				// Renew the Token/Cookie and attempt to re-invoke the same WS.
				return await RenewAndInvoke(url, method, content, authorization, attemptsLeft);
			}
			else if (errorObj.code == Settings.MAGENT_EXPIRED_SESSION_FARMID_CODE && !string.IsNullOrEmpty(errorObj.FarmId))
			{
				// FarmID was changed remotely!
				throw new PharmacyChangedException(errorObj.FarmId);
			}
			else if (errorObj.code == Settings.MAGENT_EXPIREDANF_REFRESHTOKEN)
			{
				SessionData.ForceEmergencyLogout();
				return null;
			}

			// Generic error - throw the message up
			throw new InvalidRequestException(errorObj.msg);
		}

		protected override Task<HttpResponseModel> HandleUnauthorizedResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			return base.HandleUnauthorizedResponse(url, method, content, authorization, attemptsLeft);
		}

		#endregion

	}
}

