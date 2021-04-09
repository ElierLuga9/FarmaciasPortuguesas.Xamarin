using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic;

namespace ANFAPP.Pages.Store
{
	public partial class StoreImageDetailPage : ANFPage
	{
			
		#region Properties

		private StoreImageDetailViewModel _viewModel;

		#endregion

		#region Page Initialization

		public StoreImageDetailPage(ProductDetailOut product) : base() 
		{ 
			_viewModel = new StoreImageDetailViewModel (product);
						BindingContext = _viewModel;

		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle ()
		{
			return AppResources.ImageGalleryPageTitle;
		}

		#endregion

		#region Events

		public void ThumbnailTapped (object sender, EventArgs args)
		{
			_viewModel.SelectImage (sender as Image);
		}

		public async void OnProductImageTapped(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new StoreImageZoomPage(_viewModel));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			LoadingView.IsVisible = false;
		}

 		#endregion
	}
}

