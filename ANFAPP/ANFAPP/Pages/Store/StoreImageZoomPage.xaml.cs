using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.ViewModels;

namespace ANFAPP.Pages.Store
{
	public partial class StoreImageZoomPage : ANFPage
	{
			
		#region Properties

		private StoreImageDetailViewModel _viewModel;

		#endregion

		#region Page Initialization

		public StoreImageZoomPage(StoreImageDetailViewModel viewModel) : base() 
		{ 
			_viewModel = viewModel;
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
			return "Detalhe da Imagem";
		}

		#endregion

		#region Events

		protected override void OnAppearing()
		{
			base.OnAppearing();
			BindingContext = _viewModel;

			if (_viewModel != null && _viewModel.SelectedImage != null)
			{

				var html = Device.OS != TargetPlatform.WinPhone ? 
					@"<html><head><meta name=""viewport"" content=""width=device-width, initial-scale=0.25, maximum-scale=1.0""></head><body><img width=""100%"" src=""{0}""></img></body></html>" : 
					@"<html><head><meta name=""viewport"" content=""width=device-width, initial-scale=0.25, maximum-scale=1.0""></head><body><img src=""{0}""></img></body></html>";

				ZoomableWebView.Source = new HtmlWebViewSource()
				{
					Html = string.Format(html, _viewModel.SelectedImage.Uri.ToString())
				};
			}
		}

		 #endregion
	}
}

