using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.Errors;
using ANFAPP.Logic.Models.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Logic.Network.Invokers
{
	public class I9RestClient : RestInvoker
	{
		
		#region Singleton

		private static I9RestClient _instance;
		public static I9RestClient Instance
		{
			get
			{
				if (_instance == null) _instance = new I9RestClient();
				return _instance;
			}
		}

		#endregion

		#region Invokers

		/// <summary>
		/// Renew the AccessToken and Invoke the referenced webservice with the referenced parameters.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <returns></returns>
		public async Task<HttpResponseModel> RenewAndInvoke(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft = 2)
		{
			// Don't get stuck in an authorization loop.
			System.Diagnostics.Debug.WriteLine("RenewAndInvoke - Attempts Left {0}", attemptsLeft);
			if (attemptsLeft < 1) throw new ServiceErrorException("Servidor Indisponível.");

			// Refresh ANF token.
			HttpResponseModel responseModel = null;
			// Build refresh message.
			var messageRequest = JsonConvert.SerializeObject(new RefreshIn()
				{
					ApplicationId = Settings.APPLICATION_ID,
					RefreshToken = authorization.RefreshToken
				});

			// Call webservice to renew the token.
			//responseModel = await Invoke(Settings.WS_AUTHORIZATION_REFRESH, HttpMethod.Post, messageRequest, null, attemptsLeft - 1);

			// We need to skip the Invoke header initialization and the response handlers.
			// Build the Request Message
			HttpRequestMessage requestMessage = BuildHttpRequestMessage(Settings.WS_AUTHORIZATION_REFRESH, HttpMethod.Post, messageRequest);

			// Call service and parse response
			HttpResponseMessage responseMessage;
			string responseString = string.Empty;

			//NOTE: Flag set to false so the HttpClient will use the "Cookie" Header instead.
			var HttpClient = new HttpClient(new HttpClientHandler() { UseCookies = false }) 
			{ 
				Timeout = new TimeSpan(0, 0, REST_INVOKER_TIMEOUT) 
			};

			try {
				responseMessage = await HttpClient.SendAsync(requestMessage);
				if (responseMessage.Content != null) {
					responseString = await responseMessage.Content.ReadAsStringAsync();
				}

				System.Diagnostics.Debug.WriteLine(requestMessage + "\n" + content);
				System.Diagnostics.Debug.WriteLine(responseMessage + "\n" + responseString);
			} catch (TaskCanceledException e) {
				System.Diagnostics.Debug.WriteLine(requestMessage + "\n" + content);
				System.Diagnostics.Debug.WriteLine(e.GetBaseException());

				if (e.CancellationToken.IsCancellationRequested) {
					throw e;
				} else {
					// Handle timeout.
					throw new NetworkingException ();    
				}
			}

			//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// Validate response message
			if (responseMessage != null && !string.IsNullOrEmpty(responseString))
			{
				try {
				// Deserialize response
				AuthorizationOut responseOut = JsonConvert.DeserializeObject<AuthorizationOut>(responseString);
				if (responseOut == null || responseOut.Authorization == null) throw new ServiceErrorException("Servidor Indisponível.");

				authorization.TTL = DateTime.Now.AddMinutes(responseOut.Authorization.ExpiresIn);
				authorization.AccessToken = responseOut.Authorization.AccessToken;
				authorization.RefreshToken = responseOut.Authorization.RefreshToken;
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine (ex);
				}
			}


			/*HttpResponseModel*/ responseModel = null;
			// Build refresh message.
			/*string*/ messageRequest = JsonConvert.SerializeObject(new GetTokenIn()
			{
				UserID = SessionData.PharmacyUser.Username,
				Device = SessionData.ParseInstallationId,
				Card = SessionData.HasPharmacyCard ? SessionData.PharmacyUser.CardNumber : null
			});

			// Call webservice to renew the token.
			authorization.I9Token = null;
			responseModel = await Invoke(Settings.TOMAS_ENDPOINT + "GetTokenAzure", HttpMethod.Post, messageRequest, authorization, attemptsLeft - 1);

			// Validate response message
			if (responseModel != null && !string.IsNullOrEmpty(responseModel.Message))
			{
				// Deserialize response
				var responseOut = JsonConvert.DeserializeObject<GetTokenOut>(responseModel.Message);
				if (responseOut == null || string.IsNullOrEmpty(responseOut.Token)) throw new ServiceErrorException("Servidor Indisponível.");

				authorization.I9Token = responseOut.Token;
				SessionData.SaveAuthorization(authorization);
			}

			// Invoke the webservice
			return await Invoke(url, method, content, authorization, attemptsLeft - 1);
		}

		#endregion

		#region Handlers

		protected override void InitializeAuthorizationHeader(HttpRequestMessage requestMessage, AnfAuthorizationBundle authorization)
		{
			if (authorization == null) return;

			var authBundle = authorization as AnfAuthorizationBundle;

			if (authBundle.I9Token != null && !requestMessage.RequestUri.AbsolutePath.Contains("/GetToken"))
			{
				requestMessage.Headers.Add("TokenId", authBundle.I9Token);

				// Add the access token
				// requestMessage.Headers.Add("TokenId", "1H986Z1RKSDEFKHJCF4V11R73OQFLU");
			}
			else
			{
				requestMessage.Headers.Add("AuthorizationBase", authBundle.AccessToken);
				requestMessage.Headers.Add("AuthorizationKey", authBundle.RefreshToken);

				requestMessage.Headers.Add("AplicationId", Settings.I9WS_APPLICATION_ID);
			}
		}

		protected override HttpResponseModel HandleOKResponse(AnfAuthorizationBundle authorization, string response)
		{
			// Parse the "d" tag
			var responseObj = JsonConvert.DeserializeObject<I9BaseResponse>(response);
			if (responseObj == null || responseObj.D == null) throw new InvalidRequestException(response);

			return base.HandleOKResponse(authorization, JsonConvert.SerializeObject(responseObj.D));
		}

		protected override Task<HttpResponseModel> HandleErrorResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft, string responseMessage)
		{
			try 
			{ 
				// Parse the "d" tag
				var responseObj = JsonConvert.DeserializeObject<I9BaseResponse>(responseMessage);
				if (responseObj == null || responseObj.D == null) throw new InvalidRequestException(AppResources.UnavailableServicesGenericError);

				var responseStr = JsonConvert.SerializeObject(responseObj.D);

				// Deserialize into the base model
				var errorObj = JsonConvert.DeserializeObject<I9ServiceError>(responseStr);
				if (errorObj == null || string.IsNullOrEmpty(errorObj.ErrorMessage)) throw new InvalidRequestException(responseStr);

				// Return the error
				throw new InvalidRequestException(errorObj.ErrorMessage);
			} 
			catch (Exception) 
			{
				// Unexpected reply
				throw new ServiceErrorException(AppResources.UnavailableServicesGenericError);
			}
		}

		/// <summary>
		/// Handle an expired token.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		protected override async Task<HttpResponseModel> HandleExpiredTokenException(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			// Renew token
			return await RenewAndInvoke(url, method, content, authorization, attemptsLeft);
		}

		/// <summary>
		/// Handle an expired token.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		/// <returns></returns>
		protected override async Task<HttpResponseModel> HandleUnauthorizedResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			// Renew token
			return await RenewAndInvoke(url, method, content, authorization, attemptsLeft);
		}

		protected override async Task<HttpResponseModel> HandleForbiddenResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			// Renew token
			return await RenewAndInvoke(url, method, content, authorization, attemptsLeft);
		}

		#endregion
	}
}
