using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.DosageScheduler;
using ANFAPP.Pages.DosageScheduler.Drugs;
using ANFAPP.Pages.DosageScheduler.Options;
using ANFAPP.Utils;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class DosageSchedulerTabbedBar : ContentView
    {

		public delegate Task OnNavigationStartedEventHandler();
		public enum SelectedTabEnum { DosingSchedule, Medicine, Options, None };

		#region Constants

		public readonly string RESOURCE_DOSING_SCHEDULE_UNSELECTED = "ic_dosing_schedule_tab_unselected";
		public readonly string RESOURCE_DOSING_SCHEDULE_SELECTED = "ic_dosing_schedule_tab_selected";

		public readonly string RESOURCE_MEDICINE_UNSELECTED = "ic_medicine_tab_unselected";
		public readonly string RESOURCE_MEDICINE_SELECTED = "ic_medicine_tab_selected";

		public readonly string RESOURCE_OPTIONS_UNSELECTED = "ic_options_tab_unselected";
		public readonly string RESOURCE_OPTIONS_SELECTED = "ic_options_tab_selected";

		#endregion

		#region Bindable Properties

		public OnNavigationStartedEventHandler OnNavigationStarted;
		public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create<DosageSchedulerTabbedBar, SelectedTabEnum>(p => p.SelectedTab, SelectedTabEnum.None);

		#endregion

		#region Bindable Objects

		public SelectedTabEnum SelectedTab
		{
			get { return (SelectedTabEnum)GetValue(SelectedTabProperty); }
			set 
			{ 
				SetValue(SelectedTabProperty, value);
				SetSelectedTab(SelectedTab); 
			}
		}

		#endregion

		public DosageSchedulerTabbedBar()
		{
			InitializeComponent ();
        }

		#region Initializers

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (string.Equals(propertyName, "SelectedTab"))
			{
				SetSelectedTab(SelectedTab);
			}
		}

		/// <summary>
		/// Changes the selection state for all tabs.
		/// </summary>
		/// <param name="selectedTab"></param>
		public void SetSelectedTab(SelectedTabEnum selectedTab)
		{
			
			switch (selectedTab)
			{
				case SelectedTabEnum.DosingSchedule:
					// Set Button Background
                    DosingSchedulerButton.TextColor = ColorResources.ANFWhite;
                    DosingSchedulerButton.BackgroundColor = ColorResources.ANFDarkerBlue;

                    MedicineButton.TextColor = ColorResources.ANFDarkGrey;
					MedicineButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    OptionsButton.TextColor = ColorResources.ANFDarkGrey;
                    OptionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.Medicine:
                    // Set Button Background
                    MedicineButton.TextColor = ColorResources.ANFWhite;
                    MedicineButton.BackgroundColor = ColorResources.ANFDarkerBlue;

                    DosingSchedulerButton.TextColor = ColorResources.ANFDarkGrey;
                    DosingSchedulerButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    OptionsButton.TextColor = ColorResources.ANFDarkGrey;
                    OptionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.Options:
                    // Set Button Background
                    OptionsButton.TextColor = ColorResources.ANFWhite;
                    OptionsButton.BackgroundColor = ColorResources.ANFDarkerBlue;

                    DosingSchedulerButton.TextColor = ColorResources.ANFDarkGrey;
                    DosingSchedulerButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    MedicineButton.TextColor = ColorResources.ANFDarkGrey;
					MedicineButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.None:
					// Set Button Background
                    OptionsButton.BackgroundColor = ColorResources.ANFDarkerBlue;
					DosingSchedulerButton.BackgroundColor = ColorResources.ANFLighterBlue;
                    MedicineButton.BackgroundColor = ColorResources.ANFLighterBlue;

					break;
			}
		}

		#endregion

		#region Click Listeners

		async void OnDosingSchedulerButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.DosingSchedule) 
			{

				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new DosingSchedulePage(), Navigation);
			}
		}

		async void OnMedicineButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Medicine) 
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new ListDrugsPage(), Navigation);
			}
		}

		async void OnOptionsButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Options) 
			{ 
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new SchedulerOptionsPage(), Navigation);
			}
		}

		#endregion

    }
}
