using System;
using System.Collections.Generic;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Utils;
using ANFAPP.Pages.UserArea;
using System.Threading.Tasks;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Database.DAOs;

namespace ANFAPP.Pages.UserLogin
{
	public partial class QuickRegisterPage : ANFPage
	{

		#region Enums

		public enum LoginAction { FACEBOOK_REDIRECT, REFRESH_LOGIN };

		#endregion

		#region Properties

		private QuickRegisterViewModel _viewModel = new QuickRegisterViewModel();
		private LoginViewModel _loginViewModel = new LoginViewModel();

		private LoginAction? _loginAction = null;

		#endregion

		#region Page Initialization

		public QuickRegisterPage() { }

		public QuickRegisterPage(LoginAction action)
		{
			//_loginAction = action;

		}

		protected override void InitPage()
		{

			InitializeComponent();
			base.InitPage();

			// Bind the View Model
			BindingContext = _viewModel;

		}

		#endregion

		/// <summary>
		/// Add the event handlers
		/// </summary> 
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			LoadingView.IsVisible = false;
			_loginViewModel.OnFacebookLoginCancel += OnFacebookLoginCancel;
			_loginViewModel.OnFacebookLoginSuccess += OnFacebookLoginSuccess;

			_loginViewModel.SubscribeFacebookLoginEvents();
			if (_loginAction == LoginAction.FACEBOOK_REDIRECT)
			{
				// WinPhone only - because facebook login will restart the app
				_loginAction = null;
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

				// Continue facebook login
				DependencyService.Get<IFacebookSDK>().GetUserEmail();

			}

			/*_viewModel.OnError += OnErrorEventHandler;
			_viewModel.OnRegistrationSuccess += OnRegisterSuccessEventHandler;
			_loginViewModel.OnLoginSuccess += OnLoginSuccessEventHandler;
			_loginViewModel.OnError += OnErrorEventHandler;
			_loginViewModel.OnFacebookLoginCancel += OnFacebookLoginCancel;
			_loginViewModel.OnFacebookLoginSuccess += OnFacebookLoginSuccess;

		//	_loginViewModel.SubscribeFacebookLoginEvents();

			if (!_loginAction.HasValue) return;
			/*if (_loginAction == LoginAction.FACEBOOK_REDIRECT)
			{
				// WinPhone only - because facebook login will restart the app
				_loginAction = null;
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

				// Continue facebook login
				DependencyService.Get<IFacebookSDK>().GetUserEmail();
			}
			if (_loginAction == LoginAction.REFRESH_LOGIN)
			{
				_loginAction = null;
				LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

				// Refresh the login
				RefreshLogin();
			}*/
		}

		/// <summary>
		/// Remove the event handlers
		/// </summary>
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnError -= OnErrorEventHandler;
			_viewModel.OnRegistrationSuccess -= OnRegisterSuccessEventHandler;
			_loginViewModel.OnLoginSuccess -= OnLoginSuccessEventHandler;
			_loginViewModel.OnError -= OnErrorEventHandler;
			_loginViewModel.OnFacebookLoginCancel -= OnFacebookLoginCancel;
			_loginViewModel.OnFacebookLoginSuccess -= OnFacebookLoginSuccess;

		//	_loginViewModel.UnsubscribeFacebookLoginEvents();
		}

		#region Event Handlers

		void OnFacebookLoginCancel()
		{
			LoadingView.IsVisible = false;
		}

		void OnErrorEventHandler(string title, string message)
		{
			// Show error message
			DisplayAlert(title, message, AppResources.OK);
			LoadingView.IsVisible = false;
		}

		async void OnRegisterSuccessEventHandler()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();

			mixpanelWidget.CreateAliasForDistinctId(_viewModel.Email, mixpanelWidget.DistinctId());

			if (Device.OS == TargetPlatform.iOS) {
				mixpanelWidget.Identify(mixpanelWidget.DistinctId());	
			}

			mixpanelWidget.Track("RegisterWithEmail");

			var alias = _viewModel.Email;
