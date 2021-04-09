using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Utils;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Pages.Store;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Utils;
using ANFAPP.Helpers;

namespace ANFAPP.Pages
{
    public abstract class ANFPage : ContentPage
    {

		#region Constants

		private readonly int DEFAULT_TABLET_MENU_WIDTH = 264;

		#endregion

		#region Properties

		private View LoadingView;
		private MenuView MenuView;
		private ApplicationBar ApplicationBar;
	
		private bool _isInitialized = false;

		#endregion

        public ANFPage()
        {
			// Initialize the page components.
			InitPage();

            // Disable Default Action Bar
			NavigationPage.SetHasNavigationBar(this, false);

            // Set the color for this page
            BackgroundColor = ColorResources.APPBackgroundLight;
        }

        /// <summary>
        /// Initializes the page.
        /// </summary>
        protected virtual void InitPage()
        {
            // Initializes the application bar

            InitApplicationBar();

			// 
			InitMenu();

			if (Content == null) return;

			// Initialize loading
			try 
			{ 
				LoadingView = Content.FindByName<View>("LoadingView");
			}
			catch { }
        }

        /// <summary>
        /// Initializes the controls on the application bar.
        /// </summary>s
        protected void InitApplicationBar()
        {
            if (Content == null) return;

            // Get Application Bar
			ApplicationBar = Content.FindByName<ApplicationBar>("ApplicationBar");
			if (ApplicationBar == null) return;

            // Left Button
			ApplicationBar.SetLeftButtonType(GetAppBarLeftButtonType());

            // Set the title
			ApplicationBar.SetTitle(GetAppBarTitle());

			// Set the title
			ApplicationBar.SetSubtitle(GetAppBarSubtitle());

            // Sets the application bar color
			ApplicationBar.SetApplicationBarColor(GetAppBarColor());

            // If user is logged, show points
			if (SessionData.IsLogged) ApplicationBar.SetUserPoints(SessionData.PharmacyUser.Points);

            // Hide user button if not enabled
			//if (!HasAppBarUserButton()) ApplicationBar.HideUserButton();
            if (!HasCardButton()) ApplicationBar.HideUserButton();
            
            //Hide menu button on tablet device
            if (Device.Idiom == TargetIdiom.Tablet && HasMenuInTablet()) ApplicationBar.HideMenuButton();
        }

		/// <summary>
		/// Initializes the Application Menu (Tablet Only!)
		/// </summary>
		protected void InitMenu()
		{
			MenuView = Content.FindByName<MenuView>("MenuView");
			if (MenuView == null) return;

			// Render the menu if the device is a tablet & the page has a menu
			if (Device.Idiom == TargetIdiom.Tablet && HasMenuInTablet()) 
			{
				MenuView.RenderView();
				MenuView.WidthRequest = DEFAULT_TABLET_MENU_WIDTH;
			}
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing ();

            var cid = ANFAPP.Logic.Settings.AppSettings.GetValueOrDefault(ANFAPP.Logic.Settings.ST_GA_CID, ANFAPP.Logic.Settings.ST_GA_CID_DEFAULT);
			if (null != cid) {
				var title = this.Title ?? this.GetType ().Name;
				await App.Analytics.TrackScreen (cid, title);
			}

			// Register Menu loading events
			if (MenuView != null)
			{
                //testes
                //MenuView.setPromototal();

				MenuView.OnLoadStart += ShowLoading;
				MenuView.OnLoadEnd += HideLoading;
                
			}

			// Navigation events
			if (ApplicationBar == null) return;
			ApplicationBar.NavigationStarted += OnNavigationStarted;

			// Pharmacy/Basket Events
			SessionData.OnPharmacyChanged += OnPharmacyChanged;
			SessionData.OnCatastrophicSessionLost += OnForcedLogout;
			App.StoreBasketVM.OnLoadSuccess += OnStoreBasketLoadComplete;
			App.FavoritesVM.OnLoadSuccess += OnFavoritesLoadComplete;

			//if (!_isInitialized) 
			//{ 
				// Show cart button
				if (HasBasketButton())
				{
					// Show cart and update the counter
					ApplicationBar.ShowCartButton();
					if (App.StoreBasketVM.Basket != null) {
						ApplicationBar.SetCartItems(App.StoreBasketVM.Basket.TotalQuantityInCart);
					}
				}

				if (HasFavoritesButton()) 
				{
					ApplicationBar.ShowFavButton();
					ApplicationBar.SetFavItems (App.FavoritesVM.ProductsCount);
				}
                if (HasCardButton())
                {
                    ApplicationBar.ShowCardButton();
                    if (SessionData.IsLogged){

                        ApplicationBar.SetCardItems(SessionData.PharmacyUser.Points);
                    }
                }

				_isInitialized = true;
			//}

			//// We need to update the items when navigating back.
			//if (HasBasketButton() && App.StoreBasketVM.Basket != null)
			//{
			//	ApplicationBar.SetCartItems(App.StoreBasketVM.Basket.TotalQuantityInCart);
			//}
			//if (HasFavoritesButton ()) 
			//{
			//	ApplicationBar.SetFavItems (App.FavoritesVM.ProductsCount);
			//}
				
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			// Unregister Menu loading events
			if (MenuView != null) 
			{ 
				MenuView.OnLoadStart -= ShowLoading;
				MenuView.OnLoadEnd -= HideLoading;
			}

			// Get Application Bar
			if (ApplicationBar == null) return;

			// Navigation events
			ApplicationBar.NavigationStarted -= OnNavigationStarted;

			// Pharmacy/Basket Events
			SessionData.OnPharmacyChanged -= OnPharmacyChanged;
			SessionData.OnCatastrophicSessionLost -= OnForcedLogout;
			App.StoreBasketVM.OnLoadSuccess -= OnStoreBasketLoadComplete;
			App.FavoritesVM.OnLoadSuccess -= OnFavoritesLoadComplete;
		} 

