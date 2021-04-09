using System;
using Acr.BarCodes;
using ANFAPP.Resources;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
    public partial class ListDrugsPage : ANFPage
    {
        private bool _isInitialized = false;

        #region Page Initialization

        public ListDrugsPage () : base() {}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BigAdd.IsVisible = false;
			AddSection.IsVisible = false;
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

        #region Event Handlers

        async void ReadBarcodeButtonClicked(object sender, EventArgs args)
        {
            var result = await BarCodes.Instance.Read();
            if (result.Success) {
                //var msg = String.Format("Barcode Found.  Type: {0} - Code: {1}", result.Format, result.Code);
                //System.Diagnostics.Debug.WriteLine (msg);
				await Navigation.PushAsync(new AddDrugPage(result.Code));
            }
        }

        async void AddDrugButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync (new AddDrugPage ());
        }

        async void OnDrugSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var page = new DrugDetailPage (args.SelectedItem);

            await Navigation.PushAsync (page);
        }

        #endregion

        async Task OnLoadStarted()
        {
            LoadingView.IsVisible = true;
            await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        void OnLoadComplete() 
        {
            LoadingView.IsVisible = false;  
        }

        void OnError(string title, string message)
        {
            LoadingView.IsVisible = false;

            DisplayAlert(title, message, AppResources.OK);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing ();

            TabBar.SelectedTab = DosageSchedulerTabbedBar.SelectedTabEnum.Medicine;

			App.ListDrugsVM.OnLoadComplete += OnLoadComplete;
			App.ListDrugsVM.OnLoadStart += OnLoadStarted;
			App.ListDrugsVM.OnError += OnError;

            if (!_isInitialized)
            {
                // Initialize ViewModel
                BindingContext = App.ListDrugsVM;

				// IsVisible is changed (in code) upon initialization, so we need to reset the binding.
				AddSection.SetBinding (Grid.IsVisibleProperty, new Binding ("Drugs", BindingMode.Default, ConverterResources.HasElements));

                await App.ListDrugsVM.LoadData();
                _isInitialized = true;

				// This binding should be set after LoadData().
				BigAdd.SetBinding (Grid.IsVisibleProperty, new Binding ("Drugs", BindingMode.Default, ConverterResources.EmptyOrNull));
            }
            else
            {
                await App.ListDrugsVM.LoadData();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.ListDrugsVM.OnLoadComplete -= OnLoadComplete;
            App.ListDrugsVM.OnLoadStart -= OnLoadStarted;
            App.ListDrugsVM.OnError -= OnError;

            LoadingView.IsVisible = false;
        }
    }
}
