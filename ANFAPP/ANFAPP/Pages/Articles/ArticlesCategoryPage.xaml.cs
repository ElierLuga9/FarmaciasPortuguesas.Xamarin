using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out.Articles;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
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

namespace ANFAPP.Pages.Articles
{
    public partial class ArticlesCategoryPage : ANFPage
    {
        #region Properties

        private ArticlesCategoryViewModel _viewModel = new ArticlesCategoryViewModel();
		private bool _initialized = false;

        #endregion

        #region Page Initialization

        public ArticlesCategoryPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            BindingContext = _viewModel;
        }

        #endregion

        #region Tap Actions

        async void OnListItemTapped(object sender, EventArgs args)
        {
            SectionsOut item = ((View)sender).BindingContext as SectionsOut;

            if (item != null)
            {
                if (item.Id != -2)
                {
                    if (item.Id == 32) {
                        // XXX: Requested for this specific category to open a WebView.
                        await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                        await Navigation.PushAsync(new ArticleWebView("https://bebe-mama.farmaciasportuguesas.pt"));
                        return;
                    }

                    if (item.LastSection)
                    {
                       // LoadingView.IsVisible = true;
                        await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
                        await Navigation.PushAsync(new ArticlesSearchResult(new ArticlesSearchViewModel("", item.Id, item.Name)));

                    }
                    else
                    {
						await _viewModel.SetCategory(item);
                    }
                }
                else 
                {
					LoadingView.IsVisible = true;
                    await Task.Delay(Settings.DEFAULT_LOADING_DELAY);					
					await NavigationUtils.PushPageAndClearHistory(new ArticlesMainPage(), Navigation);
                }
            }
        }

		public async void OnArticleTapped(object sender, EventArgs args)
		{
			if (sender == null || !(sender is View)) return;

			// Show Loading
			LoadingView.IsVisible = true;

			HighlightOut p = (sender as View).BindingContext as HighlightOut;

			var page = new ArticlesListDetailPage(p.Id);
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PushAsync(page);
		}

		public async void OnSeeAllButtonClicked(object sender, EventArgs args)
		{
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PushAsync(new ArticlesSearchResult(new ArticlesSearchViewModel("", _viewModel.Current.Id, _viewModel.Current.Name)));
		}



        #endregion

		#region Event Handlers

        protected override void OnAppearing()
        {
            base.OnAppearing();

			ApplicationBar.SetLeftButtonEventHandler(OnBackTapped);

            _viewModel.OnLoadStart += OnStart;
            _viewModel.OnError += OnError;
            _viewModel.OnSuccess += OnSuccess;
			LoadingView.IsVisible = false;
			if (!_initialized) {
				LoadingView.IsVisible = true;
				_viewModel.SetCategory();

				_initialized = true;
			}
        }

        protected override void OnDisappearing()
        {
			ApplicationBar.UnsetLeftButtonEventHandler(OnBackTapped);

            base.OnDisappearing();

            _viewModel.OnLoadStart -= OnStart;
            _viewModel.OnError -= OnError;
            _viewModel.OnSuccess -= OnSuccess;
        }

        async Task OnStart()
        {
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
        }

        async void OnBackTapped(object sender, EventArgs args)
        {
            int count = await _viewModel.PopCategory();

			if (count == 0) {
				if (Navigation.NavigationStack.Count > 1) Navigation.PopAsync(true);
			}
        }

        void OnSuccess()
        {
            LoadingView.IsVisible = false;

      /*      if (_viewModel.NavCount == 0)
            {
                ApplicationBar.UnsetLeftButtonEventHandler(OnBackTapped);
                ApplicationBar.SetLeftButtonType(ApplicationBar.APPBAR_LEFTBUTTON_TYPE.MENU);
            }
            else
            {
                ApplicationBar.SetLeftButtonType(ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK);
                ApplicationBar.SetLeftButtonEventHandler(OnBackTapped);
            }*/


        }

        void OnError(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.ArticlesPageTitle;
		}


		#endregion
    }
}
