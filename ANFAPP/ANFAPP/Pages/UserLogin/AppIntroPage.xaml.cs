using ANFAPP.Logic;
using ANFAPP.Pages.Tutorial;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserLogin
{
    public partial class AppIntroPage : ANFPage
    {

        #region Page Initialization

        public AppIntroPage() : base() 
        {
            // Set flag to false so this page doesnt appear ever again.
            SessionData.IsFirstRun = false;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();
		}

        #endregion

        async void ContinueButton_Click(object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			NavigationUtils.PushPageAndClearHistory(new TutorialStep1Page(), Navigation);
        }
    }
}
