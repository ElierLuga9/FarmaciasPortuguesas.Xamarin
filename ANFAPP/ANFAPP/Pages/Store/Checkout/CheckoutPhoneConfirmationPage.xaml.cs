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
    public partial class CheckoutPhoneConfirmationPage : ANFPage
    {

		#region Properties

		CheckoutPhoneConfirmationViewModel _viewModel = new CheckoutPhoneConfirmationViewModel();
		CheckoutFinalStepViewModel _vm = new CheckoutFinalStepViewModel();
		#endregion

        #region Page Initialization

		public CheckoutPhoneConfirmationPage(CheckoutStartOut basket,CheckoutFinalStepViewModel vm) : base() 
		{
			_viewModel.Basket = basket; 
			BindingContext = _viewModel;
			_viewModel.UpdateBindings();
			_vm = vm;
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

			LoadingView.IsVisible = false;
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

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async void OnLoadSuccess()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			mixpanelWidget.Track("CheckoutPhoneConfirmed");
			await DisplayAlert("","Encomenda efectuada com sucesso", AppResources.OK);

		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		async void OnConfirmButtonClicked(object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			try
			{
				var checkOut = await _vm.CheckoutConf(xMBWAYPhone.Text, true);
				LoadingView.IsVisible = false;

				if (checkOut.Error == null || ((bool)!checkOut.Error))
				{
					await DisplayAlert("", "A sua encomenda foi concluída com sucesso", AppResources.OK);
					await NavigationUtils.PushPageAndClearHistory(new CheckoutFinalStepPage(_viewModel.Basket, _vm), Navigation);

				}
				else
				{
					await DisplayAlert("", checkOut.msg, AppResources.OK);
				}
			}
			catch(Exception ex) 
			{
				await DisplayAlert("", "Ocorreu um erro a processar o pagamento, por favor tente novamente", AppResources.OK);
			}
			//_viewModel.UpdatePhoneNumber();
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
