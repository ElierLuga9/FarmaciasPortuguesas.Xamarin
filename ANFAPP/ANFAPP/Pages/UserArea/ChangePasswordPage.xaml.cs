using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea
{
    public partial class ChangePasswordPage : ANFPage
    {

        #region Properties

        public ChangePasswordViewModel _viewModel { get; set; }

        #endregion

        #region Page Initialization

        public ChangePasswordPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize context
            this.BindingContext = _viewModel = new ChangePasswordViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnError += OnErrorEventHandler;
            _viewModel.OnSuccess += OnSuccessEventHandler;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnError -= OnErrorEventHandler;
            _viewModel.OnSuccess -= OnSuccessEventHandler;
        }

        #endregion

        #region Button Events

        async void OnSubmitButtonClick(object sender, EventArgs args)
        {
            // Show loading
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Call webservice
            _viewModel.ChangePassword();
        }

        #endregion

        #region ViewModel Events

        void OnErrorEventHandler(string title, string message)
        {
            LoadingView.IsVisible = false;

            // Show error message
            DisplayAlert(null, message, AppResources.OK);
        }

        async void OnSuccessEventHandler()
        {
            LoadingView.IsVisible = false;

            // Show success message
            await DisplayAlert(null, AppResources.ChangePasswordSuccessMessage, AppResources.OK);

            // Go back to main page
            NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
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




        protected override string GetAppBarTitle()
        {
            return AppResources.UserAreaPageChangePasswordTitle;
        }

        #endregion

    }
}
