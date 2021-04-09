using Xamarin.Forms;
using ANFAPP.Views;
using ANFAPP.Views.Common;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using System.Collections.Generic;
using ANFAPP.Logic;
using System;
using System.Threading.Tasks;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
    public partial class DrugDetailPage : ANFPage
    {
		private bool _shouldReload;
        private Medicine _drug;
        private DrugDetailViewModel _vm;

        private DrugDetailPage () : base() {}

        public DrugDetailPage(object drug) : base () {
            _drug = (Medicine)drug;
			_shouldReload = false;
        }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            _vm = new DrugDetailViewModel();
            BindingContext = _vm;
        }
            
        protected async override void OnAppearing()
        {
            base.OnAppearing ();

			if (_vm.Drug == null) {
				_vm.Drug = _drug;
			}

			if (_shouldReload) {
				await _vm.Reload ();
			} else {
				await _vm.LoadData ();
			}

			App.DosingScheduleVM.OnLoadStart += OnLoadStart;
			App.DosingScheduleVM.OnLoadComplete += OnLoadComplete;
        }

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.DosingScheduleVM.OnLoadStart -= OnLoadStart;
			App.DosingScheduleVM.OnLoadComplete -= OnLoadComplete;
		}

        #region Event Handlers

        async void OnAddScheduleButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync (new AddDosingSchedulePage (_vm.Drug));
        }

        async void OnEditMedicineButtonClicked(object sender, EventArgs args)
        {
			var page = new EditDrugDetailPage (_drug);
			await Navigation.PushAsync (page);
			_shouldReload = true;
        }

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async void OnLoadComplete() 
		{
			await _vm.Reload();
			LoadingView.IsVisible = false;
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
            return AppResources.DosageSchedulerTabMedicineLabel;
        }

        #endregion
    }
}

