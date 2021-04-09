using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Pages.Store;
using ANFAPP.Utils;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class ECProductHighlight : ContentView
	{
		public delegate Task OnTaskStartedEventHandler();
		public event OnTaskStartedEventHandler OnTaskStarted;

		public static readonly BindableProperty FromCatalogProperty = BindableProperty.Create<ECProductHighlight, bool>(p => p.FromCatalog, false);
		public bool FromCatalog
		{ 
			get { return (bool)GetValue(FromCatalogProperty); }
			set { SetValue(FromCatalogProperty, value); }
		}

		public ECProductHighlight()
		{
			InitializeComponent();
			IsVisible = false;
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext != null) {
				IsVisible = true;

				var p = BindingContext as ProductOut;

				BuyButton.IsVisible = !(SessionData.IsPharmacySelected == false && p != null && p.HasPoints == false);
			}
		}

		async void OnProductClicked(object sender, EventArgs args)
		{
			if (BindingContext == null || !(BindingContext is ProductOut)) return;

			var p = BindingContext as ProductOut;
			if (p.CNP != null)
			{
				if (OnTaskStarted != null) await OnTaskStarted();
				await Navigation.PushAsync(new StoreProductDetailPage(p.CNP.GetValueOrDefault()));
			}
		}

		async void OnAddToCartButtonClicked(object sender, EventArgs args)
		{
			if (BindingContext == null || !(BindingContext is ProductOut)) return;

			// Add product to Cart
			var product = BindingContext as ProductOut;
			if (SessionData.IsAuthenticatedWithPharmacy) {		
				
				if (OnTaskStarted != null) await OnTaskStarted();

				await App.StoreBasketVM.AddProductToBasketWithCatalogRules(product, product.FromCatalog);
				//if (product.FromCatalog) {
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
				NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.AddToCartNotLoggedMessage, AppResources.OK);
			}
		}
	}
}
