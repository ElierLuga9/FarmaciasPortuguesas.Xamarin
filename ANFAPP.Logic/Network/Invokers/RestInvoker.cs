
using ANFAPP.Logic.Exceptions;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Invokers;
using ANFAPP.Logic.Utils;
using Plugin.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Platform.Device;
using ModernHttpClient;
using Plugin.Connectivity;

namespace ANFAPP.Logic
{

    #region Response Model

    public class HttpResponseModel 
    {

        /// <summary>
        /// Response Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Access Tokens Bundle, if renewed
        /// </summary>
        public IAuthorizationBundle Authorization { get; set; }

    }

    #endregion

	/// <summary>
	/// 
	/// </summary>
	public abstract class RestInvoker
	{
		
		#region Constants

		/// <summary>
		/// Timeout in seconds for a REST call (1:30 mins)
		/// </summary>
		protected static readonly int REST_INVOKER_TIMEOUT = 120;

        /// <summary>
        /// JSON Content Type definition.
        /// </summary>
        protected static readonly string JSON_CONTENT_TYPE = "application/json";

		#endregion

		#region Http Methods

        /// <summary>
        /// Authenticated GET request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
		public async Task<HttpResponseModel> Get(string url, AnfAuthorizationBundle authorization = null)
		{
			return await Invoke(url, HttpMethod.Get, null, authorization);
		}

        /// <summary>
        /// Authenticated PUT request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
		public async Task<HttpResponseModel> PutJson(string url, string content, AnfAuthorizationBundle authorization = null) 
		{
            return await Invoke(url, HttpMethod.Put, content, authorization);
		}

        /// <summary>
        /// Authenticated POST request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
		public async Task<HttpResponseModel> PostJson(string url, string content, AnfAuthorizationBundle authorization = null)
		{
			
            return await Invoke(url, HttpMethod.Post, content, authorization);
		}

        /// <summary>
        /// Authenticated PATCH request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
		public async Task<HttpResponseModel> PatchJson(string url, string content, AnfAuthorizationBundle authorization = null)
        {
            return await Invoke(url, new HttpMethod("PATCH"), content, authorization);
        }

		#endregion

		#region Invokers

        /// <summary>
        /// Invoker Method. Uses the parameters to build an HTTP request and process the results. </br>
        /// For authenticated requests, renews the access token if needed.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
		public async Task<HttpResponseModel> Invoke(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft = 2) 
		{
            System.Diagnostics.Debug.WriteLine("Invoke: Attempts Left {0}", attemptsLeft);
            if (attemptsLeft < 1) throw new NetworkingException();

            // Check if there is an active connection
            if (!CrossConnectivity.Current.IsConnected)
            {
                // No Connection
                throw new NetworkingException();
            }

            // Build the Request Message
            HttpRequestMessage requestMessage = BuildHttpRequestMessage(url, method, content);
            // Initialize Authentication
			bool isExpired = false;
			try 
			{ 
				InitializeAuthorizationHeader(requestMessage, authorization);
			}
			catch (ExpiredTokenException) { isExpired = true; }
			if (isExpired) return await HandleExpiredTokenException(url, method, content, authorization, attemptsLeft);

            // Call service and parse response
            HttpResponseMessage responseMessage;
            string responseString = string.Empty;

			//NOTE: Flag set to false so the HttpClient will use the "Cookie" Header instead.
//			var HttpClient = new HttpClient(new NativeMessageHandler() { UseCookies = false }) 
			var HttpClient = new HttpClient(new HttpClientHandler() { UseCookies = false }) 
			{ 
				Timeout = new TimeSpan(0, 0, REST_INVOKER_TIMEOUT),
			};

            try {
				
                responseMessage = await HttpClient.SendAsync(requestMessage);
                if (responseMessage.Content != null) {
                    responseString = await responseMessage.Content.ReadAsStringAsync();
                }

				// Handle the HTTP response
				HandleHttpResponse(responseMessage, authorization); 

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

			// TODO: Remove after debug is complete
			// Temporary logs for I9WS errors
			//string log = "# REQUEST # \n "
			//	+ requestMessage + " \n " + content + " \n#\n#\n "
			//	+ "# RESPONSE # \n "
			//	+ responseMessage + "\n" + responseString;


            // Build Response Model
            switch(responseMessage.StatusCode) 
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.NoContent:
					// OK
					return HandleOKResponse(authorization, responseString);
                case HttpStatusCode.Unauthorized:
					// Invalid Token
					return await HandleUnauthorizedResponse(url, method, content, authorization, attemptsLeft);
				case HttpStatusCode.Forbidden:
					return await HandleForbiddenResponse(url, method, content, authorization, attemptsLeft);
                default:
                    // Error
					System.Diagnostics.Debug.WriteLine(responseMessage);
					return await HandleErrorResponse(url, method, content, authorization, attemptsLeft, responseString);
            }
		}

        #endregion

		#region Builders

		/// <summary>
		/// Builds the Http Request Message
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		protected virtual HttpRequestMessage BuildHttpRequestMessage(string url, HttpMethod method, string content)
		{
			// Build the request message
			HttpRequestMessage requestMessage = new HttpRequestMessage();
			requestMessage.RequestUri = new Uri(url);
			requestMessage.Method = method;
			requestMessage.Headers.Add("Accept-Language", "pt-PT");
			// Set content
			if (!string.IsNullOrEmpty(content))
			{
				// Add the JSON content
				requestMessage.Content = new StringContent(content, Encoding.UTF8, JSON_CONTENT_TYPE);
			}

			return requestMessage;
		}

		/// <summary>
		/// Initialize the authorization header.
		/// </summary>
		protected virtual void InitializeAuthorizationHeader(HttpRequestMessage requestMessage, AnfAuthorizationBundle authorization)
		{
			if (authorization == null) return;

			// Add the access token


				requestMessage.Headers.Add("Authorization", "Basic " + authorization.GetAuthorizationString());


		}

		#endregion

		#region Handlers

		/// <summary>
		/// Handle the response message.
		/// </summary>
		/// <param name="responseMessage"></param>
		/// <param name="authorization"></param>
		protected virtual void HandleHttpResponse(HttpResponseMessage responseMessage, AnfAuthorizationBundle authorization) { }

		/// <summary>
		/// Handle an expired token.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		protected virtual Task<HttpResponseModel> HandleExpiredTokenException(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			throw new ExpiredTokenException();
		}

		/// <summary>
		/// Handle an OK Response
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="content"></param>
		/// <param name="authorization"></param>
		/// <param name="attemptsLeft"></param>
		/// <param name="responseString"></param>
		/// <returns></returns>
		protected virtual HttpResponseModel HandleOKResponse(AnfAuthorizationBundle authorization, string response)
		{
			return new HttpResponseModel()
			{
				Message = response,
				Authorization = authorization
			};
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
		protected virtual Task<HttpResponseModel> HandleErrorResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft, string responseMessage)
		{
			throw new InvalidRequestException(AppResources.UnavailableServicesGenericError);
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
		protected virtual Task<HttpResponseModel> HandleUnauthorizedResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			throw new ExpiredTokenException();
		}

		protected virtual Task<HttpResponseModel> HandleForbiddenResponse(string url, HttpMethod method, string content, AnfAuthorizationBundle authorization, int attemptsLeft)
		{
			throw new InvalidRequestException(content);
		}

		#endregion

    }
}

