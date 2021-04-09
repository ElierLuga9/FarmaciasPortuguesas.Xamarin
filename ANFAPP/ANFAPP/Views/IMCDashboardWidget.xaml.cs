using ANFAPP.Logic;
using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Pages.BiometricData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class IMCDashboardWidget : ContentView
	{

        public delegate Task OnNavigationStartedEventHandler();

        #region Bindable Properties

        public OnNavigationStartedEventHandler OnNavigationStarted;
        public static readonly BindableProperty HasLinkProperty = BindableProperty.Create<IMCDashboardWidget, bool>(p => p.HasLink, false);
        public static readonly BindableProperty RoundingFlagProperty = BindableProperty.Create<IMCDashboardWidget, bool>(p => p.RoundingFlag, false);

        #endregion

        #region Bindable Objects

        public bool HasLink
        {
            get { return (bool)GetValue(HasLinkProperty); }
            set { SetValue(HasLinkProperty, value); }
        }

        public bool RoundingFlag
        {
            get { return (bool)GetValue(RoundingFlagProperty); }
            set 
            { 
                SetValue(RoundingFlagProperty, value); 
                BiometricGauge.RoundingFlag = this.RoundingFlag;
            }
        }

        #endregion

        public IMCDashboardWidget()
		{
			InitializeComponent ();

            // Initialize the context.
            InitContext();
		}

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (HasLink)
            {
                BiometricGauge.Title = AppResources.IMCPageTitle;
                BiometricGauge.RoundingFlag = this.RoundingFlag;
                IMCPageButton.Clicked += GaugeButton_Clicked;
            }
        }

        /// <summary>
        /// Initialize the Binding Context.
        /// </summary>
        protected void InitContext() 
        {
            var context = App.BiometricIMCVM;
            BindingContext = context;

            context.HeightVM = App.BiometricHeightVM;
            context.WeightVM = App.BiometricWeightVM;
        }

        #region Button Events

        async void HeightButton_Clicked(object sender, EventArgs args)
        {
            if (OnNavigationStarted != null) await OnNavigationStarted();

            // Go to Height Page
            await Navigation.PushAsync(new HeightPage());
        }

        async void WeightButton_Clicked(object sender, EventArgs args)
        {
            if (OnNavigationStarted != null) await OnNavigationStarted();

            // Go to Weight Page  
            await Navigation.PushAsync(new WeightPage());
        }

        async void GaugeButton_Clicked(object sender, EventArgs args) 
        {
            if (OnNavigationStarted != null) await OnNavigationStarted();

            // Go to IMC Page
            await Navigation.PushAsync(new IMCPage());
        }

        #endregion

    }
}

