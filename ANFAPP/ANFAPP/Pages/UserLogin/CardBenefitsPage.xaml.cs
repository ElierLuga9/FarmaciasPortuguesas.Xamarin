using ANFAPP.Logic;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserLogin
{
    public partial class CardBenefitsPage : ANFPage
    {

        #region Page Initialization

        public CardBenefitsPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.CardBenefitsPageTitle;
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
