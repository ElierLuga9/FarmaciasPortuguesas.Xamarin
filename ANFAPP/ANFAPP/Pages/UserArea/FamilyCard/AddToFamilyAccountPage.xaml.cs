using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.FamilyCard
{
    public partial class AddToFamilyAccountPage : ANFPage
    {
                
        #region Properties

        protected AddToFamilyAccountViewModel _viewModel;

        #endregion

        #region Page Initialization

        public AddToFamilyAccountPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            BindingContext = _viewModel = new AddToFamilyAccountViewModel();
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

        #region Buttons

        public void OnSubmitButton_Clicked(object sender, EventArgs args)
        {
            // Call webservice
            LoadingView.IsVisible = true;
            _viewModel.RequestCardAssociation();
        }

        public void OnFamilyModuleDetailsButton_Clicked(object sender, EventArgs args)
        {
            // Go to family module details page
            Navigation.PushAsync(new FamilyAccountDetailsPage());
        }

        #endregion

        #region Service Events

        void OnErrorEventHandler(string title, string message)
        {
            // Hide Loading
            LoadingView.IsVisible = false;

            // Show error message
            DisplayAlert(title, message, AppResources.OK);
            
        }

        async void OnSuccessEventHandler()
        {
            // Hide loading
            LoadingView.IsVisible = false;

            // Show success dialog
            await DisplayAlert(
                null,
                AppResources.AddToFamilyAccountSuccessMessage, 
                AppResources.OK);

            // Go to card page
            NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.AddToFamilyAccountPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
