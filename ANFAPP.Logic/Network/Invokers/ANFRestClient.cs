using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Network.Invokers
{
	public class AnfRestClient : RestInvoker
	{
		
		#region Singleton

		private static AnfRestClient _instance;
		public static AnfRestClient Instance
		{
			get 
			{ 
				if (_instance == null) _instance = new AnfRestClient(); 
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

			HttpResponseModel responseModel = null;
			// Build refresh message.
			var messageRequest = JsonConvert.SerializeObject(new RefreshIn()
			{
				ApplicationId = Settings.APPLICATION_ID,
				RefreshToken = authorization.RefreshToken
			});

			// Call webservice to renew the token.
			responseModel = await Invoke(Settings.WS_AUTHORIZATION_REFRESH, HttpMethod.Post, messageRequest, null, attemptsLeft - 1);

			// Validate response message
			if (responseModel != null && !string.IsNullOrEmpty(responseModel.Message))
			{
				// Deserialize response
				AuthorizationOut responseOut = JsonConvert.DeserializeObject<AuthorizationOut>(responseModel.Message);
				if (responseOut == null || responseOut.Authorization == null) throw new ServiceErrorException("Servidor Indisponível.");

				authorization.TTL = DateTime.Now.AddMinutes(responseOut.Authorization.ExpiresIn);
				authorization.AccessToken = responseOut.Authorization.AccessToken;
				authorization.RefreshToken = responseOut.Authorization.RefreshToken;
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
			if (authorization == null) return;
			
			// Check if token expired
			if (authorization.TTL <= DateTime.Now)
			{
				// Renew token
				throw new ExpiredTokenException();
			}

			// Add the access token7
			if (SessionData.AuthANFavorites)
			{
				requestMessage.Headers.Add("Authorization", "BearerAnf " + authorization.GetAuthorizationString());
				SessionData.AuthANFavorites = false;

			}
			else 
			{ 
				requestMessage.Headers.Add("Authorization", "Bearer " + authorization.GetAuthorizationString());
			}
		}

		#endregion

		#region Handlers

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
		/// Handle an error response.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		/// <param name="responseString"></param>
		/// <returns></returns>
		protected override Task<HttpResponseModel> HandleErrorResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft, string responseMessage)
		{
			ServiceError serviceError = null;
			try 
			{ 
				serviceError = JsonConvert.DeserializeObject<ServiceError>(responseMessage);
			} catch { /* Handle parse error */ throw new InvalidRequestException(AppResources.UnavailableServicesGenericError); }

			if (serviceError != null && serviceError.Error != null && serviceError.Error.Code == 409)
			{
				// Catastrophic error!! The refresh token was lost somewhere and a logout will be forced.
				SessionData.ForceEmergencyLogout();
				return null;
			}

			throw new InvalidRequestException(
				serviceError != null && serviceError.Error != null && !string.IsNullOrEmpty(serviceError.Error.Reason) ?
				serviceError.Error.Reason :
				AppResources.UnavailableServicesGenericError);
		}

		/// <summary>
		/// Handle an unauthorized response.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		/// <returns></returns>
		protected override async Task<HttpResponseModel> HandleUnauthorizedResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
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

		#endregion

	}
}
