//#define WPHONE
#undef WPHONE

using System;
using ANFAPP.Helpers;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using System.Globalization;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Pages.DosageScheduler;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.Store;
using Plugin.Connectivity;
using ANFAPP.Pages.Store.Checkout;
using ANFAPP.Pages.GetPoints;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Pages.Articles;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Models.Out.Ecommerce;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Pages.Tutorial;
#if !WPHONE
using BranchXamarinSDK;
#endif
using System.Collections.Generic;

namespace ANFAPP
{
	public enum AppArea
	{
		Undefined = 0,
		ProductDetail = 1,
		CategoryList,
		ArticleDetail,
		BrandList,
		Scheduler,
		BiometricData,
		Contents,
		Points,
		Alert,
		MedAlert
	}

#if WPHONE
	public class  App : Application
#else
	public class  App : Application, IBranchSessionInterface
#endif
	{
        // The decimal separator obtained with platform specific code.
        //
        // If the iPhone Language is set to "English" and the Region is set to "Portugal" (the numeric keyboard shows ','):
        //
        //> System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator
        //"."
        //> System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        //"."
        public static string DecimalSeparator;

		#region ViewModels

        //testes Load Promotions
        public static PromotionsViewModel PromotionsVM = new PromotionsViewModel();
       

		// User Login View Models
		public static RegisterCardViewModel RegisterCardVM = new RegisterCardViewModel ();
		public static AssociateCardViewModel AssociateCardVM = new AssociateCardViewModel ();
		public static RecoverPasswordViewModel RecoverPasswordVM = new RecoverPasswordViewModel ();

		// Biometric Data
		public static UsersViewModel UsersViewModel = new UsersViewModel ();

		public static BiometricIMCViewModel BiometricIMCVM = new BiometricIMCViewModel ();
		public static BiometricArterialPressureViewModel BiometricArterialPressureVM = new BiometricArterialPressureViewModel ();
		public static BiometricAbdominalPerimeterViewModel BiometricAbdominalPerimeterVM = new BiometricAbdominalPerimeterViewModel ();
		public static BiometricTriglyceridesViewModel BiometricTriglyceridesVM = new BiometricTriglyceridesViewModel ();
		public static BiometricCholesterolViewModel BiometricCholesterolVM = new BiometricCholesterolViewModel ();
		public static BiometricGlicemiaViewModel BiometricGlicemiaVM = new BiometricGlicemiaViewModel ();
        
		public static BiometricWeightViewModel BiometricWeightVM = new BiometricWeightViewModel ();
		public static BiometricHeightViewModel BiometricHeightVM = new BiometricHeightViewModel ();
		
		public static BiometricDashboardViewModel BiometricDashboardVM = new BiometricDashboardViewModel ();

		public static GetPointsViewModel GameCenterVM = new GetPointsViewModel();

		public static DosingScheduleListViewModel DosingScheduleVM = new DosingScheduleListViewModel();
        public static ListDrugsViewModel ListDrugsVM = new ListDrugsViewModel();

		public static StoreBasketViewModel StoreBasketVM = new StoreBasketViewModel();
		public static FavoritesViewModel FavoritesVM = new FavoritesViewModel();

		#endregion

		public static AnalyticsUtils Analytics;

        public App (bool facebookRedirect = false) 
		{
			// Load Pharmacy User
			SessionData.LoadUser ();

			// Data Load
			LazyLoad.LoadDBData ();
            //testes
            //LazyLoad.PTotal = PromotionsVM.PromoList.Count;
            

			// Setup Google Analytics
			Analytics = new AnalyticsUtils(ANFAPP.Logic.Settings.GOOGLE_ANALYTICS_KEY, "Farm??cias Portuguesas", "3.0.5", "pt.anf.farmaciasportuguesas", "pt.anf.farmaciasportuguesas");

			// Setup MixPanel.
			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
			mixpanelWidget.SharedInstanceWithToken(ANFAPP.Logic.Settings.MIXPANEL_KEY);			
			
			if (string.IsNullOrEmpty(ANFAPP.Logic.Settings.AppSettings.GetValueOrDefault (ANFAPP.Logic.Settings.ST_GA_CID, ANFAPP.Logic.Settings.ST_GA_CID_DEFAULT))) {
				Guid guid = Guid.NewGuid ();
				ANFAPP.Logic.Settings.AppSettings.AddOrUpdateValue (ANFAPP.Logic.Settings.ST_GA_CID, guid.ToString ());
			}

            // If first run, show the intro page
            if (SessionData.IsFirstRun)
            {
				mixpanelWidget.Track("Install");
                MainPage = new NavigationPage(new AppIntroPage());
                return;
            }

			// First Page
			if (!SessionData.IsLogged)
			{
				// User not logged
				if (facebookRedirect) 
				{
					MainPage = new NavigationPage(new QuickRegisterPage(QuickRegisterPage.LoginAction.FACEBOOK_REDIRECT));
				}
				else
				{
					MainPage = new NavigationPage(new QuickRegisterPage());
				}
			}
			else 
			{
				if (SessionData.PharmacyUser != null && !SessionData.IsAuthenticated)
				{
					// Should only happen when upgrading from V1 to V2 while logged in.
					MainPage = new NavigationPage(new QuickRegisterPage(QuickRegisterPage.LoginAction.REFRESH_LOGIN));
				} 
				else if (!CrossConnectivity.Current.IsConnected)
				{
					// No Connection - go to menu
					MainPage = new NavigationPage(new MenuPage());
				}
				else
				{
					//if (SessionData.HasPharmacyCard)
					//{
						// Gamification Achievement! - TYPE_5 - App Launch
						// GameCenterWS.PostEventAsync(ANFAPP.Logic.Settings.GAMECENTER_EVENT_APP_LAUNCH);

					// Gamification - Upload Fitness Data (if the gamification area is visible)
					if (ANFAPP.Logic.Settings.IS_GAMIFICATION_MENU_ACTIVE && SessionData.HasPharmacyCard && SessionData.HasFitnessServicesEnabled) App.GameCenterVM.LoadData(true);
													
					// User logged and has a pharmacy card
					MainPage = new NavigationPage(new HomePage());

					//}
					//else
					//{
					//	// User logged and has a pharmacy card
					//	MainPage = new NavigationPage(new UserNotLoggedCardPage());
					//}
				}
			}
		}

		protected override void OnStart ()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
			mixpanelWidget.Identify (mixpanelWidget.DistinctId());
			mixpanelWidget.Track ("SessionStart");

		}

