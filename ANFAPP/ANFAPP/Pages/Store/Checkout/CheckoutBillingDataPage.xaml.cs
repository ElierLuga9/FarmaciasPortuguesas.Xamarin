using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Authorization;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutBillingDataPage : ANFPage
    {

		#region Properties

		private CheckoutBillingInfoViewModel _viewModel = new CheckoutBillingInfoViewModel();

		#endregion

        #region Page Initialization

		public CheckoutBillingDataPage(CheckoutStartOut basket) : base() 
		{ 
			_viewModel.Basket = basket;
			BindingContext = _viewModel;

			/// The billing form will be filled with any pre-loaded data received via the CheckoutStart WS.
			_viewModel.UpdateBindableData();
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			LoadingView.IsVisible = false;

			_viewModel.RefreshBasket();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

		#endregion

		#region Event Handlers

		async void OnConfirmButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await _viewModel.UpdateBillingAddress();
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			mixpanelWidget.Track("CheckoutBillingConfirmed");

			Navigation.PushAsync(new CheckoutPaymentMethodPage(_viewModel.Basket));
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		#endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
			return AppResources.CheckoutPageTitle;
        }

        #endregion

    }
}
