using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.DosageScheduler.Drugs;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.DosageScheduler
{
    public partial class AddDosingSchedulePage : ANFPage
    {

		#region Properties

		private DosingSchedule _scheduleToCopy;
		private AddDosingScheduleViewModel _viewModel = new AddDosingScheduleViewModel();

		#endregion

        #region Page Initialization

		public AddDosingSchedulePage() : base() { }

		public AddDosingSchedulePage(Medicine medicine) : base()
		{
			_viewModel.Medicine = medicine;
		}

		public AddDosingSchedulePage(DosingSchedule scheduleToCopy) : base()
		{
			_scheduleToCopy = scheduleToCopy;
		}
		

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();	
        }

        #endregion

		#region Event Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();

			BindingContext = _viewModel;
			AddForm.IsVisible = true;

			_viewModel.OnLoadFinished += OnLoadFinished;
			_viewModel.OnSuccess += OnSuccess;
			_viewModel.OnError += OnError;

			// Send a schedule to copy, if one exists
			if (_scheduleToCopy != null) 
			{ 
				_viewModel.LoadData(_scheduleToCopy);
				_scheduleToCopy = null;
			}
			else
			{
				_viewModel.LoadData();
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadFinished -= OnLoadFinished;
			_viewModel.OnSuccess -= OnSuccess;
			_viewModel.OnError -= OnError;
		}

		async void OnSuccess(DosingSchedule schedule) 
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			mixpanelWidget.Track("MedicationAlert_Created");

			await Navigation.PushAsync(new DosingScheduleDetailPage(schedule));
			Navigation.RemovePage(this);
		}

		void OnLoadFinished()
		{
			LoadingView.IsVisible = false;
		}

		void OnError(string title, string message)
		{
			LoadingView.IsVisible = false;

			DisplayAlert(title, message, AppResources.OK);
		}

		#endregion

		#region Button Events

		async void OnSubmitButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.AddDosingSchedule();
		}

		void OnCancelButtonClicked(object sender, EventArgs args)
		{
			Navigation.PopAsync();
		}

		void OnIntervalSwitchClicked(object sender, EventArgs args)
		{
			_viewModel.HourInterval = !_viewModel.HourInterval;
		}

		async void OnMedicineButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			
			if (_viewModel.Drugs != null && _viewModel.Drugs.Count > 0)
			{
				// Go to the DrugPicker page if more than 1 drug exists
				await Navigation.PushAsync(new DrugPickerPage(_viewModel));
			}
			else
			{
				// Go to the AddDrug page
				await Navigation.PushAsync(new AddDrugPage(_viewModel));
			}
			
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
			return AppResources.DosageSchedulerPageSubtitle;
		}

        #endregion

    }
}
