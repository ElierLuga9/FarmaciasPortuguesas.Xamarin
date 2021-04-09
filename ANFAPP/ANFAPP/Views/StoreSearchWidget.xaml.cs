using Acr.BarCodes;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Pages.Store;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic;

namespace ANFAPP.Views
{
    public partial class StoreSearchWidget : ContentView
    {

		#region Delegates & Constants

		public static double ENABLED_OPACITY_VALUE = 1.0;
		public static double DISABLED_OPACITY_VALUE = 0.6;

		public delegate void SearchParamsRefreshedEvent(string searchValue);

		#endregion

		#region Events

		public event SearchParamsRefreshedEvent SearchParamsRefreshed;
		public event EventHandler FilterButtonClickedEvent;
		public event EventHandler OrderButtonClickedEvent;

		#endregion

		#region Bindable Properties

		public static readonly BindableProperty FiltersEnabledProperty = BindableProperty.Create<StoreSearchWidget, bool>(p => p.FiltersEnabled, true);
		public bool FiltersEnabled
		{
			get { return (bool)GetValue(FiltersEnabledProperty); }
			set { SetValue(FiltersEnabledProperty, value); }
		}

		public static readonly BindableProperty SearchValueProperty = BindableProperty.Create<StoreSearchWidget, string>(p => p.SearchValue, null);
		public string SearchValue
		{
			get { return (string)GetValue(SearchValueProperty); }
			set { SetValue(SearchValueProperty, value); }
		}

		#endregion

		public StoreSearchWidget()
        {
            InitializeComponent ();
			MainContainer.BindingContext = this;
        }

		#region Search Functions

		public void OnSearchBoxInputSubmit(object sender, EventArgs args)
		{
			PerformSearch();
		}

		/// <summary>
		/// Perform a search
		/// </summary>
		protected void PerformSearch()
		{
			if (SearchParamsRefreshed != null) SearchParamsRefreshed(SearchValue);
		}

		#endregion

		#region Button Events

		protected async void ReadBarcodeButtonClicked(object sender, EventArgs args)
		{
			var result = await BarCodes.Instance.Read();
			if (result.Success)
			{

				try
				{
					int barCode = int.Parse(result.Code);
					await Navigation.PushAsync(new StoreProductDetailPage(barCode));
				}
				catch (Exception e) 
				{
					System.Diagnostics.Debug.WriteLine(e.Message.ToString());
					await App.Current.MainPage.DisplayAlert(null,"O Código lido não é válido",AppResources.OK);
				}

				//SearchValue = result.Code;
				//PerformSearch();
			}
		}

        //testes
        protected void FilterButtonClicked(object sender, EventArgs args)
        {
            if (FilterButtonClickedEvent != null) FilterButtonClickedEvent(sender, args);
            Navigation.PushAsync(new StoreSearchFilterPage());
        }

        protected void OrderButtonClicked(object sender, EventArgs args)
        {
            if (OrderButtonClickedEvent != null) OrderButtonClickedEvent(sender, args);
            Navigation.PushAsync(new StoreSearchOrderPage());
        }

		#endregion

		#region Element State Modifiers

		protected void SetSearchBoxState(bool state)
		{
			SetViewState(SearchBox, state);
		}


        //testes
        //protected void SetFilterButtonState(bool state)
        //{
        //    SetViewState(FilterButton, state);
        //}

        //protected void SetOrderButtonState(bool state)
        //{
        //    SetViewState(OrderButton, state);
        //}



		/// <summary>
		/// Changes a view visual state.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="state"></param>
		protected void SetViewState(View view, bool state)
		{
			view.IsEnabled = state;
			view.Opacity = state ? ENABLED_OPACITY_VALUE : DISABLED_OPACITY_VALUE; 
		}

		#endregion

    }
}

