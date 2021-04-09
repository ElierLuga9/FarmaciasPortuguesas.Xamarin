using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.Pages.PharmacyLocator;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserLogin
{
    public partial class PharmacySelectionPage : ANFPage
    {
        private PharmacySelectionViewModel _vm = new PharmacySelectionViewModel();

        #region Page Initialization

        public PharmacySelectionPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _vm;
        }

        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();

			LoadingView.IsVisible = true;

			_vm.OnError     += OnLoadError;
			_vm.OnStart     += OnLoadStart;
			_vm.OnSuccess   += OnLoadSuccess;

			_vm.LoadSuggestedPharmacy ();
			/*
            MessagingCenter.Subscribe<Location>(this, Settings.MS_LOCATOR_GOT_LOCATION, (userLocation) => {
                SessionData.UserLocation = userLocation;
            });
            DependencyService.Get<ILocationServices> ().StartUpdatingLocation ();
			*/
        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();

            _vm.OnError     -= OnLoadError;
            _vm.OnStart     -= OnLoadStart;
            _vm.OnSuccess   -= OnLoadSuccess;

			/*
            MessagingCenter.Unsubscribe<Location> (this, Settings.MS_LOCATOR_GOT_LOCATION);
            DependencyService.Get<ILocationServices> ().StopUpdatingLocation();
            */
        }
    
        #region Button Events

        public void GoToLocatorClicked(object sender, EventArgs args)
        {
			NavigationUtils.PushPageAndClearHistory (new DashboardLocator (), Navigation);
        }

        public async void SaveSelectionClicked(object sender, EventArgs args)
        {
            //Call setmyfarm;  
            await _vm.SetUserFarm();

            // Go to the landing page
            Navigation.InsertPageBefore(new UserCardPage(), Navigation.NavigationStack[0]);

            // Pop to user card page
            NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);    
        }
			
        public void SelectLaterClicked(object sender, EventArgs args)
        {
            // Go to the landing page
            Navigation.InsertPageBefore(new UserCardPage(), Navigation.NavigationStack[0]);

            // Pop to user card page
            NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);        
        }

        #endregion

        #region Events

        async Task OnLoadStart()
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;
        }

        void OnLoadError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
			return AppResources.PickPharmacyTitle;
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