using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Pages.PharmacyLocator;
using System.Threading.Tasks;
using ANFAPP.Logic;

namespace ANFAPP.Views
{
    public partial class MyPharmacyWidget : ContentView
    {

		public delegate Task OnTaskStartedEventHandler();
		public event OnTaskStartedEventHandler OnWidgetClicked;
		
		private MyPharmacyWidgetViewModel _vm = new MyPharmacyWidgetViewModel();


		public MyPharmacyWidget () : base()
        {
            InitializeComponent ();
			BindingContext = _vm;
        }

		public void LoadData() {
			_vm.LoadData ();
		}

        #region Event handlers

		public void OnPharmacyChanged(object sender, EventArgs args)
		{
			_vm.LoadData ();
		}

        public async void OnCallButtonClicked (object sender, EventArgs args)
        {


			/*
			string phoneUrl = 
				(Device.OS == TargetPlatform.iOS ? "telprompt:" : "tel:")
				+ _vm.Pharmacy.Phone;
			*/

			string phoneUrl = "tel:" + _vm.Pharmacy.Phone;
			Device.OpenUri(new Uri(phoneUrl));
        }

        public async void OnMapButtonClicked (object sender, EventArgs args)
        {
			if (OnWidgetClicked != null) await OnWidgetClicked();

			await Navigation.PushAsync (new LocatorPharmacyDetails (_vm.Pharmacy));
        }

        public async void OnPharmacyButtonClicked (object sender, EventArgs args)
        {
			if (OnWidgetClicked != null) await OnWidgetClicked();

			await Navigation.PushAsync (new LocatorPharmacyDetails (_vm.Pharmacy));
        }

		public async void OnSelectPharmacyButtonClicked (object sender, EventArgs args)
		{
			if (OnWidgetClicked != null) await OnWidgetClicked();

			NavigationUtils.PushPageAndClearHistory (new DashboardLocator (), Navigation);
		}

        #endregion
    }
}