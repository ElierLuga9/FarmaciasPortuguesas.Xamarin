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
    public partial class HeightPage : ANFPage
    {

        #region Page Initialization

        public HeightPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            yAxis.FormatLabel += (object sender, object item) => {
                return string.Format("{0:0.##}", item);
            };

            // Initialize the View Model
            BindingContext = App.BiometricHeightVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Device.OS == TargetPlatform.iOS) 
            { 
                ValueInput.AllowedCharacters = "0123456789" + App.DecimalSeparator;
            }
			lineseries.Brush = new SolidColorBrush(Color.FromHex("#76bd22"));
            LoadingView.IsVisible = false;
            App.BiometricHeightVM.OnLoadComplete += OnLoadComplete;

			// Reset the default time to the current time (it is the time presented to the user so this should be the local time).
			//App.BiometricHeightVM.TimeInput = DateTime.Now.TimeOfDay;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.BiometricHeightVM.OnLoadComplete -= OnLoadComplete;
			App.BiometricHeightVM.TimeInput = DateTime.Now.TimeOfDay;
        }

        #endregion

        async void OnLoadComplete()
        {
            await App.BiometricIMCVM.ReloadIMCTable();
            LoadingView.IsVisible = false;
        }

        void OnSwitch_Clicked(object sender, EventArgs args)
        {
            App.BiometricHeightVM.IsGraphToggled = !App.BiometricHeightVM.IsGraphToggled;
        }

        async void SubmitButton_Clicked(object sender, EventArgs args)
        {
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("Value", App.BiometricHeightVM.ValueInput + string.Empty);
			mixpanelWidget.TrackProperties("HeightData_Added", props);

            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            App.BiometricHeightVM.InsertNewEntry();
        }

        async void RemoveEntry_Click(object sender, EventArgs args)
        {
            if (sender == null || !(sender is View)) return;
            var context = (sender as View).BindingContext as Height;

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
                App.BiometricHeightVM.RemoveEntry(context);
            }
        }

        string GraphDateTime_FormatLabel(object sender, object item)
        {
            if (!(item is Height)) return null;

            var date = (item as Height).CreationDate;
            return date.ToString("d/M/yy");
        }

        #region Application Bar Methods

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.HeightPageTitle;
        }

        #endregion

    }
}
