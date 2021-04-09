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
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Pages.UserArea.Vouchers
{
	public partial class VouchersDetailPage : ANFPage
	{


		#region Properties
		private Voucher _voucher;
		private VoucherDetailViewModel _viewModel;
		private bool Offer = false;
		private ANFAPP.Logic.Models.Out.VoucherOut.Condition _voucherOut;
		#endregion

		#region Page Initialization

		public VouchersDetailPage() : base() { }

		public VouchersDetailPage(Voucher voucher) : base()
		{
			_voucher = voucher;
			if (_voucher.Type.Equals(ANFAPP.Logic.Settings.VOUCHER_TYPE_SPONSORED))
			{
				ApplicationBar.SetTitle(AppResources.VouchersGiftDetailPageTitle);
			}
		}
		public VouchersDetailPage(Voucher voucher, bool offer) : base()
		{
			Offer = offer;
			_voucher = voucher;
			if (_voucher.Type.Equals(ANFAPP.Logic.Settings.VOUCHER_TYPE_SPONSORED))
			{
				ApplicationBar.SetTitle(AppResources.VouchersGiftDetailPageTitle);
			}

		}
		public VouchersDetailPage(Voucher voucher, ANFAPP.Logic.Models.Out.VoucherOut.Condition voucherOut)
		{
			_voucher = voucher;
			_voucherOut = voucherOut;
			if (_voucher.Type.Equals(ANFAPP.Logic.Settings.VOUCHER_TYPE_SPONSORED))
			{
				ApplicationBar.SetTitle(AppResources.VouchersGiftDetailPageTitle);
			}
		}
		public VouchersDetailPage(Voucher voucher, ANFAPP.Logic.Models.Out.VoucherOut.Condition voucherOut, bool offer)
		{
			_voucher = voucher;
			_voucherOut = voucherOut;
			Offer = offer;
			if (_voucher.Type.Equals(ANFAPP.Logic.Settings.VOUCHER_TYPE_SPONSORED))
			{
				ApplicationBar.SetTitle(AppResources.VouchersGiftDetailPageTitle);
			}
		}
		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}

		#endregion

		#region Page Lifecycle 
		public async void ProductOutListPage(Voucher voucher)
		{


			LoadingView.IsVisible = true;
			System.Diagnostics.Debug.WriteLine("Voucher ID: " + voucher.Code);
			System.Diagnostics.Debug.WriteLine("Burn Conditions:D - "+voucher.BurnConditions);
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			try
			{
				//var prodV = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, voucher.Code, 20, 0);

				if ( _voucherOut.ExclusivePharmacies == null ||_voucherOut.ExclusivePharmacies.Count == 0 || !_voucherOut.IsPharmacyExclusive)
				{
					var prodV = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, voucher.Code, 20, 0);

					if (prodV.Products == null || prodV.Products.Capacity.Equals(0) || prodV == null || prodV.numeroDeProdutos.Equals(0))
					{


						VoucherAddToCard.IsVisible = true;

						//DisplayAlert(null, "Lista Null", AppResources.OK);

					}
					else {
						VoucherProducts.IsVisible = true;
					}
					await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
					LoadingView.IsVisible = false;
				}
				else if (_voucherOut.ExclusivePharmacies.Count > 0 && _voucherOut.IsPharmacyExclusive)
				{

					bool isInCatalog = false;
					var prodV = await ECommerceWS.ProdsVoucher(SessionData.UserAuthentication, voucher.Code, 20, 0);
					foreach (var selectedVoucher in _voucherOut.ExclusivePharmacies)
					{
						if ((int.Parse(SessionData.StorePharmacyId)) == selectedVoucher.Code)
						{
							isInCatalog = true;
						}
						else
						{
							isInCatalog = false;
						}

					}
					await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
					if (isInCatalog)
					{
						VoucherProductsNotAvaliable.IsVisible = false;
						VoucherProducts.IsVisible = true;
						if (prodV.Products == null || prodV.Products.Capacity.Equals(0) || prodV == null || prodV.numeroDeProdutos.Equals(0))
						{

							//LoadingView.IsVisible = false;
							VoucherAddToCard.IsVisible = true;
							VoucherProducts.IsVisible = false;
							//DisplayAlert(null, "Lista Null", AppResources.OK);

						}
						else {
							//LoadingView.IsVisible = false;
							VoucherProducts.IsVisible = true;
							VoucherAddToCard.IsVisible = false;
						}
					}
					else
					{
						VoucherProductsNotAvaliable.IsVisible = true;
						VoucherProducts.IsVisible = false;
					}
					await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
					LoadingView.IsVisible = false;
				}

			}
			catch (InvalidRequestException e)
			{
				VoucherProducts.IsVisible = false;
				VoucherAddToCard.IsVisible = false;
				await DisplayAlert(null, e.Message, AppResources.OK);
				LoadingView.IsVisible = false;
			}
			catch (Exception ex)
			{ 
				LoadingView.IsVisible = false;
			}

		}
        protected async override void OnAppearing()
        {
            base.OnAppearing();
			LoadingView.IsVisible = true;
			//var teste = await UserCardWS.GetFavPharm(SessionData.PharmacyUser.CardNumber);

			//foreach (var b in teste) 
			//{
			//	System.Diagnostics.Debug.WriteLine("Type: " + b.Type);
			//}

			if (_voucher.Type.Equals(Settings.VOUCHER_TYPE_PHARMACY)) 
			{
				BackgroundCard.Source = "vcardpharm.png";
			}
            // Initialize binding context
            BindingContext = _viewModel = new VoucherDetailViewModel();
            _viewModel.LoadData(_voucher);
			ProductOutListPage(_voucher);
			try
			{
				if (_voucherOut.IsPharmacyExclusive && _voucherOut.ExclusivePharmacies.Count != 0)
				{
					xExclusivePharmContainer.IsVisible = true;
					LoadingView.IsVisible = true;
					GetFarmName();
				}
				else
				{
					xExclusivePharmContainer.IsVisible = false;

				}
			}
			catch (Exception ex)
			{ 
				LoadingView.IsVisible = false;
			}

			//VoucherProducts.Text = VoucherProducts.Text + _voucherOut.ExclusivePharmacies[0].Code +"-"+ SessionData.StorePharmacyId;
			//	_viewModel.ProdVoucherRequest(_voucher);
			//		ProductsListVouchers.ItemsSource = _viewModel.ProductsVouchers;

            //_viewModel.OnError += OnError_EventHandler;
            _viewModel.OnSuccess += OnLoadSuccess;
			//_viewModel.OnStart += OnLoad;
		//	HighlightWidget.LoadData();
			if (Offer == true)
			{
				await DisplayAlert(null, "Foi adicionado um novo vale aos seus Vales e Promoções", AppResources.OK);
			}

        }


		public async void GetFarmName()
		{
			
			string farmName ="";
			foreach (var farm in _voucherOut.ExclusivePharmacies)
			{
				var getFarmDetail = await ECommerceWS.GetMyFarmDetail(SessionData.UserAuthentication, ""+farm.Code);
				farmName =  farmName + getFarmDetail.Name + "\n";
				xPharmName.Text = farmName;
			}
			//string removecomma = xPharmName.Text.Remove(xPharmName.Text.Length - 1);

			System.Diagnostics.Debug.WriteLine("GetFarmName: " + farmName + ",");
			//LoadingView.IsVisible = false;
		}
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
			LoadingView.IsVisible = false;
           // _viewModel.OnError -= OnError_EventHandler;
            //_viewModel.OnStart -= OnLoad;
            _viewModel.OnSuccess -= OnLoadSuccess;

        }


		#endregion

		void VoucherTap(object sender, EventArgs args)
		{
            //LoadingView.IsVisible = true;
            //await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			Navigation.PushAsync(new BarcodePage(_viewModel.Voucher.Code, SessionData.PharmacyUser.CardNumber, SessionData.PharmacyUser.Name));
		}
		async void OnSeeAllClicked(object sender, EventArgs args) 
		{
			await Navigation.PushAsync(new VoucherProductPage(_voucher));
		}
		async void OnAddVoucherToCardClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await ECommerceWS.AddProdsVoucher(SessionData.UserAuthentication, _voucher.Code, 0);
			var accept = await DisplayAlert(null, "O Vale foi adicionado ao seu carrinho", "Ir para Carrinho", "Ir para Loja");
			if (!accept)
			{

				Navigation.PushAsync(new StoreLandingPage());

			}
			else 
			{
				Navigation.PushAsync(new StoreBasketPage());
			}
			LoadingView.IsVisible = false;

		}

        void OnLoad()
        {
            LoadingView.IsVisible = true;
        }

        void OnLoadSuccess()
        {
            LoadingView.IsVisible = false;
			//ProductsListVouchers.ItemsSource = _viewModel.ProductsVouchers;
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
