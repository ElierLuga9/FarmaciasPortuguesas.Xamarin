using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Pages.PharmacyLocator;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Utils;
using XLabs.Ioc;
using XLabs.Platform.Device;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Contacts
{
    public partial class PrivacyPolicyPage : ANFPage
    {

        #region Page Initialization

		public PrivacyPolicyPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

		#endregion

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			HideLoading();
		}

        #region ApplicationBar

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
			return AppResources.ContactsPageTitle;
        }

        #endregion
		
    }
}
