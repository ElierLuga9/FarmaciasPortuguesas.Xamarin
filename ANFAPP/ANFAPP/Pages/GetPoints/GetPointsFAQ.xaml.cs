using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.GetPoints;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.UserArea;

namespace ANFAPP.Pages.GetPoints
{
    public partial class GetPointsFAQ : ANFPage
    {
        public GetPointsFAQ() : base() { }

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}

        #region Lifecycle Events

        protected override void OnAppearing()
        {
            LoadingView.IsVisible = false;
            base.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        #endregion


        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.GetPointsTitle;
        }

        protected override string GetAppBarSubtitle()
        {
            return AppResources.GetPointsFAQPageTitle;
        }

        #endregion
    }


}
