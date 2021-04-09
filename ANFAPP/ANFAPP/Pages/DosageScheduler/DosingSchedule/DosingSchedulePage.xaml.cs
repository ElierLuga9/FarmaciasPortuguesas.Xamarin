using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.DosageScheduler.Drugs;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Resources;
using ANFAPP.Logic.Utils;

namespace ANFAPP.Pages.DosageScheduler
{
    public partial class DosingSchedulePage : ANFPage
    {
		#region Properties

		private bool _initialized = false;
		private bool _shouldSynchronize = false;
		private string _contextId = null;
		private bool _dosageAlert;

		#endregion

        #region Page Initialization

		public DosingSchedulePage() : this(false) { }

		public DosingSchedulePage(string contextId, bool dosageAlert = true)
			: this(true)
		{
			_contextId = contextId;
			_dosageAlert = dosageAlert;
		}

		public DosingSchedulePage(bool synchronize)
			: base()
		{
			_shouldSynchronize = synchronize;
		}

        protected override void InitPage()
        {
            InitializeComponent();
      
            base.InitPage();

			BigAdd.IsVisible = false;
			AddSection.IsVisible = false;

            //var mixpanelWidget = DependencyService.Get<IMixPanel>();
            //var props = new Dictionary<string, string>();
            //props.Add("IsLoggedIn", SessionData.IsLogged ? "true" : "false");
            //mixpanelWidget.TrackProperties("MedicationAlerts", props);
        }

        #endregion

		#region Event Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();

			TabBar.OnNavigationStarted += OnNavigationStarted;

			BindingContext = App.DosingScheduleVM;
			App.DosingScheduleVM.OnLoadComplete += OnLoadComplete;
			App.DosingScheduleVM.OnLoadStart += OnLoadStarted;
			
			// Load Data
			LoadingView.IsVisible = true;

			if (!_initialized) {
				// IsVisible is changed (in code) upon initialization, so we need to reset the binding.
				AddSection.SetBinding (Grid.IsVisibleProperty, new Binding ("DosingSchedules", BindingMode.Default, ConverterResources.HasElements));
			}

			App.DosingScheduleVM.LoadData(_shouldSynchronize);

			if (!_initialized) {
				// This binding should be set after LoadData().
				BigAdd.SetBinding (Grid.IsVisibleProperty, new Binding ("DosingSchedules", BindingMode.Default, ConverterResources.EmptyOrNull));

				_initialized = true;
			}

			// Only synchronize once
			_shouldSynchronize = false;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.DosingScheduleVM.OnLoadComplete -= OnLoadComplete;
			App.DosingScheduleVM.OnLoadStart -= OnLoadStarted;

			TabBar.OnNavigationStarted -= OnNavigationStarted;
		}

		async Task OnNavigationStarted()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async Task OnLoadStarted()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadComplete() 
		{
			if (string.IsNullOrEmpty(_contextId))
			{
				LoadingView.IsVisible = false;
			}
			else
			{
				if (_dosageAlert) 
				{ 
					GoToDosagePage();
				}
				else
				{
					GoToMedPage();
				}
			}
		}

		private async void GoToDosagePage()
		{
			var dosage = await App.DosingScheduleVM.GetDosage(_contextId);
			_contextId = null;

			if (dosage != null)
			{
				var schedule = await App.DosingScheduleVM.GetDosingSchedule(dosage);
				if (schedule != null) 
				{
					// Go to the schedule detail page
					await Navigation.PushAsync(new DosingScheduleDetailPage(schedule));
					//await Navigation.PushAsync(new DosageEditPage(schedule, dosage));
					return;
				}
			}

			LoadingView.IsVisible = false;
		}

		private async void GoToMedPage()
		{
			var medicine = await App.ListDrugsVM.GetMedicine(_contextId);
			_contextId = null;

			if (medicine != null)
			{
				// Go to the dosage page
				await Navigation.PushAsync(new DrugDetailPage(medicine));
				return;
			}

			LoadingView.IsVisible = false;
		}

		async void OnListItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null) return;
			
			var selectedItem = args.SelectedItem as DosingSchedule;
			DosingScheduleList.SelectedItem = null;

			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new DosingScheduleDetailPage(selectedItem));
		}

		#endregion

		#region Button Events

		async void OnAddDosingScheduleButtonClick(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			
			await Navigation.PushAsync(new AddDosingSchedulePage());
		}

		#endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

        protected override string GetAppBarTitle()
        {
			return AppResources.DosageSchedulerPageTitle;
        }

        #endregion

    }
}
