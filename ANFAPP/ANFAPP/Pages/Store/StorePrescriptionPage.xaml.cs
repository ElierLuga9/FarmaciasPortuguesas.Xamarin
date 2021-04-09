using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Views;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Wrappers;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ANFAPP.Logic.Models.Out.Ecommerce;
using Acr.BarCodes;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.Store
{
	public partial class StorePrescriptionPage : ANFStorePage 
	{	
		private bool _initialized = false;
		private StorePrescriptionViewModel _viewModel = new StorePrescriptionViewModel();

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _viewModel;
		}
			
		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnCartChangeSuccess += OnCartChangeSuccess;

			LoadingView.IsVisible = false;

			if (_initialized)
			{
				_initialized = true;

				// MixPanel Event
				RegisterMixPanelEvent();
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnCartChangeSuccess -= OnCartChangeSuccess;
		}

		#endregion

		private void RegisterMixPanelEvent() 
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();

			var props = new Dictionary<string, string>();
			props.Add("Selection", _viewModel.SearchByActivePrinciple ? "Principio Ativo" : "CNP");

			mixpanelWidget.TrackProperties("MedicalPrescription_Access", props);
		}

		#region Event Handlers

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
		}

		void OnCartChangeSuccess()
		{
			// http://issue.innovagency.com/view.php?id=20604
			Navigation.PushAsync(new StoreBasketPage());
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}
			
		void ToggleClick (object sender, EventArgs args)
		{
			SearchEntry.Unfocus ();

			_viewModel.SearchByActivePrinciple = !_viewModel.SearchByActivePrinciple;
			BtnToggleSearch.State = _viewModel.SearchByActivePrinciple;

			if (_viewModel.SearchByActivePrinciple) {
				SearchHeader.Text = AppResources.StorePrescriptionActivePrincipleInputLabel;
				SearchEntry.Keyboard = Keyboard.Text;
			} else {
				SearchHeader.Text = AppResources.StorePrescriptionCNPInputLabel;
				SearchEntry.Keyboard = Keyboard.Numeric;
			}

			// MixPanel Event
			RegisterMixPanelEvent();

			// Clear previous search query
			SearchEntry.Text = string.Empty;

			// Clear previous search results.
			/*
			PASearchWrapper.IsVisible = false;
			*/
		}

		void OnRemove(object sender, EventArgs args)
		{
			if (sender == null || !(sender is PrescriptionItem)) return;

			_viewModel.UpdateQuantity (0, sender);
		}

		void OnQtyChanged(object sender, EventArgs args)
		{
			if (sender == null || !(sender is PrescriptionItem)) return;

			var textArgs = args as TextChangedEventArgs;

			int qty;
			if (int.TryParse (textArgs.NewTextValue, out qty)) {
				_viewModel.UpdateQuantity (qty, sender);
			}
		}

		async void OnSearchClicked(object sender, EventArgs args)
		{
			SearchEntry.Unfocus ();

			if (!string.IsNullOrWhiteSpace (SearchEntry.Text)) {

				var q = SearchEntry.Text.Trim();

				if (_viewModel.SearchByActivePrinciple) {
					
					var result = await _viewModel.SearchPA (q);

					if (result != null) {
						_viewModel.PAQueryItems = new ObservableCollection<PAListItem> (result);
						_viewModel.PAQuery = q;
						await Navigation.PushAsync (new StorePrescriptionPAListPage (_viewModel));
					}
				} else {
					await _viewModel.Search (q);
				}
			}
		}

		async void AddToCartClicked (object sender, EventArgs args)
		{
			SearchEntry.Unfocus();
			await _viewModel.AddProductsToBasket ();
		}

		async void ReadBarcodeButtonClicked(object sender, EventArgs args)
		{
			var result = await BarCodes.Instance.Read();
			if (result.Success)
			{
				SearchEntry.Text = result.Code;
				OnSearchClicked(sender, args);
			}
		}

		#endregion

		#region Application Bar Settings

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePageTitle;
		}

		protected override string GetAppBarSubtitle()
		{
			return "Receita Médica";
		}

		#endregion
	}
}

