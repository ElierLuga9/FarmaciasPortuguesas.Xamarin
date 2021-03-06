using System;
using System.Linq;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Cryptography;
using Plugin.Connectivity;

namespace ANFAPP.Logic
{
	public static class ParseRestClient
	{
		/*public async static Task RegisterDeviceToken(string deviceToken)
		{
		    // Check if there is an active connection
            if (!CrossConnectivity.Current.IsConnected) return;

			HttpClient client = new HttpClient ();
			client.BaseAddress = new Uri("https://api.parse.com");

			var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.parse.com/1/installations/"));
			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Add("X-Parse-Application-Id", Settings.PARSE_APPLICATION_ID);
			request.Headers.Add("X-Parse-REST-API-Key", Settings.PARSE_REST_API_KEY);

			string strJSONContent;
            string pushType = Device.OS == TargetPlatform.Android ? ",\"pushType\":\"gcm\"" : string.Empty;
            string gcmId = Device.OS == TargetPlatform.Android ? ",\"GCMSenderId\":\"" + Settings.ANDROID_GCM_SENDER_ID +"\"" : string.Empty;

			string device = string.Empty;
			switch (Device.OS)
			{
				case TargetPlatform.Android: device = "android"; break;
				case TargetPlatform.iOS: device = "ios"; break;
				case TargetPlatform.WinPhone: device = "wp"; break;
			}

			if (!string.IsNullOrEmpty(SessionData.ParseInstallationId)) 
            {
				strJSONContent =
                    "{\"owner\":\"" + SessionData.ParseInstallationId + "\"" +
                    ",\"deviceType\":\"" + device + "\"" +
                    ",\"deviceToken\":\"" + deviceToken + "\"" +
                    gcmId +
                    pushType +
                    ",\"channels\":[\"global\"]}";
			} 
            else 
            {
				strJSONContent = 
                    "{\"deviceType\":\"" + device + "\"" +
                    ",\"deviceToken\":\"" + deviceToken + "\"" +
                    gcmId +
                    pushType +
                    ",\"channels\":[\"global\"]}";
			}
			request.Content = new StringContent(strJSONContent);

			try { 
			var response = await client.SendAsync(request); 

			// Parse response
			string responseString = string.Empty;
			if (response.Content != null) {
				responseString = await response.Content.ReadAsStringAsync();

				var dict = JsonConvert.DeserializeObject<Dictionary<string,object>>(responseString);

				object objectId;
				if (dict.TryGetValue ("objectId", out objectId)) {
					SessionData.ParseInstallationId = objectId as string;
					System.Diagnostics.Debug.WriteLine(string.Format("Parse installation id: {0}", SessionData.ParseInstallationId));
				}
			}
			}
			catch (Exception ex)
			{
				// Service Exception
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}
*/
		/*
		public async static Task HandleOpenPush(object payload)
		{
			// Sanity check.
			if (string.IsNullOrEmpty (SessionData.ParseInstallationId) || payload == null) return;

			// Check if there is an active connection
			if (!CrossConnectivity.Current.IsConnected) return;

			// We need to generate an hash from the push notification payload. See
			// https://github.com/ParsePlatform/Parse-SDK-iOS-OSX/blob/61f68e9333206f0eebf15d5adab076a0ae7c4b71/Parse/Internal/Analytics/Controller/PFAnalyticsController.m
			var pushHash = MD5DigestFromPushPayload(payload);
			System.Diagnostics.Debug.WriteLine ("pushHash {0}", pushHash);

			HttpClient client = new HttpClient ();
			client.BaseAddress = new Uri("https://api.parse.com");

			// See:
			// https://github.com/ParsePlatform/Parse-SDK-iOS-OSX/blob/61f68e9333206f0eebf15d5adab076a0ae7c4b71/Parse/Internal/Commands/PFRESTAnalyticsCommand.m
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.parse.com/1/events/AppOpened"));
			// request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Add("X-Parse-Application-Id", Settings.PARSE_APPLICATION_ID);
			request.Headers.Add("X-Parse-REST-API-Key", Settings.PARSE_REST_API_KEY);

//			var at = string.Format ("{{\"__type\":\"Date\", \"iso\":\"{0}\"}}", DateTime.UtcNow.ToString ("o"));
//			var strJSONContent =
//				"{\"at\":\"" + at +
//				", \"push_hash\":\"" + pushHash + "\"}";
			var strJSONContent = "{\"push_hash\":\"" + pushHash + "\"}";
			request.Content = new StringContent(strJSONContent, Encoding.UTF8, "application/json");

			System.Diagnostics.Debug.WriteLine (strJSONContent);
			System.Diagnostics.Debug.WriteLine (request);
			try { 
				var response = await client.SendAsync(request); 
				System.Diagnostics.Debug.WriteLine(response);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		private static string MD5DigestFromPushPayload(object payload)
		{
			string result = "";
			if (payload == null)
				return result;

			System.Diagnostics.Debug.WriteLine (string.Format ("{0} {1}", payload, payload.GetType ()));
			var dict = payload as IDictionary;
			if (dict != null) {
				// Implement the hashing algorithm in 
				// https://github.com/ParsePlatform/Parse-SDK-iOS-OSX/blob/61f68e9333206f0eebf15d5adab076a0ae7c4b71/Parse/Internal/Analytics/Utilities/PFAnalyticsUtilities.m
				var components = new List<string>();

				List<object> keys = ((ICollection<object>)dict.Keys).ToList();
				keys.Sort ();
				foreach (object key in keys) {
					var key_s = key.ToString ();
					components.Add (key_s);

					var val = dict [key];
					if (val is IEnumerable) {
						IEnumerable<object> array = val as IEnumerable<object>;
						var tmp = new List<string> ();
						foreach (object obj in array) {
							tmp.Add (obj.ToString ());
						}

						val = string.Join ("", tmp.ToArray());
					}

					components.Add (val.ToString());
				}

				payload = string.Join ("", components.ToArray ());
			}

			// Can't use "System.Security.Cryptography" from the PCL.
			try {
				System.Diagnostics.Debug.WriteLine(string.Format("Hashing {0}", payload.ToString()));
				result = MD5.GetMd5String (payload.ToString());
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}

			return result;
		}*/
	}
}

