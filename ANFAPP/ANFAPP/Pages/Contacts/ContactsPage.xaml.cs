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
using ANFAPP.Pages.Tutorial;
using ANFAPP.Logic;

namespace ANFAPP.Pages.Contacts
{
    public partial class ContactsPage : ANFPage
    {

        #region Page Initialization

        public ContactsPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

//			var mixpanelWidget = DependencyService.Get<IMixPanel>();
//			var props = new Dictionary<string, string>();
//			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
//			mixpanelWidget.TrackProperties("Contacts", props);
        }

		#endregion

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			HideLoading();
		}

		void ContactCenterTap(object sender, EventArgs args)
		{
			var device = Resolver.Resolve<IDevice>();
			if (device.PhoneService != null)
			{
				string phoneUrl =
					(Device.OS == TargetPlatform.iOS ? "telprompt:" : "tel:")
					+ "707273273";

				Device.OpenUri(new Uri(phoneUrl));

			}
			else
			{
				DisplayAlert(AppResources.DAAtencionTitle, AppResources.DAAtencionBody, "OK");
			}
		}

		void ContactEmailTap(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri("mailto:" + AppResources.ContactSupportEmail));
		}

		void OnGeralConditionsButtonClick(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri(Settings.GENERAL_CONDITIONS_URL));
		}

		void OnPrivacyPolicyButtonClick(object sender, EventArgs args)
		{
			Device.OpenUri(new Uri(Settings.PRIVACY_POLICY_URL));
		}

		async void OpenTutorialButtonClick(object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			NavigationUtils.PushPageAndClearHistory(new TutorialStep1Page(), Navigation);
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
