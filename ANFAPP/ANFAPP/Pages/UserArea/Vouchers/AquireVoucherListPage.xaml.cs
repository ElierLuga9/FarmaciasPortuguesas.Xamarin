using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.Vouchers
{
	public partial class AquireVoucherListPage : ANFPage
	{

		#region Properties

		public AquireVoucherListViewModel _viewModel { get; set; }
		private bool _isInitialized = false;

		#endregion

		#region Page Initialization

		public AquireVoucherListPage() : base()
		{
			CardAvaliable();

		}

		protected override void InitPage()
		{

			InitializeComponent();

			base.InitPage();

		}

		#endregion

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (!_isInitialized)
			{



				// Initialize context
				this.BindingContext = _viewModel = new AquireVoucherListViewModel();

				_viewModel.OnError += OnError_EventHandler;
				_viewModel.OnSuccess += OnSuccess_EventHandler;


				_isInitialized = true;
			}


		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnError -= OnError_EventHandler;
			_viewModel.OnSuccess -= OnSuccess_EventHandler;
		}

		public void CardAvaliable()
		{
			var points = SessionData.PharmacyUser.Points;

			if (points < 50)
			{

				CartaoVale2.IsVisible = false;
				CartaoVale2Disable.IsVisible = true;
				Button2.IsVisible = false;
			}
			else {

				CartaoVale2.IsVisible = true;
				CartaoVale2Disable.IsVisible = false;
				Button2.IsVisible = true;
			}

			if (points < 120)
			{
				CartaoVale5.IsVisible = false;
				CartaoVale5Disable.IsVisible = true;
				Button5.IsVisible = false;
			}
			else {
				CartaoVale5.IsVisible = true;
				CartaoVale5Disable.IsVisible = false;
				Button5.IsVisible = true;
			}



			if (points < 230)
			{
				CartaoVale10.IsVisible = false;
				CartaoVale10Disable.IsVisible = true;
				Button10.IsVisible = false;
			}
			else {
				CartaoVale10.IsVisible = true;
				CartaoVale10Disable.IsVisible = false;
				Button10.IsVisible = true;
			}

			if (points < 440)
			{
				CartaoVale20.IsVisible = false;
				CartaoVale20Disable.IsVisible = true;
				Button20.IsVisible = false;
			}
			else {
				CartaoVale20.IsVisible = true;
				CartaoVale20Disable.IsVisible = false;
				Button10.IsVisible = true;
				Button20.IsVisible = true;
			}

		}
		#region Event Handlers

		void OnVoucherOffer_Clicked(object sender, EventArgs args)
		{

			//if (!(sender is Button)) return;

			Navigation.PushAsync(new VoucherAquisitionPage(((View)sender).ClassId));
		}

		public void OnListSelection_EventHandler(object sender, SelectedItemChangedEventArgs e)
		{
			// Disable selection
			if (e.SelectedItem == null) return;
			((ListView)sender).SelectedItem = null;
		}

		async void OnError_EventHandler(string title, string message)
		{
			LoadingView.IsVisible = false;

			// Show error message
			await DisplayAlert(title, message, AppResources.OK);
			await Navigation.PopAsync();
		}

		void OnSuccess_EventHandler()
		{
			LoadingView.IsVisible = false;

			// Cross-platform!
			//Test.InputTransparent = !_viewModel.AtLeastOneAvailable;
			//Test.IsEnabled = _viewModel.AtLeastOneAvailable;
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.AquireVoucherListPageTitle;
		}


		//testes
		protected override bool HasCardButton()
		{
			return true;
		}
		#endregion

	}
}