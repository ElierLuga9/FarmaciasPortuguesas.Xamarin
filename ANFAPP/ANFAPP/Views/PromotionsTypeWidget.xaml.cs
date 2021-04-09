using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Views
{
    public partial class PromotionsTypeWidget : ContentView
    {
		public event EventHandler PromotionSelect;
		void Promoselect(object sender, EventArgs args)
		{
			if (PromotionSelect != null) PromotionSelect(sender, args);
		}
		public event EventHandler OnPromoTapped;
		void PromoTapped(object sender, EventArgs args)
		{
			if (OnPromoTapped != null) OnPromoTapped(sender, args);
		}

		public PromotionsViewModel _viewModel ;

        public PromotionsTypeWidget()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new PromotionsViewModel();
            _viewModel.LoadList(false);

        }

        public void OnArticleTapped(object sender, EventArgs args)
        {
			
            //if (sender == null || !(sender is View)) return;

            //// Show Loading
            //LoadingView.IsVisible = true;

            //HighlightOut p = (sender as View).BindingContext as HighlightOut;

            //var page = new ArticlesListDetailPage(p.Id);
            //await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
            //await Navigation.PushAsync(page);
        }
        
    }
}
