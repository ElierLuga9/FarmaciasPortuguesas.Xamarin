using ANFAPP.Logic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.Store;
using ANFAPP.Pages.UserArea.Vouchers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Views
{
    public partial class PromotionsTypeListItem : ContentView
    {
        #region Bindable Objects

        public PromotionsTypeListItemViewModel _viewModel = new PromotionsTypeListItemViewModel();
        public PromotionsOut _promotions = new PromotionsOut();
        
        public event EventHandler PromotionSelect;


        #endregion

        public PromotionsTypeListItem()
        {
            InitializeComponent();
			//	BindingContext = _viewModel;
			_viewModel.LoadData(_promotions);
        }


        void OnPromotionSelect(object sender, EventArgs args)
        {
            if (PromotionSelect != null) PromotionSelect(BindingContext, args);
        }
    }

}
