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
    public partial class DosageEditPage : ANFPage
    {

		#region Properties

		private DosageViewModel _viewModel = new DosageViewModel();

		#endregion

        #region Page Initialization

		public DosageEditPage(DosingSchedule dosingSchedule) : this(dosingSchedule, null) { } 

		public DosageEditPage(DosingSchedule dosingSchedule, Dosage dosage) : base() 
		{ 
			this.BindingContext = _viewModel = new DosageViewModel() 
			{
				DosingSchedule = dosingSchedule,
				Dosage = dosage
			};
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

			_viewModel.OnSuccess += OnSuccess;
			_viewModel.OnError += OnError;

			_viewModel.LoadData();

			LoadingView.IsVisible = false;


			if (_viewModel == null || _viewModel.DosingSchedule == null || !_viewModel.DosingSchedule.SentByPharmacy) return;

			// Sent by Pharmacy - Disable the edits
			DateInput.IsEnabled = TimeInput.IsEnabled = QuantityInput.IsEnabled = false;
			DateInput.Opacity = TimeInput.Opacity = QuantityInput.Opacity = 0.6;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnSuccess -= OnSuccess;
			_viewModel.OnError -= OnError;
		}

		void OnSuccess()
		{
			Navigation.PopAsync();
		}

		void OnError(string title, string message)
		{
			LoadingView.IsVisible = false;
			
			DisplayAlert(title, message, AppResources.OK);
		}

		async void CancelButtonClicked(object sender, EventArgs args)
		{
			await Navigation.PopAsync();
		}

		async void SaveButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			_viewModel.SaveDosage();
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
