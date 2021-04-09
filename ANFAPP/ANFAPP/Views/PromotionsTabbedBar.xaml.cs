using System;
using System.Threading.Tasks;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages;
using ANFAPP.Pages.DosageScheduler;
using ANFAPP.Pages.DosageScheduler.Drugs;
using ANFAPP.Pages.DosageScheduler.Options;
using ANFAPP.Utils;
using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class PromotionsTabbedBar : ContentView
    {

		public delegate Task OnNavigationStartedEventHandler();
		public enum SelectedTabEnum { Promotions, Vouchers, Products, None };

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
		public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create<PromotionsTabbedBar, SelectedTabEnum>(p => p.SelectedTab, SelectedTabEnum.None);

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

		public PromotionsTabbedBar()
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
				case SelectedTabEnum.Promotions:
					// Set Button Background
                    PromotionsButton.TextColor = ColorResources.ANFWhite;
					PromotionsButton.BackgroundColor = ColorResources.ANFDarkOrange;

                    VouchersButton.TextColor = ColorResources.ANFDarkGrey;
					VouchersButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    ProductsButton.TextColor = ColorResources.ANFDarkGrey;
                    ProductsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.Vouchers:
                    // Set Button Background
                    VouchersButton.TextColor = ColorResources.ANFWhite;
                    VouchersButton.BackgroundColor = ColorResources.ANFDarkOrange;

                    PromotionsButton.TextColor = ColorResources.ANFDarkGrey;
                    PromotionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    ProductsButton.TextColor = ColorResources.ANFDarkGrey;
                    ProductsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.Products:
                    // Set Button Background
                    ProductsButton.TextColor = ColorResources.ANFWhite;
                    ProductsButton.BackgroundColor = ColorResources.ANFDarkOrange;

                    PromotionsButton.TextColor = ColorResources.ANFDarkGrey;
                    PromotionsButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

                    VouchersButton.TextColor = ColorResources.ANFDarkGrey;
					VouchersButton.BackgroundColor = ColorResources.SchedulerTabUnselectedColor;

					break;
				case SelectedTabEnum.None:
					// Set Button Background
                    ProductsButton.BackgroundColor = ColorResources.ANFDarkOrange;
					PromotionsButton.BackgroundColor = ColorResources.ANFLighterBlue;
                    VouchersButton.BackgroundColor = ColorResources.ANFLighterBlue;

					break;
			}
		}

		#endregion

		#region Click Listeners

		async void OnPromotionsButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Promotions) 
			{

				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new PromotionsPage(), Navigation);
			}
		}

		async void OnVouchersButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Vouchers) 
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new PromotionPageVouchers(), Navigation);
			}
		}

		async void onProductButtonClicked(object sender, EventArgs args)
		{
			if (SelectedTab != SelectedTabEnum.Products) 
			{ 
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new PromotionsPageProducts(), Navigation);
			}
		}

		#endregion

    }
}
