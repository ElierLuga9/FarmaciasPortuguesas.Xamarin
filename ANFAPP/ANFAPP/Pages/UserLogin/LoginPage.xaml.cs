using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Database.DAOs;
using System.Collections.Generic;

namespace ANFAPP.Pages.UserLogin
{
    public partial class LoginPage : ANFPage
    {

		#region Properties

		private LoginViewModel _viewModel = new LoginViewModel();
		private bool card;

		#endregion

        #region Page Initialization

		public LoginPage() : base() { }

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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadingView.IsVisible = false;

			_viewModel.OnError += OnErrorEventHandler;
			_viewModel.OnLoginSuccess += OnLoginSuccessEventHandler;
        }

        /// <summary>
        /// Remove the event handlers
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

			_viewModel.OnError -= OnErrorEventHandler;
			_viewModel.OnLoginSuccess -= OnLoginSuccessEventHandler;
        }

        #region Event Handlers

        void OnErrorEventHandler(string title, string message)
        {
            // Show error message
            DisplayAlert(title, message, AppResources.OK);
            LoadingView.IsVisible = false;
        }

        async void OnLoginSuccessEventHandler(bool hasCard)
        {
			// Force the next biometric update.
			SessionData.BiometricDataTS = new DateTime(1);

			if (!string.Equals(SessionData.PharmacyUser.Username, SessionData.LastLoginName)) {
				// Clear the previous user biometric data.
				await App.BiometricHeightVM.RemoveAllEntries ();
				await App.BiometricWeightVM.RemoveAllEntries ();
				await App.BiometricGlicemiaVM.RemoveAllEntries ();
				await App.BiometricArterialPressureVM.RemoveAllEntries ();
				await App.BiometricCholesterolVM.RemoveAllEntries ();
				await App.BiometricAbdominalPerimeterVM.RemoveAllEntries ();
				await App.BiometricTriglyceridesVM.RemoveAllEntries ();
				await App.BiometricIMCVM.RemoveAllEntries ();

				// Clear the dosage data.
				var dosagesDAO = new DosageDAO();
				var dosingSchedulesDAO = new DosingScheduleDAO();
				var medicineDAO = new MedicineDAO();

				await dosagesDAO.DeleteAll ();
				await dosingSchedulesDAO.DeleteAll ();
				await medicineDAO.DeleteAll ();
			}

			SessionData.LastLoginName = SessionData.PharmacyUser.Username; 

			// Mixpanel event.
            var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//			mixpanelWidget.CreateAliasForDistinctId(SessionData.PharmacyUser.Username, mixpanelWidget.DistinctId());
			mixpanelWidget.SharedInstanceWithToken(ANFAPP.Logic.Settings.MIXPANEL_KEY);
			mixpanelWidget.Identify(SessionData.PharmacyUser.Username);

			var props = new Dictionary<string,string> ();
			props.Add("$email", SessionData.PharmacyUser.Username);
			props.Add ("$name", SessionData.PharmacyUser.Name); 
			props.Add("CardNumber", SessionData.PharmacyUser.CardNumber);
			mixpanelWidget.PeopleSet(props);
			mixpanelWidget.PeopleIncrement("Points", SessionData.PharmacyUser.Points);
            mixpanelWidget.Track ("Login");

            // Add Menu on root
			if (hasCard) 
			{
				// Pop to user card page
				await Navigation.PushAsync(new HomePage());
				//await NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);

				//NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);
				//Navigation.InsertPageBefore(new UserCardPage(), Navigation.NavigationStack[0]);
			}
			else
			{
				// Pop to user card page
				await Navigation.PushAsync(new UserNotLoggedCardPage());

				//Navigation.InsertPageBefore(new UserNotLoggedCardPage(), Navigation.NavigationStack[0]);
				//NavigationUtils.PopToPageType(typeof(UserNotLoggedCardPage), Navigation);
			}
        }

        #endregion

        #region Button Events

        void LoginButton_Click(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			_viewModel.Login();
			//if (card) NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);
			//else NavigationUtils.PushPageAndClearHistory(new UserNotLoggedCardPage(), Navigation);

			//		NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);

		}
		protected void OnBackButton_Clicked(object sender, EventArgs e)
		{
			// Pop the current page
			if (Navigation.NavigationStack.Count > 1)
			{
				Navigation.PopAsync(true);
			}
		}

		async void RecoverPasswordButton_Click(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Password Recover Page
            await Navigation.PushAsync(new RecoverPasswordPage());
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

		protected override string GetAppBarTitle()
		{
			return AppResources.LoginPageTitle;
		}

        protected override bool HasAppBarUserButton()
        {
            return false;
        }
        //testes
        protected override bool HasCardButton()
        {
            return false;
        }

        #endregion

    }
}
