using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic;
using ANFAPP.Views;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Pages.Store
{
	public partial class StoreProductTextPage : ANFPage
	{
		#region Properties

		private string _title;
		private string _contents;

		#endregion

		#region Page Initialization

		public StoreProductTextPage(string title, string contents) : base() 
		{ 
			_title = title;
			_contents = contents;
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

		protected override string GetAppBarTitle()
		{
			return _title;
		}

		#endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			TextContents.Text = _contents;
			if (null != _title) {
				ApplicationBar.SetTitle (_title);
			}
			/*
			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			BindingContext = _viewModel;
			_viewModel.LoadData();
			*/
		}
		/*
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}
		*/
		#endregion

		/*
		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}
		*/
	}
}