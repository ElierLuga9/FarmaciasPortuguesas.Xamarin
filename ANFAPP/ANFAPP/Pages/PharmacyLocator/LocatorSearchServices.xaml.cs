using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Helper;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.ViewModel;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Ecommerce;


namespace ANFAPP.Pages.PharmacyLocator
{
	public partial class LocatorSearchServices : ANFPage
	{
		#region Properties

		private PharmacyServicesViewModel _viewModel;
		private bool _isInitialized = false;

		#endregion

		#region Page Initialization


		public LocatorSearchServices() : base () 
		{
			this.BindingContext = _viewModel = new PharmacyServicesViewModel();
		}

		/// <summary>
		/// Initializes the page
		/// </summary>
		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		
		}

		#endregion

		#region Page Lifecylce

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ListView.SelectedItem = null;

			_viewModel.OnLoadFinished += OnLoadFinished;

			if (!_isInitialized) {
				_isInitialized = true;

				// Load Data
				LoadData ();
			} else {
				LoadingView.IsVisible = false;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadFinished -= OnLoadFinished;
		}

		#endregion

		#region Loaders

		//public async void LoadNextPage() { }
		/// <summary>
		/// Load the data for this view
		/// </summary>
		public void LoadData()
		{
			// Show loading
			LoadingView.IsVisible = true;

			_viewModel.LoadData ();
		}

		public void FilterData(string filter)
		{
			filter = !string.IsNullOrEmpty(filter) ? filter.ToUpper() : string.Empty;
			_viewModel.FilterData(filter);
		}

		#endregion

		#region Event Handlers

		public void ClearInputClick(object sender, EventArgs args)
		{
			SearchEntry.Text = string.Empty;
			FilterData(null);
		}

		void OnSearchTextChanged(object sender, TextChangedEventArgs args)
		{
			// Update button image
			SearchBarButtonImage.Source = (string.IsNullOrEmpty(args.NewTextValue)) ? "magnifier" : "ic_delete";

			// Filter data
			FilterData(args.NewTextValue);
		}

		async void OnListItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null) return;
			ListView.SelectedItem = null;

			PharmacyService selectedService = (PharmacyService)args.SelectedItem;

			// Show Loading
			LoadingView.IsVisible = true;
			// Delay so the loading takes effect
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			try
			{
				SessionData.allPharmacies = new List<Logic.Database.Models.Pharmacy>();
				var pharmacies = await PharmacyHelper.GetPharmaciesByService(selectedService.Id);
				if (pharmacies == null || pharmacies.Count == 0) {
					/// No results
					await DisplayAlert(null, AppResources.PharmacyNoServiceResultsMessage, AppResources.OK);
				}
				else
				{
					SessionData.IsServicePagePharm = true;
					SessionData.SelectedService = selectedService.Id;
					await Navigation.PushAsync(new LocatorMap(pharmacies, MenuCurrentSelection.SERVICES,true, false, false));
					HideLoading();
				}
			}
			catch (Exception e)
			{
				// Service Error
				ShowErrorDialog(e.Message);
			}
		}

		public void OnLoadFinished()
		{
			LoadingView.IsVisible = false;
		}

		private async void ShowErrorDialog(string message)
		{
			LoadingView.IsVisible = false;
			await DisplayAlert(null, message, AppResources.OK);
		}

		#endregion

		#region Application Bar Methods

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.TitleLocator;
		}

		#endregion
	}
}
