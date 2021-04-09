using System;
using System.Collections.Generic;
using ANFAPP;
using ANFAPP.Pages;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;

namespace ANFAPP.Pages.UserLogin
{
    public partial class AssociateCardPage : ANFPage
    {

        #region Page Initialization

        public AssociateCardPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Bind the View Model
            App.AssociateCardVM.ClearForm();
            BindingContext = App.AssociateCardVM;
        }

        #endregion

        /// <summary>
        /// Add the event handlers
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.AssociateCardVM.OnError += OnErrorEventHandler;
            App.AssociateCardVM.OnSuccess += OnSuccessEventHandler;
        }

        /// <summary>
        /// Remove the event handlers
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.AssociateCardVM.OnError -= OnErrorEventHandler;
            App.AssociateCardVM.OnSuccess -= OnSuccessEventHandler;
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
            await DisplayAlert(AppResources.AssociateCardMessageTitle, AppResources.AssociateCardSuccessMessage, AppResources.OK);

            var mixpanelWidget = DependencyService.Get<IMixPanel> ();
			var props = new Dictionary<string, string> ();
			if (SessionData.IsAuthenticatedWithPharmacy) props.Add("PharmacyID", SessionData.StorePharmacyId);
			mixpanelWidget.TrackProperties ("AssociateCard", props);

//            var alias = SessionData.PharmacyUser.Username;
//			mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
//            mixpanelWidget.Identify(alias);

			//var props = new Dictionary<string,object> ();
			//props.Add("$created", DateTime.UtcNow);
			//mixpanelWidget.PeopleSet(props);

            // Go to the landing page
			// Navigation.InsertPageBefore(new UserCardPage(), Navigation.NavigationStack[0]);

			// Pop to user card page
			// NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);

			await Navigation.PushAsync(new PharmacySelectionPage());
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
            App.AssociateCardVM.IsTermsChecked = !App.AssociateCardVM.IsTermsChecked;
        }

        public void IDButton_Clicked(object sender, EventArgs args)
        {
            App.AssociateCardVM.IsBISelected = true;
        }

        public void PassportButton_Clicked(object sender, EventArgs args)
        {
            App.AssociateCardVM.IsBISelected = false;
        }

        public async void SubmitButton_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            App.AssociateCardVM.SubmitForm();
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.AssociateCardTitle;
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
