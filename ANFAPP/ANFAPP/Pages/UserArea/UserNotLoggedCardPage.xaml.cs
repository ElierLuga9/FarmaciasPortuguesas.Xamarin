using ANFAPP.Logic;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using ANFAPP.Views.Common;
using ANFAPP.Logic.ViewModels;

namespace ANFAPP.Pages.UserArea
{
    public partial class UserNotLoggedCardPage : ANFPage
    {

		#region Page Initialization

		private UserNotLoggedViewModel viewModel;
        public UserNotLoggedCardPage() : base() { }

        protected override void InitPage()
        {
			
            InitializeComponent();
            base.InitPage();

        }

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			LoadingView.IsVisible = false;
		}
		protected override void OnAppearing()
		{
			BindingContext = viewModel = new UserNotLoggedViewModel();
			viewModel.GetPromoBanner();
			LoadingView.IsVisible = false;
		}
		#endregion
        #region Buttons

        void RegisterCardButton_Click(object sender, EventArgs args)
        {
            // Go to card register page
            Navigation.PushAsync(new RegisterCardPage());
        }

        void AssociateCardButton_Click(object sender, EventArgs args)
        {
            // Go to card association page
            Navigation.PushAsync(new AssociateCardPage());
        }

		void LeaveForLaterButton_Click(object sender, EventArgs args)
        {
            // Go to the menu page
            Navigation.PushAsync(new HomePage());
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
            return AppResources.UserCardPageTitle;
        }


        //testes
        protected override bool HasCardButton()
        {
            return true;
        }
        #endregion

    }
}
