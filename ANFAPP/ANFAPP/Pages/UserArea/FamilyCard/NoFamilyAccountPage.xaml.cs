using ANFAPP.Logic;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.FamilyCard
{
    public partial class NoFamilyAccountPage : ANFPage
    {

        #region Page Initialization

        public NoFamilyAccountPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage(); 
        }

        #endregion

        #region Buttons

        void CreateFamilyAccountButton_Clicked(object sender, EventArgs args)
        {
            // Go to family creation page
            Navigation.PushAsync(new CreateFamilyAccountPage());
        }

        void AddToFamilyAccountButton_Clicked(object sender, EventArgs args)
        {
            // Go to the page where a card can be added to a family account
            Navigation.PushAsync(new AddToFamilyAccountPage());
        }

        void FamilyModelDetailsButton_Clicked(object sender, EventArgs args)
        {
            // Go to family account detail page
            Navigation.PushAsync(new FamilyAccountDetailsPage());
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.NoFamilyAccountPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
