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

namespace ANFAPP.Pages
{
    public partial class BarcodePage : ANFPage
    {

		private bool _isRotated = false;

        #region Page Initialization

        public BarcodePage() : base() { }

        public BarcodePage(string cardNumber, string optNumber=null, string cardName=null)
            : base()
        {
            this.BindingContext = new BarcodeViewModel()
            {
                CardNumber = cardNumber, OptNumber = optNumber, CardName = cardName
            };
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);

			if (!LayoutUtils.IsLandscape (this) && !_isRotated) {
				// Rotate on Portrait
				BarCodeContainer.Rotation = 270;
				_isRotated = true;
			} 
			else if (LayoutUtils.IsLandscape (this) && _isRotated) 
			{
				// Reset rotation for landscape
				BarCodeContainer.Rotation = 0;
				_isRotated = false;
			}
		}

        #endregion

        #region ApplicationBar

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.UserCardPageTitle;
        }

        #endregion

    }
}
