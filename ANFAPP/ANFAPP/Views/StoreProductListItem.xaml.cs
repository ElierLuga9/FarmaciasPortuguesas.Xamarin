using ANFAPP.Logic.Models.Out.Ecommerce;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic;
using ANFAPP.Utils;

namespace ANFAPP.Views
{
    public partial class StoreProductListItem : ContentView
    {
        public delegate bool IsGenericsButtonEnabled();

        #region Bindable Properties

        public static readonly BindableProperty IsRemovableProperty = BindableProperty.Create<StoreProductListItem, bool>(p => p.IsRemovable, false);
        public static readonly BindableProperty DisplayPricesProperty = BindableProperty.Create<StoreProductListItem, bool>(p => p.DisplayPrices, true);
        public static readonly BindableProperty DisplayPointsProperty = BindableProperty.Create<StoreProductListItem, bool>(p => p.DisplayPoints, false);
        public static readonly BindableProperty ShowGenericsButtonProperty = BindableProperty.Create<StoreProductListItem, bool>(p => p.ShowGenericsButton, true);

        #region Custom Events

        public event EventHandler RemoveClicked;
        public event EventHandler AddToCartClicked;
        public event EventHandler SeeGenericsClicked;
        public event EventHandler NoPriceButtonClicked;

        public event IsGenericsButtonEnabled CanDisplayGenerics;

        #endregion

        #endregion

        #region Bindable Objects

        public bool IsRemovable
        {
            get { return (bool)GetValue(IsRemovableProperty); }
            set { SetValue(IsRemovableProperty, value); }
        }

        public bool DisplayPrices
        {
            get { return (bool)GetValue(DisplayPricesProperty); }
            set { SetValue(DisplayPricesProperty, value); }
        }

        public bool DisplayPoints
        {
            get { return (bool)GetValue(DisplayPointsProperty); }
            set { SetValue(DisplayPointsProperty, value); }
        }

        public bool ShowGenericsButton
        {
            get { return (bool)GetValue(ShowGenericsButtonProperty); }
            set { SetValue(ShowGenericsButtonProperty, value); }
        }

        #endregion

        public StoreProductListItem()
        {
            InitializeComponent();

        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (string.Equals(propertyName, IsRemovableProperty.PropertyName))
            {
                DeleteButton.IsVisible = IsRemovable;
                DeleteButtonImage.IsVisible = IsRemovable;
            }
            else if (string.Equals(propertyName, DisplayPricesProperty.PropertyName))
            {
                PriceLabel.IsVisible = DisplayPrices;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null && BindingContext is ProductOut)
            {
                var product = BindingContext as ProductOut;

                BuyButton.IsVisible = !(SessionData.IsPharmacySelected == false && product.HasPoints == false);

                //if (CanDisplayGenerics != null) {
                //	SeeGenericsButton.IsVisible = product.HasCNPEM && CanDisplayGenerics ();
                //}

                // The ProductOut 'FromCatalog' property should be ignored if false, that is, if 'true' the 
                // product is from the catalog but if 'false' we don't know if the product is from the catalog.
                // If the product is from the catalog the price should be 0 or null but that is not guaranteed.
                PriceLabel.IsVisible = DisplayPrices || DisplayPoints;

                // Catalog products shouldn't show points.
                if (DisplayPoints && !DisplayPrices || product.FromCatalog)
                {
                    PriceLabel.Text = string.Format("{0} PTS", product.Points.GetValueOrDefault());
                }

                // http://issue.innovagency.com/view.php?id=20552
                if ((!DisplayPrices && !DisplayPoints) || product.PriceDescription == null)
                {
                    BuyButton.IsVisible = false;
                    NoPriceButton.IsVisible = true;
                }

                if (!ShowGenericsButton)
                {
                    SeeGenericsButton.IsVisible = false;
                }
            }
        }


        #region Button Handlers

        void OnRemoveButtonClicked(object sender, EventArgs args)
        {
            if (RemoveClicked != null) RemoveClicked(sender, args);

            // Removal should be done on the main view
        }

        void OnAddToCartButtonClicked(object sender, EventArgs args)
        {
			
            if (AddToCartClicked != null) AddToCartClicked(sender, args);

            if (BindingContext == null || !(BindingContext is ProductOut)) return;

            // Add product to Cart
            var product = BindingContext as ProductOut;
            if (SessionData.IsAuthenticatedWithPharmacy)
            {
                var fromCatalog = DisplayPoints && !DisplayPrices || product.FromCatalog;
                App.StoreBasketVM.AddProductToBasketWithCatalogRules(product, fromCatalog);
                //if (DisplayPoints && !DisplayPrices || product.FromCatalog) {
                //	App.StoreBasketVM.AddCatalogProductToBasket(product);
                //} else {
                //	App.StoreBasketVM.AddProductToBasket(product);
                //}
            }
            else if (SessionData.IsAuthenticated)
            {
                // http://issue.innovagency.com/view.php?id=20566
                NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.AddToCartNoPharmacyMessage, AppResources.OK);
            }
            else
            {
                // http://issue.innovagency.com/view.php?id=20774
                //the navigation stack is allways empty, don't know why.
                //And that problem results in a null pointer exception.
                //There's no page in the stack, so I can't use DisplayAlert() method
                if (Navigation.NavigationStack.Count == 0)
                {
                   return;
                }
                else
                    NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.AddToCartNotLoggedMessage, AppResources.OK);
            }
        }

        void OnSeeGenericsButtonClicked(object sender, EventArgs args)
        {
            if (SeeGenericsClicked != null) SeeGenericsClicked(sender, args);
        }

        void OnNoPriceButtonClicked(object sender, EventArgs args)
        {
            if (NoPriceButtonClicked != null) NoPriceButtonClicked(sender, args);
        }

        #endregion

    }
}

