using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ANFAPP.Logic.ViewModels;
using System.Threading.Tasks;
using ANFAPP.Logic;

namespace ANFAPP.Views
{
    public partial class HighlightsWidget : ContentView
    {
		private HighlightsViewModel _viewModel;

		public delegate Task OnTaskStartedEventHandler();
		public event EventHandler OnHeaderClicked;
		public event OnTaskStartedEventHandler OnProductButtonClicked;

		#region Bindable Properties

		public static readonly BindableProperty TitleProperty = BindableProperty.Create<HighlightsWidget, string>(p => p.Title, null);
        
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly BindableProperty FromCatalogProperty = BindableProperty.Create<HighlightsWidget, bool>(p => p.FromCatalog, false);
        
		public bool FromCatalog
		{ 
			get { return (bool)GetValue(FromCatalogProperty); }
			set { SetValue(FromCatalogProperty, value); }
		}

		#endregion

		#region

        public HighlightsWidget ()
        {
            InitializeComponent ();

			Widget1.FromCatalog = FromCatalog;
			Widget2.FromCatalog = FromCatalog;
			Widget1.OnTaskStarted += OnAddToCartClicked;
			Widget2.OnTaskStarted += OnAddToCartClicked;


        }

		protected override void OnParentSet()
		{
			base.OnParentSet();

			if (Parent != null)
			{
				_viewModel = new HighlightsViewModel (FromCatalog, Title, 2, false);
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

		#endregion
    }
}
