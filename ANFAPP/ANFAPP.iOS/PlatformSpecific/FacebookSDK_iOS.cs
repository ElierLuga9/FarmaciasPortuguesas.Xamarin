using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Utils;
using Facebook.LoginKit;
using ANFAPP.Logic;
using Xamarin.Forms;
using Facebook.CoreKit;

[assembly: Xamarin.Forms.Dependency (typeof(ANFAPP.iOS.PlatformSpecific.FacebookSDK_iOS))]
namespace ANFAPP.iOS.PlatformSpecific
{
	public class FacebookSDK_iOS : IFacebookSDK
	{

		#region Facebook SDK

		public void InitService(object context)
		{
			// Not used by iOS
			throw new NotImplementedException();
		}

		public void SendCallbackResult(int request, int result, object data)
		{
			// Not used by iOS
			throw new NotImplementedException();
		}

		public void Login()
		{
			var loginManager = new LoginManager();
			loginManager.LogOut();
			loginManager.LogInWithReadPermissions(ANFAPP.Logic.Settings.FACEBOOK_PERMISSIONS, (result, error) => 
			{
				if (error != null)
				{
					// Error
					MessagingCenter.Send<string>(error.Description, ANFAPP.Logic.Settings.MS_FACEBOOK_LOGIN_ERROR);
				}
				else if (result.IsCancelled)
				{
					// Canceled
					MessagingCenter.Send<Object>(string.Empty, ANFAPP.Logic.Settings.MS_FACEBOOK_LOGIN_CANCEL);
				}
				else
				{
					// Success
					if (!result.GrantedPermissions.Contains("email"))
					{
						// Validate if the email permission was granted
						MessagingCenter.Send<string>("Permiss?es Insuficientes", ANFAPP.Logic.Settings.MS_FACEBOOK_LOGIN_ERROR);
					}
					else
					{
						MessagingCenter.Send<string>(string.Empty, ANFAPP.Logic.Settings.MS_FACEBOOK_LOGIN_SUCCESS);
					}
				}
			});
		}

		public void Logout()
		{
			var loginManager = new LoginManager();
			loginManager.LogOut();
		}

		public bool IsLoggedOnFacebook()
		{
			return AccessToken.CurrentAccessToken != null;
		}

		public void GetUserEmail()
		{
			if (AccessToken.CurrentAccessToken == null) return;
			var request = new GraphRequest("me", null);
			request.Parameters.Add(NSObject.FromObject("fields"), NSObject.FromObject("email"));
			string fbID = string.Empty;

			request.Start((connection, result, error) => 
			{
				string id = string.Empty;
				string email = string.Empty;
				if (error == null && result is NSDictionary)
				{
					var resultDict = result as NSDictionary;
					id = resultDict.ContainsKey(NSObject.FromObject("id")) ? resultDict[NSObject.FromObject("id")].ToString() : string.Empty;
					email = resultDict.ContainsKey(NSObject.FromObject("email")) ? resultDict[NSObject.FromObject("email")].ToString() : string.Empty;
				}

				MessagingCenter.Send<FacebookObject>(new FacebookObject(email, id), ANFAPP.Logic.Settings.MS_FACEBOOK_GET_EMAIL);
			});
		}

		public string GetUserToken()
		{
			return AccessToken.CurrentAccessToken.TokenString;
		}

		#endregion

	}
}