using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Utils;
using ANFAPP.Views;

namespace ANFAPP.Pages.Store
{
    public partial class StoreSearchFilterPage : ANFPage
    {

		#region Properties

		private StoreSearchableViewModel _viewModel = new StoreSearchableViewModel();

		#endregion

        #region Page Initialization

		public StoreSearchFilterPage() : base() { }

		public StoreSearchFilterPage(StoreSearchableViewModel viewModel) : this()
		{
			BindingContext = _viewModel = viewModel;
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();			
        }

        #endregion

		#region Events Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

        //testes
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }


		async void OrderButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await NavigationUtils.PushPageWithNoHistory(new StoreSearchOrderPage(_viewModel), Navigation);
		}

		async void SearchButtonClicked(object sender, EventArgs args)
		{
			BindingContext = null;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PopAsync();
		}

		#endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
			return AppResources.StoreSearchFilterPageTitle;
        }
			
        #endregion
    }
}