        #region ApplicationBar Modifiers

        /// <summary>
        /// Returns the type of the AppBar's left button.
        /// </summary>
        /// <returns></returns>
        protected virtual ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.NONE;
        }

        /// <summary>
        /// Returns if the AppBar has the User button on the top right corner.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasAppBarUserButton()
        {
            return true;
        }

        /// <summary>
        /// Returns if the AppBar has the CardButton
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasCardButton()
        {
            return false;
        }

		/// <summary>
		/// Returns if the AppBar has the BasketButton
		/// </summary>
		/// <returns></returns>
		protected virtual bool HasBasketButton()
		{
			return false;
		}

		/// <summary>
		/// Determines whether the application bar shows the Favorites button.
		/// </summary>
		/// <returns><c>true</c> if the application bar should display the Favorites button; otherwise, <c>false</c>.</returns>
		protected virtual bool HasFavoritesButton()
		{
			return false;
		}

        /// <summary>
        /// Returns the page title.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAppBarTitle()
        {
            return Title;
        }

		/// <summary>
		/// Returns the page subtitle.
		/// </summary>
		/// <returns></returns>
		protected virtual string GetAppBarSubtitle()
		{
			return null;
		}

        /// <summary>
        /// Returns the color of the application bar.
        /// </summary>
        /// <returns></returns>
        protected virtual Color GetAppBarColor()
        {
            return ColorResources.ANFGreen;
        }

		/// <summary>
		/// Returns if this page should have a menu in tablet mode.
		/// </summary>
		/// <returns></returns>
		protected virtual bool HasMenuInTablet()
		{
			return true;
		}

        #endregion

		#region Pharmacy/Basket Handlers

