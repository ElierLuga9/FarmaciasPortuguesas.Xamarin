using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Network.Services
{
	public class GameCenterWS : I9WS
	{

		/// <summary>
		/// Get the Game Center dashboard
		/// </summary>
		/// <returns></returns>
		public static async Task<AchievementCenterOut> GetDashboard()
		{
			return await PostEvent(Settings.GAMECENTER_EVENT_DASHBOARD);
		}

		/// <summary>
		/// Post an event to the webservice
		/// </summary>
		/// <param name="eventType"></param>
		/// <returns></returns>
		public static async Task<AchievementCenterOut> PostEvent(string eventType, DateTime? date = null)
		{
			if (!Settings.IS_GAMIFICATION_ACTIVE) return null;

			var inDate = date.HasValue ? date.Value : DateTime.UtcNow;
			var request = JsonConvert.SerializeObject(new GameCenterIn() {
				CardNumber = SessionData.HasPharmacyCard ? SessionData.PharmacyUser.CardNumber : null,
				Date = inDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
				EventType = eventType
			});

			var response = await Client.PostJson(Settings.WS_GAMIFICATION_GAMECENTER, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<AchievementCenterOut>(response.Message);
		}

		/// <summary>
		/// Posts an event async
		/// </summary>
		/// <param name="eventType"></param>
		public static async void PostEventAsync(string eventType, DateTime? date = null)
		{
			try
			{
				await PostEvent(eventType, date);
			} 
			catch
			{ 
				// Do nothing on failure
			}
		}
	}
}
