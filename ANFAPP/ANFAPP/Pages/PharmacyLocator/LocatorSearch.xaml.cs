using System;
using System.Collections.Generic;
using ANFAPP.Pages;
using ANFAPP.Views;
using Xamarin.Forms;
using ANFAPP.Logic;
using System.Diagnostics;
using ANFAPP.ViewModel;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Helper;
using System.Threading.Tasks;


namespace ANFAPP.Pages.PharmacyLocator
{
	public partial class LocatorSearch : ANFPage
    {
        #region Properties

        public LocatorDataType currentDataType;
        public string DistrictID;
        public string CountyID;

        private LocalizationViewModel _vmodel;
        private bool _isInitialized = false;

        #endregion

        #region Page Initialization


        public LocatorSearch() : base () 
		{
			this.BindingContext = _vmodel = new LocalizationViewModel();
		}

		public LocatorSearch(LocatorDataType dataType, string idDistrict = null, string idCounty = null, string idParish = null) : this() 
		{ 
            // Save the referenced parameters
			currentDataType = dataType;
            DistrictID = idDistrict;
            CountyID = idCounty;
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

			if (currentDataType != LocatorDataType.DISTRITOS)
			{
				SeeAllButton.IsVisible = true;
				SeeAllButton.HeightRequest = 40;
			}

            _vmodel.OnLoadFinished += OnLoadFinished;

            if (!_isInitialized)
            {
                _isInitialized = true;

                // Load Data
                LoadData();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LoadingView.IsVisible = false;

            _vmodel.OnLoadFinished -= OnLoadFinished;
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Load the data for this view
        /// </summary>
        public void LoadData()
        {
            // Show loading
            LoadingView.IsVisible = true;

            //selection of data type
            switch (currentDataType)
            {
                case LocatorDataType.DISTRITOS:
					ApplicationBar.SetTitle(AppResources.TitleLocatorDistrict);
					_vmodel.LoadData(currentDataType);
                    break;

                case LocatorDataType.CONCELHOS:
					ApplicationBar.SetTitle(AppResources.TitleLocatorCounty);
					_vmodel.LoadData(currentDataType, DistrictID);
                    break;

                case LocatorDataType.FREGUESIA:
					ApplicationBar.SetTitle(AppResources.TitleLocatorParish);
					_vmodel.LoadData(currentDataType, DistrictID, CountyID);
                    break;

                default: break;
            }
        }

        public void FilterData(string filter)
        {
            filter = !string.IsNullOrEmpty(filter) ? filter.ToUpper() : string.Empty;
            _vmodel.FilterData(filter);
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

            // Show Loading
            LoadingView.IsVisible = true;

            // Delay so the loading takes effect
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

            LocalizationShort selectedLocal = (LocalizationShort)args.SelectedItem;

            switch (currentDataType)
            {
                case LocatorDataType.DISTRITOS:
                    DistrictID = selectedLocal.Id;
                    await Navigation.PushAsync(new LocatorSearch(LocatorDataType.CONCELHOS, selectedLocal.Id));
                    break;

                case LocatorDataType.CONCELHOS:
                    CountyID = selectedLocal.Id;
                    await Navigation.PushAsync(new LocatorSearch(LocatorDataType.FREGUESIA, DistrictID, selectedLocal.Id));
                    break;

                case LocatorDataType.FREGUESIA:
                    try
                    {
                        var pharmacies = await PharmacyHelper.GetPharmaciesByParish(DistrictID, CountyID, selectedLocal.Id);
                        await Navigation.PushAsync(new LocatorMap(pharmacies, MenuCurrentSelection.LOCALITY));
                    }
                    catch (Exception e)
                    {
                        // Service Error
                        ShowErrorDialog(e.Message);
                    }
                    break;

                default: break;
            }

            // Show Loading
            LoadingView.IsVisible = false;
        }

        public void OnLoadFinished()
        {
            LoadingView.IsVisible = false;
        }

        private async void ShowErrorDialog(string message)
        {
            await DisplayAlert(null, message, AppResources.OK);
        }

		public async void OnSeeAllButtonClicked(object sender, EventArgs args) 
        {

			//var rowLS = ((Image)sender).BindingContext as LocalizationShort;

            // Show Loading
            LoadingView.IsVisible = true;

            // Delay so the loading takes effect
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			try 
            {
				switch (currentDataType) 
                {
					case LocatorDataType.CONCELHOS:
					    var pharmaciesD = await PharmacyHelper.GetPharmaciesByDistrict (DistrictID);
					    await Navigation.PushAsync (new LocatorMap(pharmaciesD, MenuCurrentSelection.LOCALITY));
					    ListView.SelectedItem = null;
					    break;
				    case LocatorDataType.FREGUESIA: 
					    var pharmaciesC = await PharmacyHelper.GetPharmaciesByCounty (DistrictID, CountyID);
					    await Navigation.PushAsync (new LocatorMap(pharmaciesC, MenuCurrentSelection.LOCALITY));
					    ListView.SelectedItem = null;
					    break;
				}
			} 
            catch (Exception e)
            {
                ShowErrorDialog(e.Message);
			}

            LoadingView.IsVisible = false;
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

