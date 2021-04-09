using System;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.Store;
using ANFAPP.Views;

namespace ANFAPP.Pages.UserArea.Vouchers
{
	public partial class IndustryVoucherDetailPage : ANFPage
	{
		#region Properties

		private Voucher _voucher;
		private VoucherDetailViewModel _viewModel;

		#endregion

		#region Page Initialization

		public IndustryVoucherDetailPage() : base() { }

		public IndustryVoucherDetailPage(Voucher voucher) : base() 
		{
			_voucher = voucher;
			if (_voucher.Type.Equals(Settings.VOUCHER_TYPE_SPONSORED))
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

		protected override void OnAppearing()
		{
			base.OnAppearing();


				// Initialize binding context
				BindingContext = _viewModel = new VoucherDetailViewModel();
				_viewModel.LoadData(_voucher);

				_viewModel.LoadImage(_voucher.Value);

		}

		#endregion

		void VoucherTap(object sender, EventArgs args)
		{
			Navigation.PushAsync(new BarcodePage(_viewModel.Voucher.Code, SessionData.PharmacyUser.CardNumber, SessionData.PharmacyUser.Name));
		}
		async void OnAddVoucherToCardClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			var added = await ECommerceWS.AddProdsVoucher(SessionData.UserAuthentication, _voucher.Code, 0);
			if (added.result.Equals(400))
			{
				var accept = await DisplayAlert(null, "O Vale já se encontra no seu carrinho", "Ir para Carrinho", "Ir para Loja");
				if (!accept)
				{

					Navigation.PushAsync(new StoreLandingPage());

				}
				else 
				{
					Navigation.PushAsync(new StoreBasketPage());
				}
			}
			else 
			{
				var accept = await DisplayAlert(null, "O Vale foi adicionado ao seu carrinho", "Ir para Carrinho", "Ir para Loja");
				if (!accept)
				{

					Navigation.PushAsync(new StoreLandingPage());

				}
				else
				{
					Navigation.PushAsync(new StoreBasketPage());
				}
			}

			LoadingView.IsVisible = false;

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


