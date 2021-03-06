using ANFAPP.Helpers;
using ANFAPP.Logic;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.Pages.Articles;
using ANFAPP.Pages.BiometricData;
using ANFAPP.Pages.Contacts;
using ANFAPP.Pages.DosageScheduler;
using ANFAPP.Pages.GetPoints;
using ANFAPP.Pages.PharmacyLocator;
using ANFAPP.Pages.Store;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;

using Plugin.Connectivity;


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Views
{

    #region Inner Classes

    public enum _MenuItems
    {
        User,
        Home,
        Promotions,
        Locator,
        Store,
        Health,
        Scheduler,
        Card,
        BioParams,
        Points,
        Contacts
    }

    public class MenuItem
    {
        public string IconSmile { get; set; }
        public string IconName { get; set; }
        public string CardIcon { get; set; }
        public string UserIcon { get; set; }
        public string ItemName { get; set; }
        public _MenuItems MenuType { get; set; }
        public int Count { get; set; }
        public int Promo { get; set; }

    }

    #endregion

    public partial class MenuView : ContentView
    {
        #region Properties

        
        //public PromotionsViewModel _promoViewModel = new PromotionsViewModel();
       
        //testes
        //public int total = LazyLoad.PTotal;
        public int total = PromotionsViewModel.PromoTotals;
		public PromotionsViewModel  contador = new PromotionsViewModel();


        public int t { get; set; }
        //public int total = App.PromotionsVM.PromoTotal;

        public int Promototal()
        {
			
            //if (PromotionsViewModel.PromoTotals != total)
            //{
            //    return PromotionsViewModel.PromoTotals;
            //}

            if (App.PromotionsVM.PromoTotal != total)
            {
                if (total > 0)
                {
                    return total;
                }
                else
                    return App.PromotionsVM.PromoTotal;
            }
            else
                    return total;
        }

        
        


        public List<MenuItem> MenuItems
        {
            get
            {
                var menuList = new List<MenuItem>() {

                    new MenuItem () {
                        IconName = "ic_menu_home2",
                        ItemName = AppResources.MenuItemHome,
                        MenuType = _MenuItems.Home
                    },

                    new MenuItem () {
                        IconName = "ic_menu_locator",
                        ItemName = AppResources.MenuItemLocator,
                        MenuType = _MenuItems.Locator
                    },
                    new MenuItem () {
                        IconName = "ic_navbar_cart",
                        ItemName = AppResources.MenuItemStore,
                        MenuType = _MenuItems.Store
                    },
                    new MenuItem () {
                        IconName = "ic_menu_health2",
                        ItemName = AppResources.MenuItemHealth,
                        MenuType = _MenuItems.Health
                    },
                    new MenuItem () {
                        IconName = "ic_menu_tomas2",
                        ItemName = AppResources.MenuItemScheduler,
                        MenuType = _MenuItems.Scheduler
                    },
                    new MenuItem () {
                        IconName = "ic_menu_params",
                        ItemName = AppResources.MenuItemBioParams,
                        MenuType = _MenuItems.BioParams
                    },
                    new MenuItem () {
                        IconName = "ic_menu_contacts",
                        ItemName = AppResources.MenuItemContacts,
                        MenuType = _MenuItems.Contacts
                    }
                };

                //TESTES
               // if (total != Promototal) { Promototal=total;}
               //t = Promototal();

                if (App.PromotionsVM.PromoTotal != null)
                {

                    if (App.PromotionsVM.PromoTotal != 0)
                    {
                            menuList.Insert(1, new MenuItem()
                            {
                                CardIcon = "ic_menu_promocoes3",
                                ItemName = AppResources.MenuItemPromotions,
                                MenuType = _MenuItems.Promotions,
                                Promo = App.PromotionsVM.PromoTotal
                            }

                            );

                        }
                        else
                        {
                            menuList.Insert(1, new MenuItem()
                            {
                                CardIcon = "ic_menu_promocoes3",
                                ItemName = AppResources.MenuItemPromotions,
                                MenuType = _MenuItems.Promotions,
                                Promo = 0
                            }

                            );
                        }
                    
                }

                if (SessionData.PharmacyUser != null && SessionData.PharmacyUser.Name != null)
                {
                    menuList.Insert(4, new MenuItem()
                        {
                            IconSmile = "ic_smile_64x64",
                            ItemName = AppResources.MenuItemCard,
                            MenuType = _MenuItems.Card,
                            Count = SessionData.PharmacyUser.Points
                        }

                    );
                    menuList.Insert(8, new MenuItem()
                {
                    //IconName = "",
                    UserIcon = "ic_menu_utilizador",
                    ItemName = SessionData.PharmacyUser.Name,
                    MenuType = _MenuItems.User
                }

            );


                }
                else
                {
                    menuList.Insert(4, new MenuItem()
                    {
                        IconSmile = "ic_smile_64x64",
                        ItemName = AppResources.MenuItemCard,
                        MenuType = _MenuItems.Card,
                        Count = 0
                    }
                    );

                    menuList.Insert(8, new MenuItem()
                    {
                        //IconName = "",
                        UserIcon = "ic_menu_utilizador",
                        ItemName = AppResources.MenuItemUser,
                        MenuType = _MenuItems.User
                    }
                    );

                }

                //if (Settings.IS_GAMIFICATION_MENU_ACTIVE)
                //{
                //    // Only add option if gamification is active.
                //    menuList.Add(new MenuItem()
                //    {
                //        IconName = "ic_menu_points",
                //        ItemName = AppResources.MenuItemPoints,
                //        MenuType = _MenuItems.Points
                //    });
                //}

                return menuList;
            }
        }

        #endregion

        #region Events

        public event OnLoadStartEventHandler OnLoadStart;
        public event OnSuccessEventHandler OnLoadEnd;

        #endregion

        #region Initialization

        public MenuView() { }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            BindingContext = this;
			ReloadMenu();
        }


        /// <summary>
        /// Function that renders the view's XAML.
        /// </summary>
        public void RenderView()
        {
            InitializeComponent();
        }

		public void ReloadMenu() {
            OnPropertyChanged("MenuItems");
		}

        #endregion

        #region Button Event Handlers

        public async void OnDismissClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        public async void MenuItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
			//testes
			await LazyLoad.LoadPromotions(false);

            MenuItem item = (MenuItem)args.SelectedItem;
            if (null == item) return;

            // Show Loading
            if (OnLoadStart != null) await OnLoadStart();

            Menu.SelectedItem = null;
            switch (item.MenuType)
            {
                case _MenuItems.User:
                    GoToUser();
                    break;
                case _MenuItems.Home:
                    GoToHome();
                    break;
                case _MenuItems.Promotions:
                    GoToPromotions();
                    break;
                case _MenuItems.Locator:
                    GoToLocator();
                    break;
                case _MenuItems.Store:
                    GoToStore();
                    break;
                case _MenuItems.Health:
                    GoToHealth();
                    break;
                case _MenuItems.Scheduler:
                    GoToScheduler();
                    break;
                case _MenuItems.Card:
                    GoToCard();
                    break;
                case _MenuItems.BioParams:
                    GoToBioParams();
                    break;
                case _MenuItems.Points:
                    GoToPoints();
                    break;
                case _MenuItems.Contacts:
                    GoToContacts();
                    break;
            }

            if (OnLoadEnd != null) OnLoadEnd();

        }

        #region Menu Navigation

        private void ShowPermissionsError()
        {

            NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.MenuPermissionsError, AppResources.OK);
        }

        private void ShowConnectionError()
        {

            NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.NetworkingErrorMenuMessage, AppResources.OK);
        }


        private async void GoToUser()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

            // Validate if there is a user logged in
            if (SessionData.PharmacyUser == null)
            {
                // Go to login page 
				if (NavigationUtils.TopPageOfType(typeof(QuickRegisterPage), Navigation)) return;
                //NavigationUtils.PushPageAndClearHistory(new QuickRegisterPage(), Navigation);
                await Navigation.PushAsync(new QuickRegisterPage());
            }
            else
            {
                // Make sure that this page isnt already open.
                if (!NavigationUtils.ContainsPageOfType(typeof(UserAreaMainPage), Navigation.NavigationStack))
                {
                    if (NavigationUtils.TopPageOfType(typeof(UserAreaMainPage), Navigation)) return;
                    //NavigationUtils.PushPageAndClearHistory(new UserAreaMainPage(), Navigation);
                    await Navigation.PushAsync(new UserAreaMainPage());

                }
                else
                {
                    NavigationUtils.PopToPageType(typeof(UserAreaMainPage), Navigation);
                }
            }

        }

        private async void GoToHome()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(HomePage), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new HomePage(), Navigation);
            await Navigation.PushAsync(new HomePage());
        }

        private async void GoToPromotions()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

			PromotionsDAO pDAO2 = new PromotionsDAO();
			var listsize = await pDAO2.GetAllPromo();
			foreach (var a in listsize)
			{
				var promoId = new Promotion()
				{
					Id = a.Id,
					Read = true

				};
				await pDAO2.Update(promoId);

			}
			await LazyLoad.LoadPromotions(false);

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(PromotionsPage), Navigation)) return;
            await Navigation.PushAsync(new PromotionsPage());
        }

        private async void GoToLocator()
        {
            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(DashboardLocator), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new DashboardLocator(), Navigation);
            await Navigation.PushAsync(new DashboardLocator());
        }

        private async void GoToStore()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

            // If default pharmacy - show a popup to suggest a pharmacy selection to the user
            if (!SessionData.IsPharmacySelected)
            {
                var result = await NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null,
                    AppResources.PickYourPharmacyPopupMessage,
                    AppResources.PickYourPharmacyPopupSelectButton,
                    AppResources.PickYourPharmacyPopupLaterButton);

                if (result)
                {
                    GoToLocator();
                    return;
                }
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(StoreLandingPage), Navigation)) return;
            //await NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);


            await Navigation.PushAsync(new StoreLandingPage());

            //await NavigationUtils.PushPageKeepHistory(new StoreLandingPage(), Navigation);
        }

        private async void GoToHealth()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(ArticlesMainPage), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new ArticlesMainPage(), Navigation);
            await Navigation.PushAsync(new ArticlesMainPage());
        }

        private async void GoToScheduler()
        {
            if (SessionData.PharmacyUser == null)
            {
                // User not logged
                ShowPermissionsError();
                return;
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(DosingSchedulePage), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new DosingSchedulePage(true), Navigation);
            await Navigation.PushAsync(new DosingSchedulePage(true));
        }

        private async void GoToCard()
        {
            // Card Page
            if (SessionData.PharmacyUser == null)
            {
                //Show Permission Error
                ShowPermissionsError();
                return;
            }
            else
            {
                if (SessionData.HasPharmacyCard)
                {
                    // Go to dashboard
                    // Navigate only if not already there.
                    if (NavigationUtils.TopPageOfType(typeof(UserCardPage), Navigation)) return;
                    //NavigationUtils.PushPageAndClearHistory(new UserCardPage(), Navigation);
                    await Navigation.PushAsync(new UserCardPage());
                }
                else
                {
                    // Go to Sauda Card promo page
                    // Navigate only if not already there.
                    if (NavigationUtils.TopPageOfType(typeof(UserNotLoggedCardPage), Navigation)) return;
                    //NavigationUtils.PushPageAndClearHistory(new UserNotLoggedCardPage(), Navigation);
                    await Navigation.PushAsync(new UserNotLoggedCardPage());
                }
            }
        }

        private async void GoToBioParams()
        {
            if (SessionData.PharmacyUser == null)
            {
                // User not logged
                ShowPermissionsError();
                return;
            }

            // Validate if a user already exists.
            if (SessionData.BiometricUser == null)
            {
                // Create new user
                // Navigate only if not already there.
                if (NavigationUtils.TopPageOfType(typeof(NewUserPage), Navigation)) return;
                //NavigationUtils.PushPageAndClearHistory(new NewUserPage(), Navigation);
                await Navigation.PushAsync(new NewUserPage());
            }
            else
            {
                // Go to dashboard
                // Navigate only if not already there.
                if (NavigationUtils.TopPageOfType(typeof(DashboardPage), Navigation)) return;
                //NavigationUtils.PushPageAndClearHistory(new DashboardPage(), Navigation);
                await Navigation.PushAsync(new DashboardPage());
            }
        }

        private async void GoToPoints()
        {
            if (SessionData.PharmacyUser == null)
            {
                // User not logged
                ShowPermissionsError();
                return;
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(GetPointsMain), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new GetPointsMain(), Navigation);
            await Navigation.PushAsync(new GetPointsMain());
        }

        private async void GoToContacts()
        {
            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(ContactsPage), Navigation)) return;
            //NavigationUtils.PushPageAndClearHistory(new ContactsPage(), Navigation);
            await Navigation.PushAsync(new ContactsPage());
        }

        #endregion

        #endregion

    }
}

