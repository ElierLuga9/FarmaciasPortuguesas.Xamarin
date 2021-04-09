using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Store
{
	public partial class PromotionHighlightsPage : ANFStorePage
	{
		

		#region Page Initialization
		private PromotionHighlightsViewModel _viewModel;
		public List<Product> _cNPList;
		protected bool _initialized = false;


		private PromotionHighlightsPage() : base() { }

	

		public PromotionHighlightsPage(List<Product> cNPList) :base()
		{
			_cNPList = cNPList;
			BindingContext = _viewModel = new PromotionHighlightsViewModel(_cNPList);

		}

		protected override void InitPage()
		{

			InitializeComponent();

			base.InitPage();




		}

		#endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += LoadStart;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnLoadError += OnLoadError;

		if (!_initialized)
			{
				_viewModel.LoadData();


				_initialized = true;
			}

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_viewModel.OnLoadStart -= LoadStart;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnLoadError -= OnLoadError;
	
		}

		#endregion

		#region Event Handlers

		async void OnArticleTapped2(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			//ProductsList.SelectedItem = null;

			var product = e.SelectedItem as PromotionHighlightsViewModel.PromoItem;
			var cnp = product.Cnp;

			if (cnp != null)
			{
				await Navigation.PushAsync(new StoreProductDetailPage((int)cnp));
			}



			}


	

		async Task LoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

	
		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;



		}


		void OnLoadError()
		{
			LoadingView.IsVisible = false;
			DisplayAlert(null, "Os produtos em promoção não estão disponíveis nesta Farmácia", AppResources.OK);



		}

		

		#endregion

		#region Search Events


		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePageTitle;
		}

		#endregion
	}
}
