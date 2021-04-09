using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Infragistics.XF;

namespace ANFAPP.Pages.BiometricData
{
    public partial class IMCPage : ANFPage
    {

        #region Page Initialization

        public IMCPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize the View Model
            BindingContext = App.BiometricIMCVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadingView.IsVisible = false;
			lineseries.Brush = new SolidColorBrush(Color.FromHex("#76bd22"));
            IMCWidget.OnNavigationStarted += OnIMCNavigationStarted;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            IMCWidget.OnNavigationStarted -= OnIMCNavigationStarted;
        }

        #endregion
        async Task OnIMCNavigationStarted()
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        void OnSwitch_Clicked(object sender, EventArgs args)
        {
            App.BiometricIMCVM.IsGraphToggled = !App.BiometricIMCVM.IsGraphToggled;
        }

        string GraphDateTime_FormatLabel(object sender, object item)
        {
            if (!(item is IMC)) return null;

            var date = (item as IMC).CreationDate;
            return date.ToString("d/M/yy");
        }

        #region Application Bar Methods

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.IMCPageTitle;
        }

        #endregion

    }
}
