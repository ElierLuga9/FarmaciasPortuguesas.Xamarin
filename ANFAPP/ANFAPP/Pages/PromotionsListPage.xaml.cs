using System;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;

namespace ANFAPP.Pages
{
	public partial class PromotionsListPage : ANFPage
	{
		public PromotionsListViewModel _viewModel;
		public List<Product> _cnpList = new List<Product>();

		public PromotionsListPage(List<Product> cnpList ) : base()
		{
			_cnpList = cnpList;
			_viewModel = new PromotionsListViewModel(_cnpList);


		}

		public PromotionsListPage()
		{
			InitializeComponent();
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();


		}




		protected override void OnAppearing()
		{
			base.OnAppearing();
			_viewModel.Load();

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();


		}
	}
}

