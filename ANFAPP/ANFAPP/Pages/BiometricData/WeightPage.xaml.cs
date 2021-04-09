using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Utils;
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
    public partial class WeightPage : ANFPage
    {

        #region Page Initialization

        public WeightPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize the View Model
            BindingContext = App.BiometricWeightVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Device.OS == TargetPlatform.iOS) 
            { 
                //ValueInput.AllowedCharacters = "0123456789" + App.DecimalSeparator;
            }

            LoadingView.IsVisible = false;
            App.BiometricWeightVM.OnLoadComplete += OnLoadComplete;
			lineseries.Brush = new SolidColorBrush(Color.FromHex("#76bd22"));
			// Reset the default time to the current time (it is the time presented to the user so this should be the local time).
			//App.BiometricWeightVM.TimeInput = DateTime.Now.TimeOfDay;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.BiometricWeightVM.OnLoadComplete -= OnLoadComplete;
			App.BiometricWeightVM.TimeInput = DateTime.Now.TimeOfDay;
        }

        #endregion

        async void OnLoadComplete()
        {
            await App.BiometricIMCVM.ReloadIMCTable();
            LoadingView.IsVisible = false;
        }

        void OnSwitch_Clicked(object sender, EventArgs args)
        {
            App.BiometricWeightVM.IsGraphToggled = !App.BiometricWeightVM.IsGraphToggled;
        }

        async void SubmitButton_Clicked(object sender, EventArgs args)
        {
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("Value", App.BiometricWeightVM.ValueInput + string.Empty);
			mixpanelWidget.TrackProperties("WeightData_Added", props);

            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            App.BiometricWeightVM.InsertNewEntry();
        }

        async void RemoveEntry_Click(object sender, EventArgs args)
        {
            if (sender == null || !(sender is View)) return;
            var context = (sender as View).BindingContext as Weight;

            // Confirm operation
            var accepted = await DisplayAlert(null,
                string.Format(AppResources.BiometricRemoveConfirmationMessage, context.CreationDate.ToString("dd/MM/yyyy")),
                AppResources.Yes,
                AppResources.No);

            if (accepted && context != null)
            {
                LoadingView.IsVisible = true;
				await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

                // Remove entry
                App.BiometricWeightVM.RemoveEntry(context);
            }
        }

        string GraphDateTime_FormatLabel(object sender, object item)
        {
            if (!(item is Weight)) return null;

            var date = (item as Weight).CreationDate;
            return date.ToString("d/M/yy");
        }

        #region Application Bar Methods

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.WeightPageTitle;
        }

        #endregion

    }
}
