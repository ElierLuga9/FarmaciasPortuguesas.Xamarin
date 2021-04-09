using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Pages;
using ANFAPP.Pages.Store;
using ANFAPP.Utils;

using Plugin.Connectivity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Views
{
    public partial class PromotionsHighlightsWidget : ContentView
    {
        private HighlightsViewModel _viewModel;

		public delegate Task OnTaskStartedEventHandler();
		public event EventHandler OnHeaderClicked;
		public event OnTaskStartedEventHandler OnProductButtonClicked;

		#region Bindable Properties

        public static readonly BindableProperty TitleProperty = BindableProperty.Create<PromotionsHighlightsWidget, string>(p => p.Title, null);
        
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

        public static readonly BindableProperty FromCatalogProperty = BindableProperty.Create<PromotionsHighlightsWidget, bool>(p => p.FromCatalog, false);
        
		public bool FromCatalog
		{ 
			get { return (bool)GetValue(FromCatalogProperty); }
			set { SetValue(FromCatalogProperty, value); }
		}

		#endregion

		#region

        public PromotionsHighlightsWidget()
        {
            InitializeComponent ();

			Widget1.FromCatalog = FromCatalog;
			Widget2.FromCatalog = FromCatalog;
            Widget3.FromCatalog = FromCatalog;
            Widget4.FromCatalog = FromCatalog;
			Widget1.OnTaskStarted += OnAddToCartClicked;
			Widget2.OnTaskStarted += OnAddToCartClicked;
            Widget3.OnTaskStarted += OnAddToCartClicked;
            Widget4.OnTaskStarted += OnAddToCartClicked;

        }

		protected override void OnParentSet()
		{
			base.OnParentSet();

			if (Parent != null)
			{
				_viewModel = new HighlightsViewModel (FromCatalog, Title, 4, false);
				BindingContext = _viewModel;	
			}
		}
			
		public void LoadData() 
		{
			_viewModel.LoadData ();
		}

		async void OnHeaderButtonClicked(object sender, EventArgs args)
		{
			if (OnHeaderClicked != null) OnHeaderClicked(sender, args);
		}

		async Task OnAddToCartClicked()
		{
			if (OnProductButtonClicked != null) await OnProductButtonClicked();
		}

        private void ShowConnectionError()
        {

            NavigationUtils.GetPageOnTop(Navigation).DisplayAlert(null, AppResources.NetworkingErrorMenuMessage, AppResources.OK);
        }

        async void OnSeeAllClicked(object sender, EventArgs args)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                ShowConnectionError();
                return;
            }

            // Navigate only if not already there.
            if (NavigationUtils.TopPageOfType(typeof(StoreHighlightsPage), Navigation)) return;
            await Navigation.PushAsync(new StoreHighlightsPage(false, AppResources.StoreHighlightsLabel));
        }
		#endregion
    }
}
