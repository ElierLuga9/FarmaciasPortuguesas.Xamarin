using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Pages.PharmacyLocator;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea
{
    public partial class UserAreaMainPage : ANFPage
    {

        #region Properties

        public UserAreaViewModel _viewModel { get; set; }

        #endregion

        #region Page Initialization

        public UserAreaMainPage() : base() { }

        protected override void InitPage()
        {
            _viewModel = new UserAreaViewModel();
            InitializeComponent();
            base.InitPage();
        }

        #endregion

        #region Events

        public void OnLoadSuccess()
        {
            //MainArea.IsVisible = true;
            LoadingView.IsVisible = false;
        }

        public async Task OnLoadStart()
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        public void OnLoadError(string title, string Message) 
        {
            ShowErrorDialog(title,Message);
        }

        private async void ShowErrorDialog(string title, string message)
        {
            await DisplayAlert(title, message, AppResources.OK);
			
			// Go back
			//await Navigation.PopAsync();
        }

		void RegisterCardButton_Click(object sender, EventArgs args)
		{
			// Go to card register page
			Navigation.PushAsync(new RegisterCardPage());
		}

		void AssociateCardButton_Click(object sender, EventArgs args)
		{
			// Go to card association page
			Navigation.PushAsync(new AssociateCardPage());
		}

        #endregion

        #region Tap Actions
        void OnLogoutButtonClick(object sender, EventArgs args)
        {
			OnForcedLogout(null, null);


			//LoadingView.IsVisible = true;
			//await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			//// Deactivate scheduler push's
			//try
			//{
			//	// Humm... how to handle a network failure here?
			//	if (!string.IsNullOrEmpty(SessionData.ParseInstallationId)) 
			//		await SchedulerWS.DeactivateToken(SessionData.PharmacyUser.Username, SessionData.ParseInstallationId);
			//}
			//catch (Exception ex)
			//{
			//	System.Diagnostics.Debug.WriteLine(ex.Message);
			//}

			//var mixpanelWidget = DependencyService.Get<IMixPanel>();
			//mixpanelWidget.Track("Logout");
			//mixpanelWidget.Identify(mixpanelWidget.DistinctId());

			//// Clear Schedules db
			//await (new DosageDAO()).DeleteAll();
			//await (new DosingScheduleDAO()).DeleteAll();
			//await (new MedicineDAO()).DeleteAll();

			//// Save the current pharmacy id.
			//var pharmacyId = SessionData.StorePharmacyId;
			//var pharmacyName = SessionData.StorePharmacyName;
			//var pharmacyPhone = SessionData.StorePharmacyPhone;
			//var pharmacyAddress = SessionData.StorePharmacyAddress;

			//// Clear the achievements.
			//App.GameCenterVM.Achievements = null;
			//App.GameCenterVM.TotalAchievements = 0;
			//App.GameCenterVM.TotalDoneAchievements = 0;

			//// Clear all user data
			//SessionData.ClearUser();

			//// Clear the vouchers.
			//var vdao = new VouchersDAO();
			//await vdao.DeleteAll ();

			//// Wipe the current Basket
			//if (ApplicationBar != null) ApplicationBar.SetCartItems(0);
			//App.StoreBasketVM.WipeBasketCache();

			//// Wipe the favorites.
			//App.FavoritesVM.WipeFavoriteCache();

			//// Clear the pharmacies (recents and favorites).
			//var pharmacyDAO = new PharmacyDAO();
			//await pharmacyDAO.DeleteAll ();

			//try {
			//	// And (re)set the pharmacy for the public section.
			//	var result = await ECommerceWS.SetMyFarm(SessionData.UserAuthentication, pharmacyId);

			//	if (result != null && result.code == 210)
			//	{
			//		SessionData.UpdatePharmacy(pharmacyId, pharmacyName, pharmacyPhone, pharmacyAddress, true);
			//	}

			//} catch (Exception ex) {
			//	// If this fails we just get back to the login with the default pharmacy.
			//	System.Diagnostics.Debug.WriteLine(ex.Message);
			//}

			//// Go back to login
			//NavigationUtils.PushPageAndClearHistory(new QuickRegisterPage(), Navigation);
			//// Show success message
			//await DisplayAlert(null, AppResources.UserAreaLogoutSuccessMessage, AppResources.OK);
        }

		async void OnChangePasswordButtonClick(object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new ChangePasswordPage());
        }
        async void OnChangePharmButtonClick(object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new DashboardLocator());
        }
        async void OnEditDataButtonClick(object sender, EventArgs args)
        {
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new EditUserDataPage());
        }

        #endregion

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

			_viewModel.OnSuccess += OnLoadSuccess;
			_viewModel.OnStart += OnLoadStart;
			_viewModel.OnError += OnLoadError;
            
			// Reload the data after the edition.
			//MainArea.IsVisible = false;
			LoadingView.IsVisible = true;
			BindingContext = _viewModel;
			_viewModel.LoadData ();
        }
			
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

			TabBar.OnNavigationStarted -= OnLoadStart;

			_viewModel.OnSuccess -= OnLoadSuccess;
			_viewModel.OnStart -= OnLoadStart;
			_viewModel.OnError -= OnLoadError;
        }

        #endregion

    }
}
