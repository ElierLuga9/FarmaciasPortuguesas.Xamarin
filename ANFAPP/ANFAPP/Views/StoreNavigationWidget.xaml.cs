using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using ANFAPP.Views.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic;

namespace ANFAPP.Views
{
    public partial class StoreNavigationWidget : ContentView
    {

		#region Delegates & Enums

		public enum SelectedTabEnum { None, LandingPage, Categories, Catalog, Prescriptions };

		public delegate Task OnNavigationStartedEventHandler();

		#endregion

		#region Bindable Properties

		public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create<StoreNavigationWidget, SelectedTabEnum>(p => p.SelectedTab, SelectedTabEnum.None);
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

		#region Bindable Events

		public event OnNavigationStartedEventHandler OnNavigationStarted;

		#endregion

		public StoreNavigationWidget()
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
				case SelectedTabEnum.LandingPage:
					SetButtonSelectionState(LandingPageButton, true);
					SetButtonSelectionState(CategoriesButton, false);
					SetButtonSelectionState(CatalogButton, false);
					SetPrescriptionButtonState(false);
					break;
				case SelectedTabEnum.Categories:
					SetButtonSelectionState(LandingPageButton, false);
					SetButtonSelectionState(CategoriesButton, true);
					SetButtonSelectionState(CatalogButton, false);
					SetPrescriptionButtonState(false);
					break;
				case SelectedTabEnum.Catalog:
					SetButtonSelectionState(LandingPageButton, false);
					SetButtonSelectionState(CategoriesButton, false);
					SetButtonSelectionState(CatalogButton, true);
					SetPrescriptionButtonState(false);
					break;
				case SelectedTabEnum.Prescriptions:
					SetButtonSelectionState(LandingPageButton, false);
					SetButtonSelectionState(CategoriesButton, false);
					SetButtonSelectionState(CatalogButton, false);
					SetPrescriptionButtonState(true);
					break;
				default:
					SetButtonSelectionState(LandingPageButton, false);
					SetButtonSelectionState(CategoriesButton, false);
					SetButtonSelectionState(CatalogButton, false);
					SetPrescriptionButtonState(false);
					break;
			}
		}

		private void SetButtonSelectionState(Button button, bool selected)
		{
			button.BackgroundColor = selected ? ColorResources.ANFDarkOrange : ColorResources.LocatorSeparatorColorLight;
            button.TextColor = selected ? ColorResources.ANFWhite : ColorResources.ANFDarkGrey;
		}

		private void SetPrescriptionButtonState(bool selected)
		{
			PrescriptionButton.BackgroundColor = selected ? ColorResources.ANFGreen : ColorResources.APPBackgroundLight;
			PrescriptionButtonLabel.TextColor = selected ? ColorResources.APPBackgroundLight : ColorResources.ANFGreen;
			PrescriptionButtonImage.Source = selected ? "ic_prescription_selected.png" : "ic_prescription.png";
			PrescriptionButton.IsEnabled = !selected;
		}

		#endregion

		#region Button Event Handlers

		async void LandingPageButtonClicked(object sender, EventArgs args)
		{
			if (!NavigationUtils.TopPageOfType(typeof(StoreLandingPage), Navigation))
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new StoreLandingPage(), Navigation);
			}
		}

		async void CategoriesButtonClicked(object sender, EventArgs args)
		{
			if (!NavigationUtils.TopPageOfType(typeof(StoreCategoryPage), Navigation))
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new StoreCategoryPage(), Navigation);
			}
		}

		async void CatalogButtonClicked(object sender, EventArgs args)
		{
			if (!NavigationUtils.TopPageOfType(typeof(StorePointsCatalogPage), Navigation))
			{
				if (OnNavigationStarted != null) await OnNavigationStarted();

				await NavigationUtils.PushPageWithNoHistory(new StorePointsCatalogPage(), Navigation);
			}
		}

		async void PrescriptionButtonClicked(object sender, EventArgs args)
		{
			if (!NavigationUtils.TopPageOfType(typeof(StorePrescriptionPage), Navigation))
			{
				if (SessionData.StorePharmacyId.Equals (Settings.ST_MG_PHARMACY_ID_DEFAULT)) {
					await ((ContentPage)this.ParentView.ParentView).DisplayAlert("Receita", "Selecione a Sua Farmácia para usar esta funcionalidade", AppResources.OK);
				} else {
					if (OnNavigationStarted != null) await OnNavigationStarted();

					await NavigationUtils.PushPageWithNoHistory(new StorePrescriptionPage(), Navigation);
				}
			}
		}

		#endregion

    }
}

