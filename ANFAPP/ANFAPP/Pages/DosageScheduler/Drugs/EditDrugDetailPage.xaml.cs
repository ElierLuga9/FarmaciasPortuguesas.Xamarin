using System;
using System.Threading.Tasks;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
	public partial class EditDrugDetailPage : ANFPage
	{
		private Medicine _drug;
		private DrugDetailViewModel _viewModel;

		private EditDrugDetailPage () : base() {}

		public EditDrugDetailPage(object drug) : base () {
			_drug = (Medicine)drug;
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			_viewModel = new DrugDetailViewModel();
			BindingContext = _viewModel;
		}

		#region Event Handlers

		protected async override void OnAppearing()
		{
			base.OnAppearing ();

			LoadingView.IsVisible = false;
			_viewModel.OnUpdateComplete += OnUpdateComplete;
			_viewModel.OnError += OnError;

			if (_viewModel.Drug == null) {
				_viewModel.Drug = _drug;
			}

			await _viewModel.LoadData ();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnUpdateComplete -= OnUpdateComplete;
			_viewModel.OnError -= OnError;
		}

		void OnUpdateComplete()
		{
			Navigation.PopAsync();
		}

		void OnError(string title, string message)
		{
			LoadingView.IsVisible = false;

			// Show error message
			DisplayAlert(title, message, AppResources.OK);
		}

		async void OnCancelClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync ();
		}

		async void OnSaveClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.UpdateDrug ();
		}

        void CheckboxClicked(object sender, EventArgs args)
        {
			_viewModel.WarningFlag = !_viewModel.WarningFlag;
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
	}
}
