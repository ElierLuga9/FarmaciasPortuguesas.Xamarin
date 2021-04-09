using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.DosageScheduler;
using ANFAPP.Pages.DosageScheduler.Drugs;
using ANFAPP.Pages.DosageScheduler.Options;
using ANFAPP.Utils;
using Xamarin.Forms;
using ANFAPP.Pages.Contacts;

namespace ANFAPP.Views
{
	public partial class ContactsTabbedBar : ContentView
    {

		public delegate Task OnNavigationStartedEventHandler();
		public enum SelectedTabEnum { Contacts, GeneralConditions, PrivacyPolicy, None };

		#region Bindable Properties

		public OnNavigationStartedEventHandler OnNavigationStarted;
		public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create<ContactsTabbedBar, SelectedTabEnum>(p => p.SelectedTab, SelectedTabEnum.None);

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

		public ContactsTabbedBar()
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
				case SelectedTabEnum.Contacts:
					// Set Button Background
					ContactsButton.TextColor = ColorResources.ANFWhite;
					ContactsButton.BackgroundColor = ColorResources.ANFDarkerBlue;

					GeneralConditionsButton.TextColor = ColorResources.ANFDarkGrey;
					GeneralConditionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					PrivacyPolicyButton.TextColor = ColorResources.ANFDarkGrey;
					PrivacyPolicyButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.GeneralConditions:
                    // Set Button Background
					GeneralConditionsButton.TextColor = ColorResources.ANFWhite;
					GeneralConditionsButton.BackgroundColor = ColorResources.ANFDarkerBlue;

					ContactsButton.TextColor = ColorResources.ANFDarkGrey;
					ContactsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					PrivacyPolicyButton.TextColor = ColorResources.ANFDarkGrey;
					PrivacyPolicyButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.PrivacyPolicy:
                    // Set Button Background
					PrivacyPolicyButton.TextColor = ColorResources.ANFWhite;
					PrivacyPolicyButton.BackgroundColor = ColorResources.ANFDarkerBlue;

					GeneralConditionsButton.TextColor = ColorResources.ANFDarkGrey;
					GeneralConditionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					ContactsButton.TextColor = ColorResources.ANFDarkGrey;
					ContactsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.None:
					// Set Button Background
					ContactsButton.BackgroundColor = ColorResources.ANFDarkerBlue;
					GeneralConditionsButton.BackgroundColor = ColorResources.ANFLighterBlue;
					PrivacyPolicyButton.BackgroundColor = ColorResources.ANFLighterBlue;

					break;
			}
		}

		#endregion

		#region Click Listeners

		async void OnContactsButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Contacts) 
			{

				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new ContactsPage(), Navigation);
			}
		}

		async void OnGeneralConditionsButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.GeneralConditions) 
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new GeneralConditionsPage(), Navigation);
			}
		}

		async void OnPrivacyPolicyButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.PrivacyPolicy) 
			{ 
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new PrivacyPolicyPage(), Navigation);
			}
		}

		#endregion

    }
}
