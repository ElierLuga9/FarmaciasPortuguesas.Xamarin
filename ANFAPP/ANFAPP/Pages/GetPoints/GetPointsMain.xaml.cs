using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages.GetPoints;
using ANFAPP.Utils;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic.Resources;
using ANFAPP.Pages.UserArea;

namespace ANFAPP.Pages.GetPoints
{
	public partial class GetPointsMain : ANFPage
	{

		#region Page Initialization

		private bool _initialized;

		public GetPointsMain() : base() 
		{ 
			BindingContext = App.GameCenterVM;
		}

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}

		#endregion

		#region Lifecycle Events

		protected override void OnAppearing()
		{
			base.OnAppearing();

			App.GameCenterVM.OnLoadSuccess += OnLoadSuccess;
			App.GameCenterVM.OnLoadError += OnLoadError;
			App.GameCenterVM.OnLoadStart += OnLoadStart;

			if (!_initialized) {
				App.GameCenterVM.LoadData ();
				_initialized = true;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			App.GameCenterVM.OnLoadSuccess -= OnLoadSuccess;
			App.GameCenterVM.OnLoadError -= OnLoadError;
			App.GameCenterVM.OnLoadStart -= OnLoadStart;
		}

		#endregion

		#region Event Handlers

		void OnLoadSuccess()
		{
			LoadingView.IsVisible = false;
		}

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			DisplayAlert(title, message, AppResources.OK);
		}

		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnItemSelected(object sender, EventArgs args)
		{
			// Remove selection
			AchievementList.SelectedItem = null;
		}

		async void OnJoinSaudaButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await Navigation.PushAsync(new UserNotLoggedCardPage());
		}

        async void OnFAQButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new GetPointsFAQ());
        }

		async void OnActivateFitnessDataClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			await App.GameCenterVM.LoadData(true);
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

        //protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        //{
        //    return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU;
        //}

		protected override string GetAppBarTitle()
		{
			return AppResources.GetPointsTitle;
		}

		#endregion

	}
}


