using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
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
    public partial class EditDosingSchedulePage : ANFPage
    {

		#region Properties

		private DosingScheduleDetailViewModel _viewModel = new DosingScheduleDetailViewModel();

		#endregion

        #region Page Initialization

		public EditDosingSchedulePage() : base() { }

		public EditDosingSchedulePage(DosingScheduleDetailViewModel viewModel) : base() 
		{ 
			this.BindingContext = _viewModel = viewModel;
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

			LoadingView.IsVisible = false;
			_viewModel.OnUpdateComplete += OnUpdateComplete;
			_viewModel.OnError += OnError;

			if (_viewModel == null || _viewModel.DosingSchedule == null || !_viewModel.DosingSchedule.SentByPharmacy) return;

			// Sent by pharmacy, disable notes edit
			NotesInput.IsEnabled = false;
			NotesInput.Opacity = 0.6;
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

		async void OnCloneDosingScheduleButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new AddDosingSchedulePage(_viewModel.DosingSchedule));
		}
		

		async void CancelButtonClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();

			// Clear the edit fields
			_viewModel.ClearEdit();
		}

		async void SaveButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.UpdateSchedule();
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
