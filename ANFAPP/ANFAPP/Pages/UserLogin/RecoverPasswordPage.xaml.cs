using ANFAPP.Logic;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserLogin
{
    public partial class RecoverPasswordPage : ANFPage
    {

        #region Page Initialization

        public RecoverPasswordPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Bind the View Model
            BindingContext = App.RecoverPasswordVM;
            App.RecoverPasswordVM.ClearForm();
        }

        #endregion

        /// <summary>
        /// Add the event handlers
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.RecoverPasswordVM.OnError += OnErrorEventHandler;
            App.RecoverPasswordVM.OnSuccess += OnSuccessEventHandler;
        }

        /// <summary>
        /// Remove the event handlers
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.RecoverPasswordVM.OnError -= OnErrorEventHandler;
            App.RecoverPasswordVM.OnSuccess -= OnSuccessEventHandler;
        }

        #region Event Handlers

        async void OnSuccessEventHandler()
        {
            // Show error message
            LoadingView.IsVisible = false;
            await DisplayAlert(
                AppResources.RecoverPasswordMessageTitle, 
                AppResources.RecoverPasswordSuccessMessage, 
                AppResources.OK);

            // Go back to login
            await Navigation.PopAsync();
        }

        void OnErrorEventHandler(string title, string message)
        {
            // Show error message
            DisplayAlert(null, message, AppResources.OK);
            LoadingView.IsVisible = false;
        }

        #endregion

        #region Button Events

        void RecoverButton_Click(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            App.RecoverPasswordVM.RecoverPassword();
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.RecoverPasswordTitle;
        }

        protected override bool HasAppBarUserButton()
        {
            return false;
        }



        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
