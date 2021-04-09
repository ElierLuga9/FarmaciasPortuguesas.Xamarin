using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea
{
    public partial class PointsHistoryPage : ANFPage
    {

        #region Properties

        public PointsHistoryViewModel _viewModel { get; set; }

        #endregion

        #region Page Initialization

        public PointsHistoryPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize context
            this.BindingContext = _viewModel = new PointsHistoryViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnLoadError += OnLoadError;
            _viewModel.OnLoadFinished += OnLoadFinished;

            _viewModel.LoadData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnLoadError -= OnLoadError;
            _viewModel.OnLoadFinished -= OnLoadFinished;
        }

        #endregion

        public void OnListSelection_EventHandler(object sender, SelectedItemChangedEventArgs e) {
            // Disable selection
            if (e.SelectedItem == null) return;
            ((ListView)sender).SelectedItem = null;
        }

        #region ViewModel Events

        void OnLoadFinished()
        {
            // Hide Loading
            LoadingView.IsVisible = false;
        }

        void OnLoadError(string title, string message)
        {
            // Hide Loading
            DisplayAlert(title, message, AppResources.OK);
            LoadingView.IsVisible = false;
        }

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.PointsHistoryPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
