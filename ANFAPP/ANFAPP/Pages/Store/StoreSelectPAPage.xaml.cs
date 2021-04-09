using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Pages;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Views;
using System.Threading.Tasks;
using ANFAPP.Utils;

namespace ANFAPP.Pages.Store
{
	public partial class StoreSelectPAPage : ANFPage
	{
		private StorePrescriptionViewModel _context;
		private StoreSelectPAViewModel _viewModel = new StoreSelectPAViewModel();

		public StoreSelectPAPage (StorePrescriptionViewModel context, PAListItem selected)
		{
			_context = context;
			_viewModel.SelectedItem = selected;
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _viewModel;

			//ApplicationBar.HideUserButton ();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			LoadingView.IsVisible = false;

			await _viewModel.LoadPA (_viewModel.SelectedItem.Id, null);
			if (_viewModel.DosageFilter == null || _viewModel.DosageFilter.Count == 0) {
				await DisplayAlert ("", "Não foram encontrados produtos com o princípio ativo selecionado", AppResources.OK);
			
				DosagePicker.IsEnabled = false;
				PackPicker.IsEnabled = false;
				AddListButton.IsEnabled = false;
			}

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
		}

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


		async void AddClicked(object sender, EventArgs args)
		{
			if (_viewModel.SelectedPack != null) {
				_context.AddWithPA (_viewModel.Result, _viewModel.SelectedPack.CNPEM);
				NavigationUtils.PopToPageType (typeof(StorePrescriptionPage), Navigation);
			}
		}

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePrescriptionPATitle;
		}

		protected override string GetAppBarSubtitle()
		{
			return AppResources.StorePrescriptionPASubtitle;
		}

		#endregion
	}
}
