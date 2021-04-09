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
    public partial class FamilyAccountDetailsPage : ANFPage
    {

        #region Page Initialization

        public FamilyAccountDetailsPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

        #region Buttons

        

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.FamilyAccountDetailPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
