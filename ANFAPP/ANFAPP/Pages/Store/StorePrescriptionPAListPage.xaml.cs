using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Pages;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Views;

namespace ANFAPP.Pages.Store
{
	public partial class StorePrescriptionPAListPage : ANFPage
	{
		private StorePrescriptionViewModel _context;

		public StorePrescriptionPAListPage (StorePrescriptionViewModel context)
		{
			_context = context;
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();

			BindingContext = _context;

			//ApplicationBar.HideUserButton ();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			BindingContext = _context;

			LoadingView.IsVisible = false;
		}

		async void OnItemSelected (object sender, SelectedItemChangedEventArgs args)
		{
			var item = args.SelectedItem as PAListItem;
			PAList.SelectedItem = null;
			if (null != item) 
			{
				await Navigation.PushAsync (new StoreSelectPAPage (_context, item));
			}
		}

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.StorePrescriptionPATitle;
		}

		protected override string GetAppBarSubtitle()
		{
			return AppResources.StorePrescriptionPASubtitle;
		}

		#endregion
	}
}

