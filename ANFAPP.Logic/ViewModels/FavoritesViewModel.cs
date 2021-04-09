using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using Xamarin.Forms;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class FavoritesViewModel : IViewModel
	{
		private bool _initialized = false;

		#region Events

		public OnErrorEventHandler OnLoadError;
		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadSuccess;

		#endregion

		#region Properties

		private ObservableCollection<ProductOut> _products;
		public ObservableCollection<ProductOut> Products {
			get { return _products; }
			set { 
				_products = value;
				OnPropertyChanged ("Products");
			}
		}

		public int ProductsCount
		{
			get { return Products != null ? Products.Count : 0; }
		}

		public bool IsLoaded = false;

		#endregion

		public void WipeFavoriteCache()
		{
			Products = null;
			IsLoaded = false;
		}

		public async Task<bool> AddToFavorites(int cnp)
		{
			if (OnLoadStart != null) await OnLoadStart();

			bool success = false;
			try {
				var result = await ECommerceWS.AddWishlistProd(SessionData.UserAuthentication, cnp);
				success = true;
			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			} 
				
			await LoadFavoritesAsync ();
			if (OnLoadSuccess != null) OnLoadSuccess();
			return success;
		}

		public async Task<bool> RemoveFromFavorites(int lineId)
		{
			if (OnLoadStart != null) await OnLoadStart();

			bool success = false;
			try {
				var result = await ECommerceWS.ItemWishlistDelete(SessionData.UserAuthentication, lineId);
				success = true;
			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				if (OnLoadError != null) OnLoadError("", message);
			} 

			await LoadFavoritesAsync ();
			if (OnLoadSuccess != null) OnLoadSuccess();
			return success;
		}

		private async Task LoadFavoritesAsync()
		{
			try {
				var result = await ECommerceWS.GetWishlist(SessionData.UserAuthentication);
				if (result.Products.Count > 0) 
				{
					if (Device.OS == TargetPlatform.WinPhone) 
					{ 
						// When the page loads on WPhone, this list behaves strangely and reloads again for no reason..
						// This delay is needed to help the layout processing.. Xamarin FTW
						Products = null;
						await Task.Delay(500);
					}

					Products = new ObservableCollection<ProductOut>(result.Products);

					// Same as previous delay, otherwise the layout wont render before the loading ends on WPhone
					if (Device.OS == TargetPlatform.WinPhone) await Task.Delay(500);
				} else {
					Products = null;
				}

			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex.Message);
			} 

			IsLoaded = true;
			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		public async Task LoadData() 
		{
			if (OnLoadStart != null) await OnLoadStart();

			await LoadFavoritesAsync ();
		}

		public async Task InitializeFavorites()
		{
			if (!_initialized && SessionData.IsAuthenticatedWithPharmacy) {
				LoadFavoritesAsync ();
				_initialized = true;
			}
		}
	}
}
