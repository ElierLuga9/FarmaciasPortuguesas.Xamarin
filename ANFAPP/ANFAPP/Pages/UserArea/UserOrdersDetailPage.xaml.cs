using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages;
using ANFAPP.Utils;
using ANFAPP.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using XLabs.Platform;
using System.Diagnostics;
using ANFAPP.Logic.Resources;
using XLabs.Forms.Controls;
using System.Globalization;
using ANFAPP.Pages.Store;


namespace ANFAPP.Pages.UserArea
{
	public partial class UserOrdersDetailPage : ANFPage
	{

		private UserOrderDetailsViewModel _viewModel = new UserOrderDetailsViewModel();
		private bool _initialized = false;


		public UserOrdersDetailPage(DeliveryOut order) : base()
		{
			_viewModel.Detail = order;
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }



		#region Page Lifecylce

		protected override void OnAppearing()
		{
			base.OnAppearing();
			TabBar.OnNavigationStarted += OnLoadStart;

			BindingContext = _viewModel;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnAddToBasketSuccess += OnAddToBasketSuccess;

			_viewModel.LoadData(_viewModel.Detail.AnfId);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			TabBar.OnNavigationStarted -= OnLoadStart;

			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnAddToBasketSuccess -= OnAddToBasketSuccess;
		}

		#endregion

		#region Events

		public async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		public void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
			_initialized = true;
		}
			
		public async void OnAddToBasketSuccess(string message) 
		{
			var result = await DisplayAlert(null, message, "Ver Carrinho", "Fechar");

			if (result) {
				await Navigation.PushAsync(new StoreBasketPage());	
			} else {
				LoadingView.IsVisible = false;
			}
		}

		public async void OnLoadError(string title, string Message)
		{
			//Original error message from service is overriden in viewModel 
			await ShowErrorDialog(title, Message);

			if (!_initialized) await Navigation.PopAsync();
		}

		private async Task ShowErrorDialog(string title, string message)
		{
			await DisplayAlert(title, message, AppResources.OK);
		}

		async void OnAddToCartButtonClicked(object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.AddOrderToCart();
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override bool HasAppBarUserButton()
		{
			return false;
		}


        //testes
        protected override bool HasCardButton()
        {
            return true;
        }


		protected override string GetAppBarTitle()
		{
			return AppResources.UserAreaPageTitle;
		}

		#endregion

	}
}
