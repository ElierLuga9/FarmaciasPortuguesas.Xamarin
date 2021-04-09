using ANFAPP.Logic;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.Pages.GetPoints;
using ANFAPP.Pages.Store;
using ANFAPP.Pages.UserArea;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Resources;
using ANFAPP.Utils;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Views
{
    public partial class ApplicationBar : ContentView
    {
        #region Enums

        public enum APPBAR_LEFTBUTTON_TYPE { MENU, BACK, NONE };

        #endregion

        #region Properties

        private readonly string DEFAULT_APPBAR_USER_IMAGE = "ic_user_appbar.png";

        private string _baseTitle;
        private APPBAR_LEFTBUTTON_TYPE _leftButtonType = APPBAR_LEFTBUTTON_TYPE.NONE;

        public event OnLoadStartEventHandler NavigationStarted;

        #endregion

        public ApplicationBar()
        {
            InitializeComponent();

            //CardButton.IsVisible = false;
            //CardButtonImage.IsVisible = false;

            FavButton.IsVisible = false;
            FavButtonImage.IsVisible = false;
            //FavouritesCol.Width = 0;
            //FavLabelCol.Width = 0;

            CartButtonImage.IsVisible = false;
            CartButton.IsVisible = false;

            //SetCartColumns();
            //CartButtonCol.Width = 0;
            //CartLabelCol.Width = 0;

        }


        #region Layout Setters

        public void SetUserPointsBackgroundImage()
        {
            if (SessionData.PharmacyUser != null)
            {
                if (UserPoints.Text.Length <= 2)
                {
                    UserPointsBGImage.Source = "points_bg4.png";
                }
                else if (UserPoints.Text.Length == 3)
                {
                    UserPointsBGImage.Source = "points_bg5.png";
                }
                else
                    UserPointsBGImage.Source = "points_bg6.png";
            }
        }

        /// <summary>
        /// Sets the left button type.
        /// </summary>
        /// <param name="leftButton"></param>
        public void SetLeftButtonType(APPBAR_LEFTBUTTON_TYPE leftButton)
        {
            //if (_leftButtonType == leftButton) return;
            _leftButtonType = leftButton;

            switch (leftButton)
            {
                //case APPBAR_LEFTBUTTON_TYPE.MENU:
                //    LeftButton.Clicked -= OnBackButton_Clicked;
                //    LeftButton.Clicked += OnMenuButton_Clicked;

                //    if (Device.Idiom != TargetIdiom.Tablet)
                //    {
                //        LeftButton.IsVisible = true;
                //        LeftButtonImage.IsVisible = true;
                //        LeftButtonImage.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                //        LeftButtonImage.Source = "ic_appbar_menu2.png";
                //    }
                //    else
                //    {
                //        LeftButtonCol.Width = 15;
                //    }
                //    break;
                case APPBAR_LEFTBUTTON_TYPE.BACK:
                    //LeftButton.Clicked -= OnMenuButton_Clicked;
                    LeftButton.Clicked += OnBackButton_Clicked;

                    LeftButton.IsVisible = true;
                    LeftButtonImage.IsVisible = true;
                    LeftButtonImage.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false);
                    LeftButtonImage.Source = "ic_appbar_back.png";
                    LeftButtonCol.Width = 35;
                    MarginCol.Width = 0;
                    break;
                case APPBAR_LEFTBUTTON_TYPE.NONE:
                    LeftButton.Clicked -= OnBackButton_Clicked;
                    LeftButton.Clicked -= OnMenuButton_Clicked;

                    LeftButton.IsVisible = false;
                    LeftButtonImage.IsVisible = false;
                    LeftButtonCol.Width = 0;
                    MarginCol.Width = 13;
                    break;
            }
        }

        public void SetLeftButtonEventHandler(EventHandler ev)
        {
            switch (_leftButtonType)
            {
                //case APPBAR_LEFTBUTTON_TYPE.MENU:
                //    LeftButton.Clicked -= OnMenuButton_Clicked;
                //    LeftButton.Clicked += ev;
                //    break;
                case APPBAR_LEFTBUTTON_TYPE.BACK:
                    LeftButton.Clicked -= OnBackButton_Clicked;
                    LeftButton.Clicked += ev;
                    break;
                default:
                    break;
            }
        }

        public void UnsetLeftButtonEventHandler(EventHandler ev)
        {
            LeftButton.Clicked -= ev;
        }


        // Sets Cart and Favorit columns to zero
        public void SetCartAndFavColumns()
        {
            CartPointsCol.Width = 0;
            CartButtonCol.Width = 0;
            CartButtonImageCol.Width = 0;

            FavPointsCol.Width = 0;
            FavButtonCol.Width = 0;
            FavButtonImageCol.Width = 0;
        }

        /// <summary>
        /// Shows the number of user points.
        /// </summary>
        /// <param name="points"></param>
        public void SetUserPoints(int points)
        {
            if (SessionData.PharmacyUser != null)
            {
                //var userImage = UserAreaViewModel.GetPlatformPhotoLocation();
                //RightButtonMask.IsVisible = true;
                //RightButtonImage.Source = !string.IsNullOrEmpty(userImage) ? userImage : DEFAULT_APPBAR_USER_IMAGE;

                // Logged
                if (points <= 0)
                {
                    UserPointsContainer.IsVisible = false;
                    UserPoints.Text = string.Empty;
                }
                else
                {
                    UserPointsContainer.IsVisible = true;
                    UserPoints.Text = points + string.Empty;
                }
            }
            else
            {
                // Not logged
                UserPointsContainer.IsVisible = false;
                UserPoints.Text = string.Empty;
                //RightButtonMask.IsVisible = false;
                RightButtonImage.Source = "ic_smile_56x56_verde";
                //RightButtonImage.Source = "ic_appbar_not_logged.png";
            }
        }

        /// <summary>
        /// Hides the user button on the top right corner.
        /// </summary>
        public void HideUserButton()
        {
            RightButtonImageArea.IsVisible = false;
            RightButtonMask.IsVisible = false;
            RightButtonImage.IsVisible = false;
            RightButton.IsVisible = false;
            UserPointsContainer.IsVisible = false;
        }

        /// <summary>
        /// Hides the Menu button on the top right corner.
        /// </summary>
        public void HideMenuButton()
        {
            MenuRightButton.IsVisible = false;
            MenuRightButtonImage.IsVisible = false;
        }

        /// <summary>
        /// Show green or white cart button.
        /// </summary>
        public void ShowCartButton(bool selected = false)
        {
            CartButtonImage.IsVisible = true;
            CartButton.IsVisible = true;
            //CartButtonCol.Width = 17;
            //CartLabelCol.Width = 28;

            // http://issue.innovagency.com/view.php?id=20312
            if (NavigationUtils.TopPageOfType(typeof(StoreBasketPage), Navigation) || selected)
            {
                CartButtonImage.Source = "ic_navbar_cart.png";
            }
            else
            {
                CartButtonImage.Source = "ic_navbar_cart_green.png";
            }

            UpdateTitleLayout();
        }

        /// <summary>
        /// Show green or white Favorit button.
        /// </summary>
        public void ShowFavButton(bool selected = false)
        {
            //FavLabelCol.Width = 28;
            //FavouritesCol.Width = 17;
            FavButtonImage.IsVisible = true;
            FavButton.IsVisible = true;

            // http://issue.innovagency.com/view.php?id=20312
            if (TopIsFavorites() || selected)
            {
                FavButtonImage.Source = "ic_navbar_star_white.png";
            }
            else
            {
                FavButtonImage.Source = "ic_navbar_star_green.png";
            }

            UpdateTitleLayout();
        }

        /// <summary>
        /// Show green or white Card button.
        /// </summary>
        public void ShowCardButton(bool selected = false)
        {
            //FavLabelCol.Width = 28;
            //FavouritesCol.Width = 17;
            //CardButtonImage.IsVisible = true;
            //CardButton.IsVisible = true;
            RightButtonImage.IsVisible = true;
            RightButton.IsVisible = true;

            // http://issue.innovagency.com/view.php?id=20312
            if (TopIsCard() || selected)
            {
                RightButtonImage.Source = "ic_smile_56x56_branco.png";
            }
            else
            {
                RightButtonImage.Source = "ic_smile_56x56_verde.png";
            }

            UpdateTitleLayout();
        }

        /// <summary>
        /// Set Card items if button is active
        /// </summary>
        public void SetCardItems(int items)
        {
            if (RightButton.IsVisible)
            {
                //FavLabelCol.Width = 28;
                //FavouritesCol.Width = 17;

                if (items <= 0)
                {
                    UserPointsContainer.IsVisible = false;
                    UserPoints.Text = string.Empty;
                }
                else
                {
                    UserPointsContainer.IsVisible = true;
                    UserPoints.Text = items + string.Empty;
                }

                UpdateTitleLayout();
            }
        }

        /// <summary>
        /// Set Cart items if button is active
        /// </summary>
        public void SetCartItems(int items)
        {
            if (CartButton.IsVisible)
            {
                //CartButtonCol.Width = 17;
                //CartLabelCol.Width = 28;

                if (items <= 0)
                {
                    CartItemsContainer.IsVisible = false;
                    CartItems.Text = null;
                }
                else
                {
                    CartItemsContainer.IsVisible = true;
                    CartItems.Text = items + string.Empty;
                }

                UpdateTitleLayout();
            }
        }

        /// <summary>
        /// Set Favorit items if button is active
        /// </summary>
        public void SetFavItems(int items)
        {
            if (FavButton.IsVisible)
            {
                //FavLabelCol.Width = 28;
                //FavouritesCol.Width = 17;

                if (items <= 0)
                {
                    FavItemsContainer.IsVisible = false;
                    FavItems.Text = string.Empty;
                }
                else
                {
                    FavItemsContainer.IsVisible = true;
                    FavItems.Text = items + string.Empty;
                }

                UpdateTitleLayout();
            }
        }

        /// <summary>
        /// Sets the application bar title.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            _baseTitle = title;

            UpdateTitleLayout();

        }

        protected void UpdateTitleLayout()
        {
            //var text = (string)ConverterResources.Capitalize.Convert(_baseTitle, typeof(string), null, System.Globalization.CultureInfo.DefaultThreadCurrentCulture);
            var text = _baseTitle;

            if (PageSubtitle.IsVisible)
            {
                int available_space = 15; // XXX: in? chars? is the font monospaced? fragile...

                // Resize if is too long and truncate if there is no space available.
                if (text != null)
                {
                    int TextSize = text.Length;

                    if (!FavButtonImage.IsVisible)
                    {
                        available_space += 4;
                    }
                    if (!CartButtonImage.IsVisible)
                    {
                        available_space += 4;
                    }

                    if (available_space + 1 < TextSize)
                    {
                        text = text.Remove(available_space - 3);
                        text += "...";
                    }

                    PageTitle.Text = text;
                }
            }
            else
            {
                PageTitle.Text = text;
            }
        }

        /// <summary>
        /// Sets the application bar subtitle.
        /// </summary>
        /// <param name="subtitle"></param>
        public void SetSubtitle(string subtitle)
        {
            PageSubtitle.Text = subtitle;
            PageSubtitle.IsVisible = !string.IsNullOrEmpty(subtitle);

            var options = PageTitle.VerticalOptions;
            PageTitle.VerticalOptions = PageSubtitle.IsVisible ? new LayoutOptions(LayoutAlignment.End, false) : new LayoutOptions(LayoutAlignment.Center, true);
        }

        /// <summary>
        /// Sets the color of the application bar.
        /// </summary>
        /// <param name="color"></param>
        public void SetApplicationBarColor(Color color)
        {
            if (color == null) return;

            this.BackgroundColor = color;
        }

        #endregion

        #region Button Events
        protected bool TopIsCard()
        {
            if (NavigationUtils.TopPageOfType(typeof(UserCardPage), Navigation) || NavigationUtils.TopPageOfType(typeof(UserNotLoggedCardPage), Navigation))
            {
                return true;
            }
            else
                return false;

        }

        protected bool TopIsFavorites()
        {
            //var count = Navigation.NavigationStack.Count;
            //return count > 0 && Navigation.NavigationStack[count - 1].GetType() == typeof(StoreFavoritesPage);
            return NavigationUtils.TopPageOfType(typeof(StoreFavoritesPage), Navigation);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            SetUserPointsBackgroundImage();
        }

        protected void OnBackButton_Clicked(object sender, EventArgs e)
        {
            // Pop the current page
            if (Navigation.NavigationStack.Count > 1)
            {
                Navigation.PopAsync(true);
            }
        }

        protected async void OnMenuButton_Clicked(object sender, EventArgs e)
        {
            //var navStack = Navigation.NavigationStack;
            //if (NavigationUtils.ContainsPageOfType(typeof(MenuPage), navStack))
            //{
            //	// Go back to root (menu)
            //	NavigationUtils.PopToPageType(typeof(MenuPage), Navigation);
            //}
            //else
            //{
            //	// Insert me on root and go back to it
            //	Navigation.InsertPageBefore(new MenuPage(), navStack[0]);
            //	Navigation.PopToRootAsync();
            //}

            // Temporary measure to avoid opening the menu on tablet
            if (Device.Idiom == TargetIdiom.Tablet) return;

            if (NavigationStarted != null) await NavigationStarted();
            await Navigation.PushAsync(new MenuPage());
        }

        protected async void OnUserButton_Clicked(object sender, EventArgs e)
        {
            // Validate if there is a user logged in
            if (SessionData.PharmacyUser == null)
            {
                // Go to login page 
                if (NavigationStarted != null) await NavigationStarted();
                await Navigation.PushAsync(new QuickRegisterPage());
            }
            else
            {
                // Make sure that this page isnt already open.
                if (!NavigationUtils.ContainsPageOfType(typeof(UserAreaMainPage), Navigation.NavigationStack))
                {
                    if (NavigationStarted != null)
                        await NavigationStarted();
                    await Navigation.PushAsync(new UserAreaMainPage());
                }
                else
                {
                    NavigationUtils.PopToPageType(typeof(UserAreaMainPage), Navigation);
                }
            }
        }

        // Navigat to user's card page if is not already open
        protected async void OnCardButton_Clicked(object sender, EventArgs e)
        {
            if (SessionData.HasPharmacyCard)
            {
                // Go to dashboard
                // Navigate only if not already there.
                if (!NavigationUtils.TopPageOfType(typeof(UserCardPage), Navigation))
                {
                    if (NavigationStarted != null) await NavigationStarted();
                    await Navigation.PushAsync(new UserCardPage());
                }
            }
            else
            {
                // Go to Sauda Card promo page
                // Navigate only if not already there.
                if (!NavigationUtils.TopPageOfType(typeof(UserNotLoggedCardPage), Navigation))
                {
                    if (NavigationStarted != null) await NavigationStarted();
                    await Navigation.PushAsync(new UserNotLoggedCardPage());
                }
            }

        }

        // Navigat to user´s cart page if is not already open
        protected async void OnCartButton_Clicked(object sender, EventArgs e)
        {
            if (!NavigationUtils.TopPageOfType(typeof(StoreBasketPage), Navigation))
            {
                if (NavigationStarted != null) await NavigationStarted();
                await Navigation.PushAsync(new StoreBasketPage());
            }
        }

        // Navigat to user's Favorit page if is not already open
        protected async void OnFavButton_Clicked(object sender, EventArgs e)
        {
            if (TopIsFavorites())
            {
                await Navigation.PopAsync();
            }
            else
            {
                if (NavigationStarted != null) await NavigationStarted();
                await Navigation.PushAsync(new StoreFavoritesPage());
            }
        }

        #endregion
    }
}