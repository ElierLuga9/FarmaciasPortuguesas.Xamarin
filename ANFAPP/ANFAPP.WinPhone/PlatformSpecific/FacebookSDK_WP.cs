using ANFAPP.Logic;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Utils;
using Facebook;
using Facebook.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.WinPhone.PlatformSpecific.FacebookSDK_WP))]
namespace ANFAPP.WinPhone.PlatformSpecific
{

	public class FacebookSDK_WP : IFacebookSDK
	{

		#region Facebook SDK

		public void InitService(object contextActivity) { }

		public void SendCallbackResult(int request, int result, object data) { }

		public void Login()
		{
			Session.ActiveSession.LoginWithBehavior("email,public_profile", FacebookLoginBehavior.LoginBehaviorApplicationOnly);
		}

		public void Logout()
		{
			Session.ActiveSession.Logout();
		}

		public async void GetUserEmail()
		{
			string email = string.Empty, id = string.Empty;
			var accessToken = GetUserToken();
			if (string.IsNullOrEmpty(accessToken)) return;

			try
			{
				FacebookClient client = new FacebookClient(accessToken);
				dynamic result = await client.GetTaskAsync("me");
				email = (string)result["email"];
				id = (string)result["id"];
			}
			catch {  }

			MessagingCenter.Send<FacebookObject>(new FacebookObject(email, id), Settings.MS_FACEBOOK_GET_EMAIL);
		}

		public string GetUserToken()
		{
			return Session.ActiveSession != null && Session.ActiveSession.CurrentAccessTokenData != null ? 
				Session.ActiveSession.CurrentAccessTokenData.AccessToken : null;
		}

		public bool IsLoggedOnFacebook()
		{
			return Session.ActiveSession.CurrentAccessTokenData != null && !string.IsNullOrEmpty(Session.ActiveSession.CurrentAccessTokenData.AccessToken);
		}

		#endregion

		#region Delegate Handlers

		

		#endregion

	}
}