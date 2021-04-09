using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.ViewModels;

using ANFAPP.Pages.UserArea.Vouchers;

using ANFAPP.Views;

using Xamarin.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ANFAPP.Views
{
    public partial class PromotionsVouchersValueListItem : ContentView
    {
       

        #region Page Initialization
		public event EventHandler PromoVoucherSelect;
		void VoucherSelect(object sender, EventArgs args)
		{
			if (PromoVoucherSelect != null) PromoVoucherSelect(sender, args);
		}
		public event EventHandler VoucherTapped;
		void OnVoucherTapped(object sender, EventArgs args)
		{
			if (VoucherTapped != null) VoucherTapped(sender, args);
		}

        public PromotionsVouchersValueListItem()
        {
            InitializeComponent();

        }

        #endregion

    }
}
