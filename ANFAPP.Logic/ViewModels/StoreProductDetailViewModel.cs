using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using System.Threading.Tasks;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Logic.ViewModels
{
	public class StoreProductDetailViewModel : IViewModel
	{
		#region
        private int _quantity=1;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

		private string _farmId;
		private int _cnp;

		private bool _isInFavorites;
		public bool IsInFavorites
		{
			get { return _isInFavorites; }
			set { 
				_isInFavorites = value;
				OnPropertyChanged ();

				if (_isInFavorites) {
					FavImage = "ic_star_orange_filled";
				} else {
					FavImage = "ic_star_orange";
				}
			}

		}

		private string _favImage = "ic_star_orange";
		public string FavImage 
		{
			get { return _favImage; }
			set { 
				_favImage = value;
				OnPropertyChanged ();
			}
		}

		private FavoritesViewModel _favVM;
		private int _favLineId;

		private ProductDetailOut _product;
		public ProductDetailOut Product {
			get { return _product; }
			set { 
				_product = value; 
				OnPropertyChanged ("Product");
			}
		}

		public OnErrorEventHandler OnLoadError;
		public OnErrorEventHandler OnProductLoadError;
		public OnLoadStartEventHandler OnLoadStart;
        public OnLoadStartEventHandler OnAddCartStart;
		public OnSuccessEventHandler OnLoadSuccess;

		public StoreProductDetailViewModel (string farmId, int cnp, FavoritesViewModel ctx = null) : base()
		{
			_farmId = farmId;
			_cnp = cnp;
			_favVM = ctx;
		}

		#endregion

		public async Task AddToFavorites()
		{
			if (Product == null || Product.CNP == null || IsInFavorites)
				return;

			if (OnLoadStart != null) await OnLoadStart();

			_favVM.OnLoadError += OnLoadError;
			if (await _favVM.AddToFavorites (Product.CNP.GetValueOrDefault ())) {
				// We need to update the favorites to get the line id.
				UpdateFavorites ();
			} 
			_favVM.OnLoadError -= OnLoadError;

			if (OnLoadSuccess != null) OnLoadSuccess ();
		}

		public async Task RemoveFromFavorites()
		{
			if (Product == null || Product.CNP == null || !IsInFavorites)
				return;

			if (OnLoadStart != null) await OnLoadStart();

			_favVM.OnLoadError += OnLoadError;
			if (await _favVM.RemoveFromFavorites(_favLineId)) {
				IsInFavorites = false;
			}
			_favVM.OnLoadError -= OnLoadError;

			if (OnLoadSuccess != null) OnLoadSuccess ();
		}

		public async void LoadData()
		{
			if (OnLoadStart != null) await OnLoadStart();

			try {
				var result = await ECommerceWS.ProdDetail (SessionData.UserAuthentication, _farmId, _cnp);
				Product = result;

				if (null != _favVM) {
					await _favVM.InitializeFavorites();

					UpdateFavorites();
				}

			} catch (Exception e) {
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;

				if (OnProductLoadError != null)
					OnProductLoadError ("", e.Message);
				else if (OnLoadError != null)
					OnLoadError ("", e.Message);
			}

			if (OnLoadSuccess != null) OnLoadSuccess();
		}

		public void UpdateFavorites()
		{
			var inFavorites = false;

			if (_favVM.Products != null) {
				foreach (ProductOut p in _favVM.Products) {
					if ((p.CNP != 0 && (p.CNP == Product.CNP)) || (p.CNPEM != 0 && (p.CNPEM == Product.CNPEM))) {
						_favLineId = p.LineNumber.GetValueOrDefault();
						inFavorites = true;
						break;
					}
				}
			}

			IsInFavorites = inFavorites;
		}
	}
}