//			mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
//			mixpanelWidget.Identify(alias);

			// Success message and go to login
			await DisplayAlert(null, AppResources.QuickRegisterSuccessMessage, AppResources.OK);
			await Navigation.PushAsync(new LoginPage());
		}

		async void OnLoginSuccessEventHandler(bool hasCard) 
		{
			if (SessionData.PharmacyUser != null)
			{
//				mixpanelWidget.CreateAliasForDistinctId(SessionData.PharmacyUser.Username, mixpanelWidget.DistinctId());
//				mixpanelWidget.Identify(SessionData.PharmacyUser.Username);

//				var props = new Dictionary<string, string>();
//				props.Add("Email", SessionData.PharmacyUser.Username);
//				props.Add ("Name", SessionData.PharmacyUser.Name); 
//				props.Add("CardNumber", SessionData.PharmacyUser.CardNumber);
//
//				mixpanelWidget.TrackProperties(SessionData.FacebookLogin ? "Login Facebook" : "Login", props);

				var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//				mixpanelWidget.CreateAliasForDistinctId(SessionData.PharmacyUser.Username, mixpanelWidget.DistinctId());

				if (SessionData.FacebookLogin) {
					mixpanelWidget.CreateAliasForDistinctId(SessionData.FaceBookEmail, mixpanelWidget.DistinctId()); //TODO: IMPLEMENTAR MIXPANEL EM IOS para resolver crash

					if (Device.OS == TargetPlatform.iOS) {
					//	mixpanelWidget.Identify(mixpanelWidget.DistinctId());	TODO: IMPLEMENTAR MIXPANEL EM IOS para resolver crashTODO: IMPLEMENTAR MIXPANEL EM IOS para resolver crash
					}
				} else {
					mixpanelWidget.Identify(SessionData.PharmacyUser.Username);					
				}

				var props = new Dictionary<string,string> ();
				props.Add("$email", SessionData.PharmacyUser.Username);
				props.Add("$name", SessionData.PharmacyUser.Name); 
				props.Add("CardNumber", SessionData.PharmacyUser.CardNumber);
				mixpanelWidget.PeopleSet(props);
				mixpanelWidget.PeopleIncrement("Points", SessionData.PharmacyUser.Points);


				mixpanelWidget.Track(SessionData.FacebookLogin ? "Login Facebook" : "Login");

			}

			if (hasCard)
			{
				// Go to menu
				// await Navigation.PushAsync(new HomePage());
				await NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);
			}
			else
			{
				// Go to card benefits page
				//await Navigation.PushAsync(new UserNotLoggedCardPage());
				await NavigationUtils.PushPageAndClearHistory(new UserNotLoggedCardPage(), Navigation);
			}
		}

		void OnFacebookLoginSuccess()
		{
//			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			if (SessionData.PharmacyUser == null) return;
			Navigation.PushAsync(new HomePage());
		}

		protected async void RefreshLogin()
		{
			bool result = false;
			if (SessionData.UserAuthentication != null)
			{
				result = await _loginViewModel.LoadUser(new AuthorizationOut()
				{
					Authorization = new AuthorizationOut.AuthorizationModel()
					{
						UserName = SessionData.PharmacyUser.Username,
						AccessToken = SessionData.UserAuthentication.AccessToken,
						RefreshToken = SessionData.UserAuthentication.RefreshToken,
						ExpiresIn = 60
					}
				});
			}

			if (!result || (SessionData.PharmacyUser != null && !SessionData.IsAuthenticated))
			{
				// Failed to log into magento or I9WS - Force Logout
				SessionData.ClearUser();

				// Clear the vouchers.
				var vdao = new VouchersDAO();
				await vdao.DeleteAll();

				// Clear the pharmacies (recents and favorites).
				var pharmacyDAO = new PharmacyDAO();
				await pharmacyDAO.DeleteAll();

				HideLoading();
			}
		}

		#endregion

		#region Button Events

		async void LoginButton_Click(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			// Navigate to the page
			await Navigation.PushAsync(new LoginPage());
		}

		async void EnterAsGuestButton_Click(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			mixpanelWidget.Track("Guest");

			// Go to Menu
			await Navigation.PushAsync(new HomePage());
		}

		async void LoginWithFacebookClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_loginViewModel.FacebookLogin();
			//_loginViewModel.FacebookGetEmailSuccess();
		}

		void RegisterButton_Click(object sender, EventArgs args)
		{
			
			LoadingView.IsVisible = true;
			
			_viewModel.RegisterUser();
		}
		void SignupButtonClicked(object sender, EventArgs args)
		{
			NavigationUtils.PushPageKeepHistory(new RegisterPage(), Navigation);

		}

		#endregion

	}
}
