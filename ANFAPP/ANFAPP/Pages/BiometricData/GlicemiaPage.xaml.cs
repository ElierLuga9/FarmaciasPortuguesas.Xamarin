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
using XLabs.Forms.Controls;
using Infragistics.XF;

namespace ANFAPP.Pages.BiometricData
{
    public partial class GlicemiaPage : ANFPage
    {

        #region Page Initialization

        public GlicemiaPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize the View Model
            BindingContext = App.BiometricGlicemiaVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
			lineseries1.Brush = new SolidColorBrush(Color.FromHex("#76bd22"));
			lineseries2.Brush = new SolidColorBrush(Color.FromHex("#4ab4e6"));
            LoadingView.IsVisible = false;
            App.BiometricGlicemiaVM.OnLoadComplete += OnLoadComplete;

			// Reset the default time to the current time (it is the time presented to the user so this should be the local time).
			App.BiometricGlicemiaVM.TimeInput = DateTime.Now.TimeOfDay;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.BiometricGlicemiaVM.OnLoadComplete -= OnLoadComplete;
        }

        #endregion

        void OnLoadComplete()
        {
            LoadingView.IsVisible = false;
        }

        void OnSwitch_Clicked(object sender, EventArgs args)
        {
            App.BiometricGlicemiaVM.IsGraphToggled = !App.BiometricGlicemiaVM.IsGraphToggled;
        }

        async void SubmitButton_Clicked(object sender, EventArgs args)
        {
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
			props.Add("Value", App.BiometricGlicemiaVM.ValueInput + string.Empty);
			props.Add("IsUnfed", App.BiometricGlicemiaVM.Unfed + string.Empty);
			mixpanelWidget.TrackProperties("GlicemiaData_Added", props);

            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            App.BiometricGlicemiaVM.InsertNewEntry();
        }

        async void RemoveEntry_Click(object sender, EventArgs args)
        {
            if (sender == null || !(sender is View)) return;
            var context = (sender as View).BindingContext as Glicemia;

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
                App.BiometricGlicemiaVM.RemoveEntry(context);
            }
        }

        string GraphDateTime_FormatLabel(object sender, object item)
        {
            if (!(item is Glicemia)) return null;

            var date = (item as Glicemia).CreationDate;
            return date.ToString("d/M/yy");
        }

        void UnfedCheckbox_Clicked(object sender, EventArgs args)
        {
            App.BiometricGlicemiaVM.Unfed = !App.BiometricGlicemiaVM.Unfed;
        }

        #region Application Bar Methods

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.GlicemiaTitle;
        }

        #endregion

    }
}
