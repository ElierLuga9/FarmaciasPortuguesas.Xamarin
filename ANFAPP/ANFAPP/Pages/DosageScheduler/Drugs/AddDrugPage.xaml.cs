using System;
using System.Threading.Tasks;
using Acr.BarCodes;
using ANFAPP.Views;
using ANFAPP.Views.Common;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
    public partial class AddDrugPage : ANFPage
    {
        private AddDrugViewModel _addDrugVM = new AddDrugViewModel (); 
        private AddDosingScheduleViewModel _addScheduleVM;
		private string _cnpQuery;

        #region Page Initialization

        public AddDrugPage() : base() 
		{ 
			BindingContext = _addDrugVM;
		}

		public AddDrugPage(AddDosingScheduleViewModel scheduleVM) : this() 
		{ 
			_addScheduleVM = scheduleVM;
		}

		public AddDrugPage(string cnpQuery) : this() 
		{ 
			_cnpQuery = cnpQuery;
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			SearchField.TextChanged -= SearchField.EnforceMaxLength;

			SearchField.AccessoryTapped += async (object sender, Xamarin.Forms.ElementEventArgs e) =>
			{
				var entry = sender as CustomEntry;

				bool hasFocus = entry.IsFocused;

				// http://issue.innovagency.com/view.php?id=20381
				if (!string.IsNullOrWhiteSpace(entry.Text) && entry.Text.Length >= 3)
				{
					if (hasFocus) entry.Unfocus();

					// A new search clears the CNP query.
					_cnpQuery = null;

					await PushSearchPageWithText(entry.Text.Trim());
				} else {
					if (!hasFocus) entry.Focus();
				}
			};
				
			SearchField.Completed += async (object sender, System.EventArgs e) =>
			{
				var entry = sender as CustomEntry;

				// http://issue.innovagency.com/view.php?id=20381
				if (!string.IsNullOrWhiteSpace(entry.Text) && entry.Text.Length >= 3)
				{
					entry.Unfocus();

					// A new search clears the CNP query.
					_cnpQuery = null;

					await PushSearchPageWithText(entry.Text.Trim());
				} else {
					// XXX: Should we let "return" dismiss the keyboard if the Entry has text but not enough to search?
					// if (!string.IsNullOrWhiteSpace(entry.Text)) entry.Focus();
				}
			};
        }

		private async Task PushSearchPageWithText(string text)
		{
			var drugSearchPage = new DrugSearchPage(text);
			drugSearchPage.OnDrugSelected += delegate(object drug, EventArgs _)
			{
				_addDrugVM.UpdateSearchResults((ProductOut)drug);
			};

			await Navigation.PushAsync(drugSearchPage);
		}

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.DosageSchedulerPageTitle;
        }

        protected override string GetAppBarSubtitle()
        {
            return AppResources.DosageSchedulerTabMedicineLabel;
        }

        #endregion

        #region Event Handlers

        async void ReadBarcodeButtonClicked(object sender, EventArgs args)
        {
            var result = await BarCodes.Instance.Read();
            if (result.Success) {
                //var msg = String.Format("Barcode Found.  Type: {0} - Code: {1}", result.Format, result.Code);
                //System.Diagnostics.Debug.WriteLine (msg);
               // LoadingView.IsVisible = true;
                //await _addDrugVM.AddMedicine();
				await _addDrugVM.SelectMedicineWithCNP(result.Code);
            }
        }

        void CheckboxClicked(object sender, EventArgs args)
        {
            _addDrugVM.WarnUser = !_addDrugVM.WarnUser;
        }

        async void CancelClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync ();
        }

        async void AddClicked(object sender, EventArgs args)
        {
            LoadingView.IsVisible = true;
            await _addDrugVM.AddMedicine (); 
        }

		/// <summary>
		/// A medicine was added with success.
		/// </summary>
		/// <param name="medicine">the new medicine.</param>
        async void OnAddMedicineSuccess(Medicine medicine) 
        {
			// Resynchronize schedules due to auto-generated schedules for specific medicines.
			await App.DosingScheduleVM.SynchronizeSchedules();

            LoadingView.IsVisible = false;

            if (_addScheduleVM != null)
            {
                // Part of the AddDosingSchedule Form
                _addScheduleVM.Medicine = medicine;
            }

            await Navigation.PopAsync ();
        }

		async Task OnLoadStarted()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		/// <summary>
		/// An event other than adding a medicine was completed with success.
		/// </summary>
        void OnLoadComplete()
        {
            LoadingView.IsVisible = false;
        }

        void OnError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing ();

			_addDrugVM.OnLoadComplete += OnLoadComplete;
			_addDrugVM.OnLoadStart += OnLoadStarted;
            _addDrugVM.OnSuccess += OnAddMedicineSuccess;
            _addDrugVM.OnError += OnError;

			LoadingView.IsVisible = false;
			if (!string.IsNullOrEmpty(_cnpQuery)) {
				_addDrugVM.SelectMedicineWithCNP (_cnpQuery);
			}
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

			_addDrugVM.OnLoadComplete -= OnLoadComplete;
			_addDrugVM.OnLoadStart -= OnLoadStarted;
            _addDrugVM.OnSuccess -= OnAddMedicineSuccess;
            _addDrugVM.OnError -= OnError;
        }
    }
}
