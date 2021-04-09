using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using Xamarin.Forms;

namespace ANFAPP.Pages.DosageScheduler
{
    public partial class DosingScheduleDetailPage : ANFPage
    {

		#region Properties

		private bool _isInitialized = false;

		private DosingSchedule _schedule;
		private DosingScheduleDetailViewModel _viewModel = new DosingScheduleDetailViewModel();

		#endregion

        #region Page Initialization

		public DosingScheduleDetailPage() : base() { }

		public DosingScheduleDetailPage(DosingSchedule schedule) : base() 
		{ 
			_schedule = schedule;
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

			_viewModel.OnLoadComplete += OnLoadFinished;
			_viewModel.OnError += OnError;

			if (!_isInitialized)
			{
				BindingContext = _viewModel;
				_viewModel.LoadData(_schedule);
				_isInitialized = true;
			}
			else
			{
				_viewModel.LoadDosages();
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadComplete -= OnLoadFinished;
			_viewModel.OnError -= OnError;
		}

		void OnLoadFinished()
		{
			LoadingView.IsVisible = false;
		}

		void OnError(string title, string message)
		{
			DisplayAlert(title, message, AppResources.OK);
		}

		async void OnDosageDetailClicked(object sender, EventArgs args)
		{
			var view = sender as View;
			if (view.BindingContext == null || !(view.BindingContext is Dosage)) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new DosageEditPage(_viewModel.DosingSchedule, view.BindingContext as Dosage));
		}

		async void OnCheckedDosageButtonClicked(object sender, EventArgs args)
		{
			var view = sender as View;
			if (view.BindingContext == null || !(view.BindingContext is Dosage)) return;

			// Reverse flag
			var dosage = view.BindingContext as Dosage;
			dosage.Done = !dosage.Done;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			// Update the dosage
			_viewModel.UpdateDosage(dosage);
		}

		async void OnDeleteDosageButtonClicked(object sender, EventArgs args)
		{
			var view = sender as View;
			if (view.BindingContext == null || !(view.BindingContext is Dosage)) return;
            
			// Show confirmation dialog
			if (!await DisplayAlert(null, string.Format(AppResources.DosingsDeleteMessage, 
				(view.BindingContext as Dosage).ReprDateTime.Day,
                (view.BindingContext as Dosage).ReprDateTime.Month,
                (view.BindingContext as Dosage).ReprDateTime.Hour,
                (view.BindingContext as Dosage).ReprDateTime.Minute), AppResources.Yes, AppResources.No)) return;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			// Delete dosage
			_viewModel.DeleteDosage(view.BindingContext as Dosage);
		}

		async void OnAddDosageButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new DosageEditPage(_viewModel.DosingSchedule));
		}

		async void OnEditDosingScheduleButtonClicked(object sender, EventArgs args) 
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new EditDosingSchedulePage(_viewModel));
		}

		async void OnCloneDosingScheduleButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new AddDosingSchedulePage(_viewModel.DosingSchedule));
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
