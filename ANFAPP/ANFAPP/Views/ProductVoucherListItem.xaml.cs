using ANFAPP.Logic.Models.Out.Ecommerce;
using System;
using Xamarin.Forms;
using ANFAPP.Logic;
using ANFAPP.Utils;
using ANFAPP.Logic.Network.Services;

using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Pages.Store;

namespace ANFAPP.Views
{
    public partial class ProductVoucherListItem : ContentView
    {
        public delegate bool IsGenericsButtonEnabled();
		public delegate Task OnTaskStartedEventHandler();
		public event OnTaskStartedEventHandler OnTaskStarted;
        #region Bindable Properties

        public static readonly BindableProperty IsRemovableProperty = BindableProperty.Create<ProductVoucherListItem, bool>(p => p.IsRemovable, false);
        public static readonly BindableProperty DisplayPricesProperty = BindableProperty.Create<ProductVoucherListItem, bool>(p => p.DisplayPrices, true);
        public static readonly BindableProperty ShowGenericsButtonProperty = BindableProperty.Create<ProductVoucherListItem, bool>(p => p.ShowGenericsButton, true);

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

        public bool ShowGenericsButton
        {
            get { return (bool)GetValue(ShowGenericsButtonProperty); }
            set { SetValue(ShowGenericsButtonProperty, value); }
        }

        #endregion

        public ProductVoucherListItem()
        {
            InitializeComponent();

        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
			/*BuyButton.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command((obj) => {

					AddToCartClicked += OnAddToCartButtonClicked;
				})

			});*/
			/*var tapGestureRecognizer = new TapGestureRecognizer();

			tapGestureRecognizer.Tapped += OnAddToCartButtonClicked;

			BuyButton.GestureRecognizers.Add(tapGestureRecognizer);*/
            if (string.Equals(propertyName, IsRemovableProperty.PropertyName))
            {
                //DeleteButton.IsVisible = IsRemovable;
                //
				//DeleteButtonImage.IsVisible = IsRemovable;
            }            else if (string.Equals(propertyName, DisplayPricesProperty.PropertyName))
            {
				PriceLabel.IsVisible = DisplayPrices;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
			var tapGestureRecognizer = new TapGestureRecognizer();



			BuyButton.Focus();
			BuyButton.GestureRecognizers.Add(tapGestureRecognizer);
            if (BindingContext != null && BindingContext is ProductOut)
            {
				

                var product = BindingContext as ProductOut;
				if (product.ExistsInFarmCatalog == false)
				{
					ButtonNotAvailable.IsVisible = true;
					ButtonNotAvailable.IsEnabled = true;
					BuyButton.IsVisible = false;
					WImageURL.Opacity = 0.5;
				}
				else{
					BuyButton.IsVisible = !(SessionData.IsPharmacySelected == false && product.HasPoints == false);
				
	                //if (CanDisplayGenerics != null) {
	                //	SeeGenericsButton.IsVisible = product.HasCNPEM && CanDisplayGenerics ();
	                //}

	                // The ProductOut 'FromCatalog' property should be ignored if false, that is, if 'true' the 
	                // product is from the catalog but if 'false' we don't know if the product is from the catalog.
	                // If the product is from the catalog the price should be 0 or null but that is not guaranteed.
	                PriceLabel.IsVisible = true;
					PriceLabel.Text = string.Format("{0} €", product.Price);

	              /*  // Catalog products shouldn't show points.
	                if (DisplayPoints && !DisplayPrices || product.FromCatalog)
	                {
	                }
					*/
	                // http://issue.innovagency.com/view.php?id=20552
	            /*    if ((!DisplayPrices && !DisplayPoints) || product.PriceDescription == null)
	                {
	                    BuyButton.IsVisible = false;
	                    NoPriceButton.IsVisible = true;
	                }*/

               }

            }
        }


        #region Button Handlers



        void OnRemoveButtonClicked(object sender, EventArgs args)
        {
            if (RemoveClicked != null) RemoveClicked(sender, args);

            // Removal should be done on the main view
        }

		async void OnAddToCartButtonClicked(object sender, EventArgs args) 
        {

            if (AddToCartClicked != null) AddToCartClicked(sender, args);

			if (BindingContext == null || !(BindingContext is ProductOut)) return;

			var app = this.Parent.Parent.FindByName<Grid>("LoadingView");
			app.IsVisible = true;


            // Add product to Cart
            var product = BindingContext as ProductOut;
			if (SessionData.IsAuthenticatedWithPharmacy)
			{
				try
				{

					var response = await ECommerceWS.AddProdsVoucher(SessionData.UserAuthentication, SessionData.OpenedVoucher.Code, product.CNP.Value);

					//System.Diagnostics.Debug.WriteLine("Result from AddProdsVoucher request: " + AddToCard.result);


					if (response.result.Equals(400))
					{
						var accept = await App.Current.MainPage.DisplayAlert(null, "Este vale já se encontra no carrinho",  "Ir para Carrinho","Ir para Loja");
						if (!accept)
						{
							
							await App.Current.MainPage.Navigation.PushAsync(new StoreLandingPage());
							app.IsVisible = false;

						}
						else
						{
							await App.Current.MainPage.Navigation.PushAsync(new StoreBasketPage());
							app.IsVisible = false;
						}
						App.StoreBasketVM.PeekBasket();

					}
					else {
						var accept = await (App.Current.MainPage.DisplayAlert(null, AppResources.SucessVoucherAndProdutToCard,  "Ir para Carrinho", "Ir para Loja"));
						if (!accept)
						{
							await App.Current.MainPage.Navigation.PushAsync(new StoreLandingPage());
							app.IsVisible = false;

						}
						else 
						{
							await App.Current.MainPage.Navigation.PushAsync(new StoreBasketPage());
							app.IsVisible = false;
						}

						App.StoreBasketVM.PeekBasket();
						   

					}

					if (response.result.Equals(400)	)			
					{
						if (Navigation.NavigationStack.Count == 0)
						{
							return;
						}
						else
							NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.AddToCartNotLoggedMessage, AppResources.OK);
					}
					else
					{
						if (Navigation.NavigationStack.Count == 0)
						{
							return;
						}
						else
							NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.AddToCartNotLoggedMessage, AppResources.OK);
					}
						
				}
				catch (InvalidCastException)
				{
					var accept = await App.Current.MainPage.DisplayAlert(null, "Este vale já se encontra no carrinho", "Ir para Carrinho", "Ir para Loja");
					if (!accept)
					{

						await App.Current.MainPage.Navigation.PushAsync(new StoreLandingPage());
						app.IsVisible = false;

					}
					else
					{
						await App.Current.MainPage.Navigation.PushAsync(new StoreBasketPage());
						app.IsVisible = false;
					}
				}

			}				
				//TODO ADICIONAS AOS STRING APPRESOURCES O TEXTO PARA O USER



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

