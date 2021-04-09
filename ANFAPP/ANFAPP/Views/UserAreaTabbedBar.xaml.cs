using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.UserArea;
using ANFAPP.Utils;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class UserAreaTabbedBar : ContentView
	{

		public delegate Task OnNavigationStartedEventHandler();
		public enum SelectedTabEnum { PersonalData, History, None };

		#region Bindable Properties

		public OnNavigationStartedEventHandler OnNavigationStarted;
		public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create<UserAreaTabbedBar, SelectedTabEnum>(p => p.SelectedTab, SelectedTabEnum.None);

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

		public UserAreaTabbedBar()
		{
			InitializeComponent();
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
				case SelectedTabEnum.PersonalData:
					// Set Button Background
					PersonalDataButton.TextColor = ColorResources.ANFWhite;
					PersonalDataButton.BackgroundColor = ColorResources.ANFDarkerBlue;
					HistoryButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;


					break;
				case SelectedTabEnum.History:
					// Set Button Background

					HistoryButton.BackgroundColor = ColorResources.ANFDarkerBlue;
					HistoryButton.TextColor = ColorResources.ANFWhite;
					PersonalDataButton.TextColor = ColorResources.ANFDarkGrey;
					PersonalDataButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;


					break;

				case SelectedTabEnum.None:
					// Set Button Background
					HistoryButton.BackgroundColor = ColorResources.ANFDarkerBlue;
					PersonalDataButton.BackgroundColor = ColorResources.ANFLighterBlue;

					break;
			}
		}

		#endregion

		#region Click Listeners

		async void OnPersonalDataButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.PersonalData)
			{

				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new UserAreaMainPage(), Navigation);
			}
		}

		async void OnHistoryButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.History)
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new UserOrdersPage(), Navigation);
			}
		}

		#endregion

	}
}