		protected override void OnSleep ()
		{
            SessionData.FlushReports ();

//			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//			mixpanelWidget.Track ("SessionStop");
		}

		protected override void OnResume ()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel> ();
//			mixpanelWidget.Identify (mixpanelWidget.DistinctId());
			mixpanelWidget.Track ("SessionStart");
#if !WPHONE
			Branch branch = Branch.GetInstance ();
			branch.InitSessionAsync (this);
#endif
		}

		public async Task OpenPage(AppArea area, string context = null) {
			System.Diagnostics.Debug.WriteLine ("Open Area: " + area);

			// Push the new area. The history is cleared if the destination page application bar action presents the menu.
			switch (area) {
				case AppArea.Alert:
					if (SessionData.IsLogged) 
					{
						NavigationUtils.PushPageAndClearIfNotActive (new DosingSchedulePage(context), MainPage.Navigation);
					} 
					else 
					{
						//NavigationUtils.PushPageAndClearIfNotActive (new LoginPage (), MainPage.Navigation);
						await MainPage.Navigation.PushAsync(new LoginPage());
					}
					break;
				case AppArea.MedAlert:
					if (SessionData.IsLogged)
					{
						NavigationUtils.PushPageAndClearIfNotActive(new DosingSchedulePage(context, false), MainPage.Navigation);
					}
					else
					{
						//NavigationUtils.PushPageAndClearIfNotActive(new LoginPage(), MainPage.Navigation);
						await MainPage.Navigation.PushAsync(new LoginPage());
					}
					break;
				case AppArea.ArticleDetail:
					await MainPage.Navigation.PushAsync (new ArticlesListDetailPage(Convert.ToInt32 (context)));
					break;
				case AppArea.BiometricData:
					if (SessionData.IsLogged) 
					{
						NavigationUtils.PushPageAndClearIfNotActive (new DashboardPage (), MainPage.Navigation);
					} 
					else 
					{
						//NavigationUtils.PushPageAndClearIfNotActive (new LoginPage (), MainPage.Navigation);
						await MainPage.Navigation.PushAsync(new LoginPage());
					}
					break;
				case AppArea.BrandList:
					await MainPage.Navigation.PushAsync (new StoreSearchPage (new StoreBrandSearchViewModel(Convert.ToInt32 (context))));
					break;
				case AppArea.CategoryList:
					/* TODO: push with cat name*/
					await MainPage.Navigation.PushAsync(new StoreSearchPage(new CategoryStoreSearchViewModel(Convert.ToInt32 (context), ""), StoreNavigationWidget.SelectedTabEnum.Categories));
					break;
				case AppArea.Contents:
					NavigationUtils.PushPageAndClearIfNotActive (new ArticlesMainPage (), MainPage.Navigation);
					break;
				case AppArea.Points:
					NavigationUtils.PushPageAndClearIfNotActive (new GetPointsMain(), MainPage.Navigation);
					break;
				case AppArea.ProductDetail:
					await MainPage.Navigation.PushAsync (new StoreProductDetailPage (Convert.ToInt32 (context)));
					break;
				case AppArea.Scheduler:
					if (SessionData.IsLogged)
					{
						NavigationUtils.PushPageAndClearIfNotActive (new DosingSchedulePage(), MainPage.Navigation);
					} 
					else 
					{
						//NavigationUtils.PushPageAndClearIfNotActive (new LoginPage (), MainPage.Navigation);
						await MainPage.Navigation.PushAsync(new LoginPage());
					}
					break;
			}
		}

		/// <summary>
		/// Open area via banner link.
		/// </summary>
		/// <param name="area"></param>
		/// <param name="context"></param>
		public async void OpenPage(BannerDestination area, string context) 
		{
			System.Diagnostics.Debug.WriteLine ("Open Area (Banner): " + area);

			// Push the new area. The history is cleared if the destination page application bar action presents the menu.
			switch (area) 
			{
				case BannerDestination.ProductDetail:
					await OpenPage(AppArea.ProductDetail, context);
					break;
				case BannerDestination.ProductsInCategory:
					await OpenPage(AppArea.CategoryList, context);
					break;
				case BannerDestination.ArticleDetail:
					await OpenPage(AppArea.ArticleDetail, context);
					break;
				case BannerDestination.ProductsWithBrand:
					await OpenPage(AppArea.BrandList, context);
					break;
				case BannerDestination.DrugAlertsHome:
					await OpenPage(AppArea.Scheduler);
					break;
				case BannerDestination.BiometricHome:
					await OpenPage(AppArea.BiometricData);
					break;
				case BannerDestination.ArticlesHome:
					await OpenPage(AppArea.Contents);
					break;
				case BannerDestination.GamificationHome:
					await OpenPage(AppArea.Points);
					break;
			}
		}

		#region IBranchSessionInterface implementation
#if !WPHONE
		public void InitSessionComplete (Dictionary<string, object> data)
		{
			// Do something with the referring link data...
		}

		public void SessionRequestError (BranchError error)
		{
			// Handle the error case here
		}

		public void CloseSessionComplete() 
		{
		
		}
#endif

		#endregion

	}
}
