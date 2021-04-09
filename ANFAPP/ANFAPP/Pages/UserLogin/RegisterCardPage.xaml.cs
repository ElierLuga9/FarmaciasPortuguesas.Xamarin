using System;
using System.Collections.Generic;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;

namespace ANFAPP.Pages.UserLogin
{
    public partial class RegisterCardPage : ANFPage
    {
        #region Page Initialization

        public RegisterCardPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Bind the View Model
            App.RegisterCardVM.ClearForm();
            BindingContext = App.RegisterCardVM;
        }

        #endregion

        /// <summary>
        /// Add the event handlers
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.RegisterCardVM.OnError += OnErrorEventHandler;
            App.RegisterCardVM.OnSuccess += OnSuccessEventHandler;
        }

        /// <summary>
        /// Remove the event handlers
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.RegisterCardVM.OnError -= OnErrorEventHandler;
            App.RegisterCardVM.OnSuccess -= OnSuccessEventHandler;
        }

        #region Event Handlers

        void OnErrorEventHandler(string title, string message)
        {
            // Show error message
            DisplayAlert(title, message, AppResources.OK);
            LoadingView.IsVisible = false;
        }

        async void OnSuccessEventHandler()
        {
            // Show error message
            LoadingView.IsVisible = false;
            await DisplayAlert(AppResources.RegisterCardMessageTitle, AppResources.RegisterCardSuccessMessage, AppResources.OK);

            var mixpanelWidget = DependencyService.Get<IMixPanel> ();
            mixpanelWidget.Track ("RegisterCard");

            var alias = SessionData.PharmacyUser.Username;
//            mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
//            mixpanelWidget.Identify(alias);

            //var props = new Dictionary<string,object> ();
            //props.Add("$created", DateTime.UtcNow);
            //mixpanelWidget.PeopleSet(props);

			// Go to the landing page
			//Navigation.InsertPageBefore(new UserCardPage(), Navigation.NavigationStack[0]);

			// Pop to user card page
			//NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);
            if (!(SessionData.IsPharmacySelected))
            {
                Navigation.PushAsync(new PharmacySelectionPage());
            }
            else 
            {
                Navigation.PushAsync(new  UserCardPage());
            }
        }

        #endregion

        #region Button Events

		public void TermsAndConditionsButton_Clicked(object sender, EventArgs args)
        {
            // Go to Terms and Conditions Page
            Navigation.PushAsync(new TermsAndConditionsPage());
        }

        public void TermsAndConditionsCheckbox_Clicked(object sender, EventArgs args)
        {
            App.RegisterCardVM.IsTermsChecked = !App.RegisterCardVM.IsTermsChecked;
        }

		public void AllowPromotionsCheckbox_Clicked(object sender, EventArgs args)
        {
			App.RegisterCardVM.AllowPromotions = !App.RegisterCardVM.AllowPromotions;
        }

        public void MaleButton_Clicked(object sender, EventArgs args)
        {
            App.RegisterCardVM.IsMale = true;
        }

        public void FemaleButton_Clicked(object sender, EventArgs args)
        {
            App.RegisterCardVM.IsMale = false;
        }

        public void IDButton_Clicked(object sender, EventArgs args)
        {
            App.RegisterCardVM.IsBISelected = true;
        }

        public void PassportButton_Clicked(object sender, EventArgs args)
        {
            App.RegisterCardVM.IsBISelected = false;
        }

        public async void SubmitButton_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            await App.RegisterCardVM.SubmitForm();
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.RegisterCardTitle;
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
