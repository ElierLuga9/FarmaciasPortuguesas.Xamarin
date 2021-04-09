using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserArea.FamilyCard;
using ANFAPP.Pages.UserArea.Vouchers;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Pages.UserArea
{
    public partial class UserCardPage : ANFPage
    {

        #region Properties

        public UserCardViewModel _viewModel;

        #endregion

        #region Page Initialization

        public UserCardPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize binding context and load data
            this.BindingContext = _viewModel = new UserCardViewModel();

//			var mixpanelWidget = DependencyService.Get<IMixPanel>();
//			var props = new Dictionary<string, string>();
//			props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
//			mixpanelWidget.TrackProperties("UserCard", props);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadingView.IsVisible = false;

            _viewModel.OnSuccess += OnSuccessEventHandler;
            _viewModel.LoadData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnSuccess -= OnSuccessEventHandler;
        }

        #endregion

        #region Buttons

        public async void OnPointsHistoryButton_Clicked(object sender, EventArgs args)
        {
			
		
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new PointsHistoryPage());
        }

		public async void OnAquireVoucherButton_Clicked(object sender, EventArgs args)
		{


			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new AquireVoucherListPage());
		}

        public async void OnVoucherButton_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            // Go to vouchers page
            await Navigation.PushAsync(new VouchersPage());
        }

        public async void OnFamilyAccountButton_Clicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            if (SessionData.PharmacyUser.IsFamilyCard || _viewModel.HasFamilyWarning)
            {
                // Family card
                await Navigation.PushAsync(new FamilyAccountMasterPage());
            }
            else
            {
                // Not part of a family
                await Navigation.PushAsync(new NoFamilyAccountPage());
            }
        }

		public async void OnPartnershipButton_Clicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(300);

			//Device.OpenUri(new Uri(Settings.PARTNERSHIP_ENDPOINT));
			await Navigation.PushAsync(new WebViewPage(Settings.PARTNERSHIP_ENDPOINT, AppResources.UserPagePartnershipsButton));
		}

        public async void CardTap(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            await Navigation.PushAsync(new BarcodePage(_viewModel.CardNumber, null, _viewModel.Name));
        }

        #endregion

        #region ViewModel Event Handlers

        public void OnSuccessEventHandler()
        {
            // Refresh appbar points
			if (SessionData.IsLogged) {
				ApplicationBar.SetUserPoints(SessionData.PharmacyUser.Points);

				var mixpanelWidget = DependencyService.Get<IMixPanel> ();
				var props = new Dictionary<string,string> ();
                // props.Add("Email", SessionData.PharmacyUser.Username);
                props.Add("$email", SessionData.PharmacyUser.Username);
				// props.Add("Name", SessionData.PharmacyUser.Name);
                props.Add ("$name", SessionData.PharmacyUser.Name); 
			    props.Add("CardNumber", SessionData.PharmacyUser.CardNumber);
				mixpanelWidget.PeopleSet(props);
					
				mixpanelWidget.PeopleIncrement("Points", SessionData.PharmacyUser.Points);
//				mixpanelWidget.Identify(SessionData.PharmacyUser.Username);
			}
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

        protected override string GetAppBarTitle()
        {
            return AppResources.UserCardPageTitle;
        }


        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion
    }
}
