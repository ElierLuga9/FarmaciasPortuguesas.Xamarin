using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.BiometricData
{
    public partial class DashboardPage : ANFPage
    {

        #region Properties

        private bool _isInitialized = false;
		EventHandler ArterialButton;
        #endregion

        #region Page Initialization

        public DashboardPage() : base() { }

        /// <summary>
        /// Initializes the page
        /// </summary>
        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

//            var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//            var props = new Dictionary<string,string> ();
//            props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
//            mixpanelWidget.TrackProperties ("BiometricData", props);
	     }

        protected override void OnAppearing()
        {
            base.OnAppearing();
			ArterialButton += ArterialPressure_Clicked;
			App.BiometricDashboardVM.OnLoadComplete += OnLoadComplete;
			App.BiometricDashboardVM.OnLoadStart += OnLoadStart;

            if (!_isInitialized)
            {
                InitContext();
                InitButtonEvents();

				App.BiometricDashboardVM.IMCVM = App.BiometricIMCVM;
				App.BiometricDashboardVM.BioSync();
                _isInitialized = true;
			}
			else
			{
				LoadingView.IsVisible = false;
			}

            IMCWidget.OnNavigationStarted += OnIMCNavigationStarted;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            IMCWidget.OnNavigationStarted -= OnIMCNavigationStarted;
			App.BiometricDashboardVM.OnLoadComplete -= OnLoadComplete;
			App.BiometricDashboardVM.OnLoadStart -= OnLoadStart;
			LoadingView.IsVisible = false;
        }

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
		}

		void OnLoadComplete()
		{
			LoadingView.IsVisible = false;
		}

        #endregion

        #region Initializers

        /// <summary>
        /// Initialize the button event listeners.
        /// </summary>
        protected void InitButtonEvents()
        {
            // Initialize button Listeners
            ArterialPressureButton.ButtonClicked = ArterialPressure_Clicked;
            CholesterolButton.ButtonClicked = Cholesterol_Clicked;
            GlicemiaButton.ButtonClicked = Glicemia_Clicked;
            AbdominalPerimeterButton.ButtonClicked = AbdominalPerimeter_Clicked;
            TriglyceridesButton.ButtonClicked = Triglycerides_Clicked;
        }

        protected void InitContext()
        {
            var context = App.BiometricDashboardVM;
            BindingContext = context;

			context.WeightVM = App.BiometricWeightVM;
			context.HeightVM = App.BiometricHeightVM;
            context.ArterialPressureVM = App.BiometricArterialPressureVM;
            context.AbdominalPerimeterVM = App.BiometricAbdominalPerimeterVM;
            context.CholesterolVM = App.BiometricCholesterolVM;
            context.GlicemiaVM = App.BiometricGlicemiaVM;
            context.TriglyceridesVM = App.BiometricTriglyceridesVM;
        }

        #endregion

        async Task OnIMCNavigationStarted()
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        #region Application Bar Methods

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
            return AppResources.BiometricDashboardTitle;
        }

        #endregion

        #region Dashboard Button Click Events

        async void ArterialPressure_Clicked(object sender, EventArgs args)
        {
			System.Diagnostics.Debug.WriteLine("Clicked");
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Arterial Pressure Page
            await Navigation.PushAsync(new ArterialPressurePage());
        }

        async void Cholesterol_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Cholesterol Page
            await Navigation.PushAsync(new CholesterolPage());
        }

        async void Glicemia_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Clicemia Page
            await Navigation.PushAsync(new GlicemiaPage());
        }

        async void AbdominalPerimeter_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Abdominal Perimeter Page
            await Navigation.PushAsync(new AbdominalPerimeterPage());
        }

        async void Triglycerides_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to Triglycerides Page
            await Navigation.PushAsync(new TriglyceridesPage());
        }   
            
        #endregion

    }
}
