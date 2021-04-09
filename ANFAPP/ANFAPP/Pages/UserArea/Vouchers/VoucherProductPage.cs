using System;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Utils;
using ANFAPP.Pages.Store;
using Xamarin.Forms;
using ANFAPP.Views.Common;
using ANFAPP.Logic.EventHandlers;

namespace ANFAPP.Pages.UserArea.Vouchers
{
	public partial class VoucherProductPage : ANFPage
	{
		
	
		#region Properties
		int _maxPerPage = 20;
		int rInicial = 0;
	//	bool hasMore = true;
		public OnLoadStartEventHandler OnLoadStart;

		private ObservableCollection<ProductOut> products = new ObservableCollection<ProductOut>();
		private Voucher _voucher;
		private bool Offer = false;
		bool first = true;
		bool footerLoading = false;
		private bool _isOnLoading  ;
		public bool isOnLoading
		{
			get { return _isOnLoading; }
			set
			{
				_isOnLoading = value;
				OnPropertyChanged("isOnLoading");
			}
		}
		private bool _hasMore = true;
		public bool hasMore
		{
			get { return _hasMore; }
			set
			{
				_hasMore = value;
				OnPropertyChanged("hasMore");
			}
		}

		#endregion

		#region Page Initialization

		public VoucherProductPage(Voucher voucher)
		{
			_voucher = voucher;
			SessionData.OpenedVoucher = voucher;
		}

	/*	public VoucherProductPage(Voucher voucher) : base()
		{
			_voucher = voucher;
			if (_voucher.Type.Equals(ANFAPP.Logic.Settings.VOUCHER_TYPE_SPONSORED))
			{
				ApplicationBar.SetTitle(AppResources.VouchersGiftDetailPageTitle);
			}
		}*/

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
			hasMore = true;
			ProductsList.LoadMoreCommand = new Command(LoadNextPage);
		}

		#endregion

		#region Page Lifecycle 

        protected  override void OnAppearing()
        {
            base.OnAppearing();
			//	ProductsList.FooterBinding
			isOnLoading = false;
			footerLoading = true;
			LoadingView.IsVisible = true;
			//ProductOutListPage(_voucher);
            // Initialize binding context
			// _viewModel.LoadData(_voucher);
			//		ProductsListVouchers.ItemsSource = _viewModel.ProductsVouchers;
			ProductsList.ItemsSource = products;
			ProductsList.Footer = hasMore && isOnLoading;
			LoadProducts(first);

            //_viewModel.OnError += OnError_EventHandler;
        //    _viewModel.OnSuccess += OnLoadSuccess;
			//_viewModel.OnStart += OnLoad;
			App.StoreBasketVM.OnLoadStart += OnCartLoadStart;

		//	HighlightWidget.LoadData();
			if (Offer == true)
			{
				DisplayAlert(null, "Foi adicionado um novo vale aos seus Vales e Promoções", AppResources.OK);
			}

        }
		public void Loading()
		{
			LoadingView.IsVisible = true;
		}
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
			App.StoreBasketVM.OnLoadStart -= OnCartLoadStart;
           // _viewModel.OnError -= OnError_EventHandler;
            //_viewModel.OnStart -= OnLoad;
			//_viewModel.OnSuccess -= OnLoadSuccess;

        }


		#endregion

		async Task OnCartLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

        void OnLoad()
        {
			
            LoadingView.IsVisible = true;
		//	ListViewActivityIndicator.IsVisible = true;
		//	ListViewActivityIndicator.IsRunning = true;



        }
		protected void LoadNextPage()
		{
			if (!isOnLoading && hasMore)
			{
				NextPage();
			}
			else 
			{
				hasMore = false;
			}
		}
		public async Task NextPage()
		{
			if (!hasMore) return;
			//IsLoading = true;
			isOnLoading = true;
			first = false;
			try
			{


				var result = await LoadProducts(first);

				if (result == null || result.numeroDeProdutos < 20)
				{
					hasMore = false;
					isOnLoading = false;
					footerLoading = false;
				}
				else
				{
					hasMore = true;
					footerLoading = true;
					foreach (var prod in result.Products)
					{
						products.Add(prod);
					}
					isOnLoading = false;
				}

				//ProductsList.ItemsSource = products;

				/*// Because the observable collection items are not observable themselves.
				var pg = ProductsGrouped;
				ProductsGrouped = null;
				ProductsGrouped = pg;

				var pp = Products;
				Products = null;
				Products = pp;
*/


			}
			catch (Exception ex)
			{

			}
			finally
			{
				isOnLoading = false;
			}
		}

		private async Task<ProductVoucherOut> LoadProducts(bool firstLoad)
		{
			isOnLoading = true;
			ProductsList.ItemsSource = products;

			var result = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, _voucher.Code, _maxPerPage, rInicial);
			if (firstLoad)
			{
				rInicial = 0;
				footerLoading = false;
				foreach (ProductOut prod in result.Products)
				{
					products.Add(prod);
				}
				isOnLoading = false;
				LoadingView.IsVisible = false;
				return result;
			}
			else
			{
				footerLoading = true;
				rInicial = result.Products.Count;
				var results2 = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, _voucher.Code, _maxPerPage,rInicial);
				isOnLoading = false;
				LoadingView.IsVisible = false;
				return results2;
			}
		}
        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;
			//ProductsListVouchers.ItemsSource = _viewModel.ProductsVouchers;
        }

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			LoadingView.IsVisible = true;
			var selectedItem = ProductsList.SelectedItem;
			ProductsList.SelectedItem = null;

			if (selectedItem != null && selectedItem is ProductOut)
			{
				ProductOut p = selectedItem as ProductOut;
				if (p.CNP != null && p.ExistsInFarmCatalog)
				{
					await Navigation.PushAsync(new StoreProductDetailPage(p.CNP.GetValueOrDefault()));
					LoadingView.IsVisible = false;

				}
				else
				{
					await DisplayAlert(null, AppResources.NotAvaliableInPharm, AppResources.OK);
					LoadingView.IsVisible = false;

				}

			}

		}

        void OnLoadError(string title, string message)
        {
            LoadingView.IsVisible = false;
            //LoadingMessage.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }


		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.VouchersDetailPageTitle;
		}

		//testes
		protected override bool HasCardButton()
		{
			return true;
		}

		#endregion

	}
}