		public async void OnForcedLogout(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
            await Task.Delay(ANFAPP.Logic.Settings.DEFAULT_LOADING_DELAY);

			// Deactivate scheduler push's
			try
			{
				// Humm... how to handle a network failure here?
				if (!string.IsNullOrEmpty(SessionData.ParseInstallationId))
					await SchedulerWS.DeactivateToken(SessionData.PharmacyUser.Username, SessionData.ParseInstallationId);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			mixpanelWidget.Track("Logout");
//			mixpanelWidget.Identify(mixpanelWidget.DistinctId());

			// Clear Schedules db
			await(new DosageDAO()).DeleteAll();
			await(new DosingScheduleDAO()).DeleteAll();
			await(new MedicineDAO()).DeleteAll();

			// Save the current pharmacy id.
			var pharmacyId = SessionData.StorePharmacyId;
			var pharmacyName = SessionData.StorePharmacyName;
			var pharmacyPhone = SessionData.StorePharmacyPhone;
			var pharmacyAddress = SessionData.StorePharmacyAddress;

			// Clear the achievements.
			App.GameCenterVM.Achievements = null;
			App.GameCenterVM.TotalAchievements = 0;
			App.GameCenterVM.TotalDoneAchievements = 0;

			// Clear all user data
			SessionData.ClearUser();

			mixpanelWidget.Reset();

			// Clear the vouchers.
			var vdao = new VouchersDAO();
			await vdao.DeleteAll();

			// Wipe the current Basket
			if (ApplicationBar != null) ApplicationBar.SetCartItems(0);
			App.StoreBasketVM.WipeBasketCache();

			// Wipe the favorites.
			App.FavoritesVM.WipeFavoriteCache();

			// Clear the pharmacies (recents and favorites).
			var pharmacyDAO = new PharmacyDAO();
			await pharmacyDAO.DeleteAll();

			try
			{
				// And (re)set the pharmacy for the public section.
				var result = await ECommerceWS.SetMyFarm(SessionData.UserAuthentication, pharmacyId);

				if (result != null && result.code == 210)
				{
					SessionData.UpdatePharmacy(pharmacyId, pharmacyName, pharmacyPhone, pharmacyAddress, true);
				}

			}
			catch (Exception ex)
			{
				// If this fails we just get back to the login with the default pharmacy.
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			// Go back to login
			await Navigation.PushAsync(new QuickRegisterPage());

			var message = AppResources.UserAreaLogoutSuccessMessage;
			if (args != null && args is OnLogoutEventArgs)
			{
				message = (args as OnLogoutEventArgs).Message;
			}

			// Show success message
			await DisplayAlert(null, message, AppResources.OK);
		}

		void OnPharmacyChanged(object sender, EventArgs args)
		{
			// http://issue.innovagency.com/view.php?id=20569
			// PharmacyChangedEventArgs pargs = args as PharmacyChangedEventArgs;
			// if (!string.Equals (pargs.OldPharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT)
			// 	&& !string.Equals(pargs.NewPharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT)) 
			// { 
			// 	DisplayAlert (AppResources.YourPharmacyChangedDialogTitle, AppResources.YourPharmacyChangedDialogMessage, AppResources.OK);
			// }
			if (!(this is HomePage) && !(this is LoginPage) && !(this is CardBenefitsPage)) {
				DisplayAlert (AppResources.YourPharmacyChangedDialogTitle, AppResources.YourPharmacyChangedDialogMessage, AppResources.OK);
			}

			// Wipe the current Basket
			if (ApplicationBar != null) ApplicationBar.SetCartItems(0);
			App.StoreBasketVM.WipeBasketCache();

			// Load the new pharmacy's basket if one is selected
			if (SessionData.IsAuthenticatedWithPharmacy) {

				if (this is StoreBasketPage) {
					App.StoreBasketVM.LoadBasket();
				} else {
					App.StoreBasketVM.PeekBasket();
				}
			} 

			// Update the pharmacy name on the application bar.
			ApplicationBar.SetSubtitle (GetAppBarSubtitle ());
		}

		void OnStoreBasketLoadComplete()
		{
			if (HasBasketButton() && ApplicationBar != null)
			{
				// Show cart and update the counter.
				ApplicationBar.SetCartItems(App.StoreBasketVM.Basket.TotalQuantityInCart);
			}
		}

		void OnFavoritesLoadComplete()
		{
			if (HasFavoritesButton() && ApplicationBar != null)
			{
				// Show star and update the counter
				ApplicationBar.SetFavItems(App.FavoritesVM.ProductsCount);
			}
		}

        void OnCardLoadComplete()
        {
            if (HasCardButton() && ApplicationBar != null)
            {
                //Show star and update the counter
                if (SessionData.IsLogged)
                {
                    ApplicationBar.SetFavItems(SessionData.PharmacyUser.Points);
                }
            }
        }

		#endregion

		#region Loadings

		async Task OnNavigationStarted()
		{
			await ShowLoading();
		}

		public async Task ShowLoading()
		{
			if (LoadingView == null) return;

			LoadingView.IsVisible = true;
            await Task.Delay(ANFAPP.Logic.Settings.DEFAULT_LOADING_DELAY);
		}

		public void HideLoading()
		{
			if (LoadingView != null) LoadingView.IsVisible = false;
		}

		#endregion

    }
}
