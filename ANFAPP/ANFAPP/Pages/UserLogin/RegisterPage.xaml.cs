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
    public partial class RegisterPage : ANFPage
    {

		#region Properties

		private RegisterViewModel _viewModel = new RegisterViewModel();

		#endregion

        #region Page Initialization

		public RegisterPage() : base() { }

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
			_viewModel.OnRegistrationSuccess += OnRegisterSuccessEventHandler;

        }

        /// <summary>
        /// Remove the event handlers
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

			_viewModel.OnError -= OnErrorEventHandler;
			_viewModel.OnRegistrationSuccess -= OnRegisterSuccessEventHandler;
        }

        #region Event Handlers

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

			if (Device.OS == TargetPlatform.iOS)
			{
				mixpanelWidget.Identify(mixpanelWidget.DistinctId());
			}

			mixpanelWidget.Track("RegisterWithEmail");

			var alias = _viewModel.Email;
			//			mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
			//			mixpanelWidget.Identify(alias);

			// Success message and go to login
			await DisplayAlert(null, AppResources.QuickRegisterSuccessMessage, AppResources.OK);
			await Navigation.PushAsync(new QuickRegisterPage());
		}

        #endregion

        #region Button Events

        
		protected void OnBackButton_Clicked(object sender, EventArgs e)
		{
			// Pop the current page
			if (Navigation.NavigationStack.Count > 1)
			{
				Navigation.PopAsync(true);
			}
		}

		void RegisterButton_Click(object sender, EventArgs args)
		{

			LoadingView.IsVisible = true;

			_viewModel.RegisterUser();
		}

        #endregion

    }
}
