using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Network.Services.Abstract;
using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out;

namespace ANFAPP.Logic.ViewModels
{
    public class LoginViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
		public event OnLoginSuccessEventHandler OnLoginSuccess;

		public event OnFacebookEventHandler OnFacebookLoginCancel;
		public event OnFacebookEventHandler OnFacebookLoginSuccess;

		public delegate void OnLoginSuccessEventHandler(bool hasCard);
		public delegate void OnFacebookEventHandler();

        #endregion

        #region Properties

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        #endregion

        #region Loaders

		/// <summary>
		/// Start login via facebook.
		/// </summary>
		public void FacebookLogin()
		{
			// Attempt login
			DependencyService.Get<IFacebookSDK>().Login();
		//	FacebookGetEmailSuccess();


		}

		/// <summary>
		/// Login via facebook.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="accessToken"></param>
		protected async void FacebookLoginANF(string email, string userId, string accessToken)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(accessToken)) 
			{
				// Logout in case of error
				if (OnError != null) OnError(null, AppResources.GenericErrorMessage);
				DependencyService.Get<IFacebookSDK>().Logout();
				return;
			}

			try
			{
				// Call Login WebService
				var authResponse = await LoginWS.FacebookLogin(email, accessToken);

				// Sets the Facebook Profile Picture as active photo
				SessionData.UserPhotoLocation = string.Format(Settings.FACEBOOK_PROFILE_PICTURE_GRAPH, userId);
				SessionData.FacebookLogin = true;
				SessionData.FaceBookEmail = email;
				// Complete the Login Process
				await LoadUser(authResponse);

				if (OnFacebookLoginSuccess != null) OnFacebookLoginSuccess();
			}
			catch (Exception e)
			{
				// Logout in case of error
				if (OnError != null) OnError(null, e.Message);
				DependencyService.Get<IFacebookSDK>().Logout();

				SessionData.UserPhotoLocation = null;
			}
		}

        /// <summary>
        /// Validate the inputs and atempts to login.
        /// </summary>g
        public async void Login()
        {
            // Validate inputs
            if (!ValidateInputs()) return;

            try
            {
                // Call Login WebService
				var authResponse = await LoginWS.Login(Email, Password);

				SessionData.FacebookLogin = false;
                
				// Complete the login process
                var a = await LoadUser(authResponse);

            }
            catch (Exception e)
            {
                // Service error
                if (OnError != null) OnError(AppResources.LoginPageErrorTitle, e.Message);
            }
        }

		/// <summary>
		/// Validate the inputs and atempts to login.
		/// </summary>
		public async Task<bool> LoadUser(AuthorizationOut authResponse)
		{
			try
			{
				// Build auth bundle
				var username = authResponse.Authorization.UserName;
				var authBundle = new AnfAuthorizationBundle(authResponse);

				// Call Magento Login WebService
				var mgResponse = await ECommerceWS.Login(authBundle, username);
				authBundle.StoreToken = mgResponse.TokenId;

				bool hasProfile = true;
				UserProfileOut userProfile = null;
				// Call User Profile WS
				try
				{
					userProfile = await UserCardWS.GetUserProfile(username, authBundle);
					// Save login
					SessionData.SaveUser(userProfile, authBundle);
				}
				catch { hasProfile = false; }

				if (!hasProfile)
				{ 
					try
					{
						// Get the profile with no card
						userProfile = await UserCardWS.GetUserProfileNoCard(username, authBundle);

						// Save login
						SessionData.SaveUser(userProfile, authBundle);
					}
					catch
					{
						userProfile = new UserProfileOut
						{
							User = new UserProfileOut.UserOut
							{
								UserName = username,
								Email = username
							}
						};

						// http://issue.innovagency.com/view.php?id=20517
						SessionData.SaveUser(userProfile, authBundle);
					}
				}

				// Call I9 Login WebService
				var i9Response = await I9WS.Authenticate(username, SessionData.ParseInstallationId, authBundle);
				authBundle.I9Token = i9Response.Token;
				SessionData.SaveUser(userProfile, authBundle);
				//(SessionData.HasPharmacyCard);
				// Login Success
				if (OnLoginSuccess != null) OnLoginSuccess(SessionData.HasPharmacyCard);
				return true;
			}
			catch (Exception e)
			{
				// Service error
				if (OnError != null) OnError(AppResources.LoginPageErrorTitle, e.Message);
				SessionData.ClearUser();
				return false;
			}
		}

        /// <summary>
        /// Validate if any of the inputs are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                if (OnError != null) OnError(AppResources.LoginPageErrorTitle, AppResources.LoginPageErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all the view forms.
        /// </summary>
        public void ClearForm()
        {
			Email = Password = null;
        }

        #endregion

		#region Facebook Event Handlers

		void FacebookLoginSuccess(string message)
		{
			// Get the email from Facebook	
			DependencyService.Get<IFacebookSDK>().GetUserEmail();
		}

		void FacebookLoginCancel(string message)
		{
			OnFacebookLoginCancel();
		}

		void FacebookLoginError(string error)
		{
			OnError(null, error);
		}
		public void FacebookGetEmailSuccess(FacebookObject login)
		{	
			FacebookLoginANF(login.Email, login.UserId, DependencyService.Get<IFacebookSDK>().GetUserToken());
		}

		/// <summary>
		/// Unsubscribe from MessagingCenter Facebook Login Events
		/// </summary>
		public void UnsubscribeFacebookLoginEvents()
		{
			MessagingCenter.Unsubscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_SUCCESS);
			MessagingCenter.Unsubscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_CANCEL);
			MessagingCenter.Unsubscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_ERROR);
			MessagingCenter.Unsubscribe<FacebookObject>(this, Settings.MS_FACEBOOK_GET_EMAIL);
		}

		/// <summary>
		/// Subscribe to MessagingCenter Facebook Login Events
		/// </summary>
		public void SubscribeFacebookLoginEvents()
		{
			MessagingCenter.Subscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_SUCCESS, FacebookLoginSuccess);
			MessagingCenter.Subscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_CANCEL, FacebookLoginCancel);
			MessagingCenter.Subscribe<string>(this, Settings.MS_FACEBOOK_LOGIN_ERROR, FacebookLoginError);
			MessagingCenter.Subscribe<FacebookObject>(this, Settings.MS_FACEBOOK_GET_EMAIL , FacebookGetEmailSuccess);
		}

		#endregion

    }
}
