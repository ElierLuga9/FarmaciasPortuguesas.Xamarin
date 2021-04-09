using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.UserLogin;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
    public partial class DrugPickerPage : ANFPage
    {

		#region Properties

		private AddDosingScheduleViewModel _viewModel = new AddDosingScheduleViewModel();

		#endregion

        #region Page Initialization

		public DrugPickerPage(AddDosingScheduleViewModel viewModel) : base() 
		{
			BindingContext = _viewModel = viewModel; 
		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();
        }

        #endregion

		#region Event Handlers

		protected override void OnAppearing()
		{
			base.OnAppearing();

			LoadingView.IsVisible = false;
		}

		void OnListItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem == null) return;

			_viewModel.Medicine = args.SelectedItem as Medicine;
			Navigation.PopAsync();
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
