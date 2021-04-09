using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ANFAPP.Logic.Utils;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using ANFAPP.Logic;
using Org.Json;
using Xamarin.Forms;
using ANFAPP.Logic.Models;

[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.Droid.FacebookSDK_Droid))]
namespace ANFAPP.Droid
{

	public class FacebookSDK_Droid : Java.Lang.Object, IFacebookSDK, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
	{
		
		#region Properties

		protected static Activity ParentActivity;
		protected static ICallbackManager Callback;

		#endregion

		#region Facebook SDK

		public void InitService(object contextActivity)
		{
			if (contextActivity == null || !(contextActivity is Activity)) return;
			ParentActivity = contextActivity as Activity;

			FacebookSdk.SdkInitialize(ParentActivity.ApplicationContext);
			Callback = CallbackManagerFactory.Create();

			LoginManager.Instance.RegisterCallback(Callback, this);
		}

		public void SendCallbackResult(int request, int result, object data)
		{
			if (Callback == null || !(data is Intent)) return;
			Callback.OnActivityResult(request, result, data as Intent);
		}

		public void Login()
		{
			if (ParentActivity == null) return;

			// Attempt to login.
			LoginManager.Instance.LogInWithReadPermissions(ParentActivity, Settings.FACEBOOK_PERMISSIONS);
		}

		public void Logout()
		{
			// Attempt to logout.
			LoginManager.Instance.LogOut();
		}

		public void GetUserEmail()
		{
			
			if (AccessToken.CurrentAccessToken == null) return;
			// Request user email
			GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
			request.Parameters = new Bundle();
			request.Parameters.PutString("fields", "email");
			request.ExecuteAsync();
		}

		public string GetUserToken()
		{
			return (AccessToken.CurrentAccessToken != null) ? AccessToken.CurrentAccessToken.Token : null;
		}

		public bool IsLoggedOnFacebook()
		{
			return AccessToken.CurrentAccessToken != null;
		}

		#endregion

		#region Facebook Login Callback

		public void OnCancel()
		{
			MessagingCenter.Send<Object>(string.Empty, Settings.MS_FACEBOOK_LOGIN_CANCEL);
		}

		public void OnError(FacebookException error)
		{
			MessagingCenter.Send<string>(error.Message, Settings.MS_FACEBOOK_LOGIN_ERROR);
		}

		public void OnSuccess(Java.Lang.Object result)
		{
			if (!AccessToken.CurrentAccessToken.Permissions.Contains("email"))
			{
				// Validate if the email permission was granted
				MessagingCenter.Send<string>("Permissões Insuficientes", Settings.MS_FACEBOOK_LOGIN_ERROR);
			}
			else
			{
				MessagingCenter.Send<string>(string.Empty, Settings.MS_FACEBOOK_LOGIN_SUCCESS);
			}
		}

		#endregion

		public void OnCompleted(JSONObject obj, GraphResponse response)
		{
			string email = obj != null ? obj.OptString("email", string.Empty) : string.Empty;


			MessagingCenter.Send<FacebookObject>(
				new FacebookObject(email, AccessToken.CurrentAccessToken.UserId), 
				Settings.MS_FACEBOOK_GET_EMAIL);
		}
	}
}