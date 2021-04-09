using ANFAPP.Logic.ViewModels;

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ANFAPP.Views
{
	public partial class PromotionsHilightItem : ContentPage
	{
		public PromotionHighlightsViewModel _viewModel;
		public PromotionsHilightItem()
		{
			InitializeComponent();
			BindingContext = _viewModel = new PromotionHighlightsViewModel();
			_viewModel.LoadData();
		}
	}
}

