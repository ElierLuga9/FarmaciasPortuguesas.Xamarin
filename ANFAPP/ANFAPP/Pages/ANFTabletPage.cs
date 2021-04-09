using System;
using ANFAPP.Logic;
using ANFAPP.Logic.Resources;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages
{
    public class ANFTabletPagenmasdasd : MasterDetailPage
    {

		public ANFTabletPagenmasdasd()
			: base()
        {
            // Initialize the page components.
            InitPage();

            // Disable Default Action Bar
            NavigationPage.SetHasNavigationBar(this, false);

            // Set the color for this page
            BackgroundColor = ColorResources.APPBackgroundLight;
        }

        /// <summary>
        /// Initializes the page.
        /// </summary>
        protected virtual void InitPage()
        {
            // Initializes the application bar
            InitApplicationBar();
        }

        /// <summary>
        /// Initializes the controls on the application bar.
        /// </summary>s
        protected void InitApplicationBar()
        {
            if (Detail == null) return;

            // Get Application Bar
            var ab = Detail.FindByName<ApplicationBar>("ApplicationBar");
            if (ab == null) return;

            // Left Button
            ab.SetLeftButtonType(GetAppBarLeftButtonType());

            // Set the title
            ab.SetTitle(GetAppBarTitle());

            // Sets the application bar color
            ab.SetApplicationBarColor(GetAppBarColor());

            // If user is logged, show points
            if (SessionData.IsLogged) ab.SetUserPoints(SessionData.PharmacyUser.Points);

            // Hide user button if not enabled
            //if (!HasAppBarUserButton()) ab.HideUserButton();
            if (!HasCardButton()) ab.HideUserButton();

            //hide menu button
            if (!HasMenuButton()) ab.HideMenuButton();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing ();

            var cid = Settings.AppSettings.GetValueOrDefault (Settings.ST_GA_CID, Settings.ST_GA_CID_DEFAULT);
            if (null != cid) {
                var title = this.Title ?? this.GetType ().Name;
                await App.Analytics.TrackScreen (cid, title);
            }
        }

        #region ApplicationBar Modifiers

        /// <summary>
        /// Returns the type of the AppBar's left button.
        /// </summary>
        /// <returns></returns>
        protected virtual ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE;
        }

        /// <summary>
        /// Returns if the AppBar has the User button on the top right corner.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasAppBarUserButton()
        {
            return true;
        }



        /// <summary>
        /// Returns if the AppBar has the Card button on.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasCardButton()
        {
            return true;
        }


        /// <summary>
        /// Returns if the AppBar has the Menu button on.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasMenuButton()
        {
            return false;
        }


        /// <summary>
        /// Returns the page title.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAppBarTitle()
        {
            return Title;
        }

        /// <summary>
        /// Returns the color of the application bar.
        /// </summary>
        /// <returns></returns>
        protected virtual Color GetAppBarColor()
        {
            return ColorResources.ANFGreen;
        }

        #endregion

    }
}
