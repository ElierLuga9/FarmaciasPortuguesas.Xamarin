using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.Vouchers
{
    public partial class VouchersHistoryPage : ANFPage
    {

        #region Properties

        private bool _isInitialized = false;
        public VouchersHistoryViewModel _viewModel { get; set; }

        #endregion

        #region Page Initialization

        public VouchersHistoryPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            // Initialize context
            this.BindingContext = _viewModel = new VouchersHistoryViewModel();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_isInitialized)
            {
                _viewModel.LoadData();
                _isInitialized = true;
            }
        }

		void VoucherItemClick(object sender, EventArgs args)
		{
			Navigation.PushAsync(new VouchersDetailPage(((Grid)sender).BindingContext as Voucher));
		}

        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.VouchersHistoryPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
