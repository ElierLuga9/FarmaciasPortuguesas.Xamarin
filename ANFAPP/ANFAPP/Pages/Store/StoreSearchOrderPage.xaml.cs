using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
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

namespace ANFAPP.Pages.Store
{
    public partial class StoreSearchOrderPage : ANFPage
    {

		#region Properties

		StoreSearchableViewModel _viewModel = new StoreSearchableViewModel();

		#endregion

        #region Page Initialization

		public StoreSearchOrderPage() : base() { }

		public StoreSearchOrderPage(StoreSearchableViewModel viewModel) : this()
		{
			BindingContext = _viewModel = viewModel;
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
		}


        //testes
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

		#endregion

		#region Event Handlers

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PopAsync();
		}

		async void FilterButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await NavigationUtils.PushPageWithNoHistory(new StoreSearchFilterPage(_viewModel), Navigation);
		}

		#endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
			return AppResources.StoreSearchOrderPageTitle;
        }

        #endregion
    }
}
