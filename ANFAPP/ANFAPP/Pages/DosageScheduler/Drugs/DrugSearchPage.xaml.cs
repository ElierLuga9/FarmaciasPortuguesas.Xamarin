using System;
using System.Threading.Tasks;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Out;
using Xamarin.Forms;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Exceptions;

namespace ANFAPP.Pages.DosageScheduler.Drugs
{
	public partial class DrugSearchPage : ANFPage
	{

		private bool _isInitialized = false;
		private string _query;
		private bool _isCNP;

		private List<ProductOut> _queryResults;
		public List<ProductOut> QueryResults
		{
			get { return _queryResults; }
			set
			{
				_queryResults = value;
				OnPropertyChanged("QueryResults");
			}
		}

		private bool HasResults
		{
			get { return (QueryResults != null && QueryResults.Count > 0); }
		}

		private DrugSearchPage() : base() { }

		public DrugSearchPage(string query)
			: base()
		{
			_query = query;
			_isCNP = false;
		}

		public DrugSearchPage(string query, bool isCNP)
			: base()
		{
			_query = query;
			_isCNP = isCNP;
		}

		public event EventHandler OnDrugSelected;

		#region Page Initialization

		protected override void InitPage()
		{
			InitializeComponent();
			base.InitPage();
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			return AppResources.DosageSchedulerPageTitle;
		}

		protected override string GetAppBarSubtitle()
		{
			return AppResources.DosageSchedulerTabMedicineLabel;
		}

		#endregion

		#region Events

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			BindingContext = this;
			QueryString.Text = string.Format("\"{0}\"", _query.ToUpper());

			await OnLoadStarted();

			await results();
			_isInitialized = true;

			NoResults.IsVisible = !HasResults;
			OnLoadComplete();
		}
		public async Task results()
		{
			try
			{
				// Here we should search the entire catalog, thus we use the default pharmacy.
				// http://issue.innovagency.com/view.php?id=20949
				var results = await ECommerceWS.Search(SessionData.UserAuthentication, SessionData.StorePharmacyId, 0, _query, 0, null, 0,
					null, null, null, null, null, null, null, false, true);
				QueryResults = results.Products;
			}
			catch (Exception e)
			{
				string message = e.Message;
				if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
				DisplayAlert("", message, AppResources.OK);
			}
		}
		async Task OnLoadStarted()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		void OnLoadComplete()
		{
			LoadingView.IsVisible = false;
		}

		async void OnDrugItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var item = args.SelectedItem;
			if (null != OnDrugSelected)
			{
				OnDrugSelected(item, null);
			}
			await Navigation.PopAsync();
		}

		#endregion
	}
}