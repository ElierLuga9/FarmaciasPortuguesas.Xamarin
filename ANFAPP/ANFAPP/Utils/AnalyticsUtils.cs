using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ANFAPP.Utils
{
	// Adapted from FrazzApps.Xamarin.GoogleAnalyticsConnector.
	public class AnalyticsUtils
	{
		private const string BaseURL = "https://ssl.google-analytics.com/collect";

		private HttpClient Client;

		private string TrackingID { get; set; }
		private string AppName { get; set; }
		private string AppVersion { get; set; }
		private string AppId { get; set; }
		private string AppInstallerId { get; set; }

		public AnalyticsUtils(
			string trackingID,
			string appName,
			string appVersion,
			string appId,
			string appInstallerId)
		{
			TrackingID = trackingID;

			AppName = appName;
			AppVersion = appVersion;
			AppId = appId;
			AppInstallerId = appInstallerId;

			Client = new HttpClient ();
		}

		private async Task Track(Uri url)
		{  
			HttpResponseMessage response = null;
			try
			{
				// TODO: se user agent!
				response = await Client.PostAsync(url, null);

				System.Diagnostics.Debug.WriteLine("Tracking result = [" + response.StatusCode + "] " + response.ReasonPhrase);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("GA Tracking Exception: " + ex.Message + "\n - URL: " + url.OriginalString);
			}
		}

		public async Task TrackPage(string userId, string pageName)
		{
			string parametersBaseString = string.Format(
				"ds=app&v={0}&tid={1}&cid={2}",
				1,
				TrackingID,
				userId);

			string parametersPageView = string.Format(
				"&t=pageview&dp=%2F{0}",
				pageName);

			Uri url = new Uri(BaseURL + "?" + parametersBaseString + parametersPageView);
			Track(url);
		}

		public async Task TrackException(string userId, Exception ex, bool isFatal)
		{
			string parametersBaseString = string.Format(
				"ds=app&v={0}&tid={1}&cid={2}",
				1,
				TrackingID,
				userId);

			string parametersPageView = string.Format(
				"&t=exception&exd={0}&exf={1}",
				ex.Message,
				(isFatal)?1:2);

			Uri url = new Uri(BaseURL + "?" + parametersBaseString + parametersPageView);
			await Track(url);
		}

		public async Task TrackScreen(string userId, string screenName)
		{
			string parametersBaseString = string.Format(
				"ds=app&v={0}&tid={1}&cid={2}",
				1,
				TrackingID,
				userId);

			string parametersPageView = string.Format(
				"&t=screenview&an={0}&av={1}&aid={2}&cd={3}",
				AppName, // App name.
				AppVersion, // App version.
				AppId,// App Id.
				screenName);

			// &aiid=AppInstallerId		// Optional: App Installer Id.

			Uri url = new Uri(BaseURL + "?" + parametersBaseString + parametersPageView);
			await Track(url);
		}

		public async Task TrackEvent(string userId, string category, string action)
		{
			string parametersBaseString = string.Format(
				"ds=app&v={0}&tid={1}&cid={2}",
				1,
				TrackingID,
				userId);

			string parametersPageView = string.Format(
				"&t=event&ec={0}&ea={1}",
				category, // Event Category. Required.
				action);

			//&t=event        // Event hit type
			//&ec=video       // Event Category. Required.
			//&ea=play        // Event Action. Required.
			//&el=holiday     // Event label.
			//&ev=300         // Event value.

			Uri url = new Uri(BaseURL + "?" + parametersBaseString + parametersPageView);
			await Track(url);
		}
	}
}
