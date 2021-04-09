using ANFAPP.Logic;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages
{
    public partial class NewUserPage : ANFPage
    {

        #region Page Initialization

        public NewUserPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            BindingContext = App.UsersViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadingView.IsVisible = false;

            App.UsersViewModel.OnInsertSuccess += OnInsertSuccess;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.UsersViewModel.OnInsertSuccess -= OnInsertSuccess;
        }

        #endregion

        public void MaleButton_Clicked(object sender, EventArgs args)
        {
            // Insert new male user
            App.UsersViewModel.InsertNewUser(true);
        }

        public void FemaleButton_Clicked(object sender, EventArgs args) 
        {
            // Insert new female user
            App.UsersViewModel.InsertNewUser(false);
        }

        #region ViewModel Events

        /// <summary>
        /// Called when the new user is inserted
        /// </summary>
        public async void OnInsertSuccess()
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			//await Navigation.PushAsync(new DashboardPage());
			//Navigation.RemovePage(this);
			NavigationUtils.PushPageAndClearHistory(new DashboardPage(), Navigation);
			//await NavigationUtils.PushPageWithNoHistory(new DashboardPage(), Navigation);
        }

        #endregion

        #region Application Bar Methods

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
            return AppResources.NewUserPageTitle;
        }

        #endregion

    }
}
