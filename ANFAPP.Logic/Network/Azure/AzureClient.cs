using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Network.Services.Abstract;
using Plugin.Connectivity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ANFAPP.Logic.Network.Azure
{
	public class AzureClient
	{
		public async static Task RegisterDeviceToken(string azureToken) {
			if (!SessionData.IsLogged || SessionData.NotificationService == Settings.AZURE_NOTIFICATION_SERVICE) {
				SessionData.ParseInstallationId = azureToken;
				SessionData.NotificationService = Settings.AZURE_NOTIFICATION_SERVICE;
				return;
			}

			try 
			{
				var response = await I9WS.UpdateDeviceToken(SessionData.PharmacyUser.Username, azureToken, Settings.PARSE_APPLICATION_ID);
				if (response.TokenUpdated) {
					SessionData.ParseInstallationId = azureToken;
					SessionData.NotificationService = Settings.AZURE_NOTIFICATION_SERVICE;
				}
			} catch (Exception e) {
				
			}
		}


        public async static Task RegisterWPhoneDeviceToken(string token)
        {
            // Check if there is an active connection
            if (!CrossConnectivity.Current.IsConnected) return;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Settings.AZURE_CONNECTION_ENDPOINT);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(Settings.AZURE_CONNECTION_ENDPOINT + "/Registrations?api-version=2015-01"));
            request.Headers.Add("Content-Type", "application/atom+xml;type=entry;charset=utf-8");
            request.Headers.Add("x-ms-version", "2015-01");
            request.Headers.Add("Authorization", GetSaSToken(Settings.AZURE_CONNECTION_ENDPOINT, 60));

            string strContent = "<? xml version =\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><content type=\"application/xml\"><WindowsRegistrationDescription xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\"><ChannelUri>{0}</ChannelUri></WindowsRegistrationDescription></content></entry>";
            strContent = string.Format(strContent, token);
            request.Content = new StringContent(strContent);

            try
            {
                var response = await client.SendAsync(request);

                // Parse response
                string responseString = string.Empty;
                if (response.Content != null)
                {
                    responseString = await response.Content.ReadAsStringAsync();

                    await RegisterDeviceToken(token);
                }
            }
            catch (Exception ex)
            {
                // Service Exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public static string GetSaSToken(string connectionString, int minUntilExpire)
        {
            string Endpoint = string.Empty;
            string SasKeyName = string.Empty;
            string SasKeyValue = string.Empty;

            char[] separator = { ';' };
            string[] parts = connectionString.Split(separator);
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("Endpoint"))
                    Endpoint = "https" + parts[i].Substring(11);
                if (parts[i].StartsWith("SharedAccessKeyName"))
                    SasKeyName = parts[i].Substring(20);
                if (parts[i].StartsWith("SharedAccessKey"))
                    SasKeyValue = parts[i].Substring(16);
            }

            string targetUri = Uri.EscapeDataString(Endpoint.ToLower()).ToLower();

            // Add an expiration in seconds to it.
            long expiresOnDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            expiresOnDate += minUntilExpire * 60 * 1000;
            long expires_seconds = expiresOnDate / 1000;
            String toSign = targetUri + "\n" + expires_seconds;

            // Generate a HMAC-SHA256 hash or the uri and expiration using your secret key.
            //MacAlgorithmProvider macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            //BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;
            //var messageBuffer = CryptographicBuffer.ConvertStringToBinary(toSign, encoding);
            //IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(SasKeyValue, encoding);
            //CryptographicKey hmacKey = macAlgorithmProvider.CreateKey(keyBuffer);
            //IBuffer signedMessage = CryptographicEngine.Sign(hmacKey, messageBuffer);

            //string signature = Uri.EscapeDataString(CryptographicBuffer.EncodeToBase64String(signedMessage));
            string signature = SasKeyValue;

            return "SharedAccessSignature sr=" + targetUri + "&sig=" + signature + "&se=" + expires_seconds + "&skn=" + SasKeyName;
        }
    }
}
