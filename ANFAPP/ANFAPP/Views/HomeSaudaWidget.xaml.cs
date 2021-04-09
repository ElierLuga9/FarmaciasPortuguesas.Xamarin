using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.UserLogin;
using System.Threading.Tasks;
using ANFAPP.Logic;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace ANFAPP.Views
{
    public partial class HomeSaudaWidget : ContentView
    {

		public delegate Task OnTaskStartedEventHandler();
		public event OnTaskStartedEventHandler OnWidgetClicked;
		
		private SaudaWidgetViewModel _vm = new SaudaWidgetViewModel();


        public HomeSaudaWidget ()
        {
            InitializeComponent ();
			BindingContext = _vm;
        }

		public void LoadData() 
		{
			_vm.LoadData();
		}

		public void ReloadData()
		{
			_vm.LoadData();
		}

		public async void OnWidgetButtonClicked (object sender, EventArgs args)
		{
			if (_vm.HasSAFEBanner) {

				string phoneUrl = "tel:800241400";
				Device.OpenUri(new Uri(phoneUrl));

				/*
				var device = Resolver.Resolve<IDevice>();
				if (device.PhoneService != null)
				{
				}
				*/
			} else if (_vm.HasCard) {
				if (OnWidgetClicked != null) await OnWidgetClicked();

				//NavigationUtils.PushPageAndClearHistory (new UserCardPage(), Navigation);
                await Navigation.PushAsync(new UserCardPage());
			} else {
				if (OnWidgetClicked != null) await OnWidgetClicked();

				if (SessionData.IsAuthenticated) {
					Navigation.PushAsync (new RegisterCardPage ());
				} else {
					//NavigationUtils.PushPageAndClearHistory (new QuickRegisterPage(), Navigation);
                    await Navigation.PushAsync(new QuickRegisterPage());
				}
			}
		}
    }
}