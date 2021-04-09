using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea
{
	public partial class UserOrdersPage : ANFPage
	{

		#region Properties

		private UserOrdersViewModel _viewModel { get; set; }
		private bool _isInitialized = false;

		#endregion

		#region Page Initialization

		public UserOrdersPage() : base() { }

		protected override void InitPage()
		{
			_viewModel = new UserOrdersViewModel();

			InitializeComponent();
			base.InitPage();
		}

		#endregion

		#region Events

		public void OnLoadSuccess()
		{
			BindingContext = _viewModel;
			LoadingView.IsVisible = false;
		}


		public void OnLoadError(string title, string Message)
		{
			//Original error message from service is overriden in viewModel 
			ShowErrorDialog(title, Message);
		}

		private async void ShowErrorDialog(string title, string message)
		{
			await DisplayAlert(title, message, AppResources.OK);
		}

		public async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		#endregion

		public async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null)
				return;

			// Start loading
			await OnLoadStart();

			// Open detail page
            var item = args.SelectedItem as DeliveryOut;
			await Navigation.PushAsync(new UserOrdersDetailPage(item));
			((ListView)sender).SelectedItem = null;
		}

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

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

		#region Page Lifecylce

		protected override void OnAppearing()
		{
			base.OnAppearing();

			TabBar.OnNavigationStarted += OnLoadStart;

			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnLoadError += OnLoadError;

			if (!_isInitialized) 
			{ 
				_isInitialized = true;
				LoadingView.IsVisible = true;
				_viewModel.LoadData();
			}
			else
			{
				LoadingView.IsVisible = false;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			TabBar.OnNavigationStarted -= OnLoadStart;

			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnLoadError -= OnLoadError;

		}
		#endregion

	}
}
