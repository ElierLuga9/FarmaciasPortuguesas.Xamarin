using System;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using ANFAPP.Logic.Network.Services;


namespace ANFAPP.Pages.UserArea
{
	public partial class EditUserDataPage : ANFPage
	{

		#region Page Initialization

		private EditUserDataViewModel _viewModel;

		public EditUserDataPage () : base ()
		{
		}

		protected override void InitPage ()
		{
			_viewModel = new EditUserDataViewModel ();
			InitializeComponent ();
			base.InitPage ();
		}

		#endregion

		/// <summary>
		/// Add the event handlers
		/// </summary>
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;

			_viewModel.OnUpdateError += OnUpdateError;
			_viewModel.OnUpdateSuccess += OnUpdateSuccess;

			LoadingView.IsVisible = true;

			BindingContext = _viewModel;
			_viewModel.LoadData ();
		}

		/// <summary>
		/// Remove the event handlers
		/// </summary>
		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;

			_viewModel.OnUpdateError -= OnUpdateError;
			_viewModel.OnUpdateSuccess -= OnUpdateSuccess;

                        
		}

		#region Event Handlers

		void OnLoadError (string title, string message)
		{
			// Don't display not found errors, in case the user doesn't have a profile.
			LoadingView.IsVisible = false;
		}

		async void OnLoadSuccess ()
		{
			// Show success message
			LoadingView.IsVisible = false;
        

			/*    var mixpanelWidget = DependencyService.Get<IMixPanel>();
                        mixpanelWidget.Track("RegisterCard");

                        var alias = SessionData.PharmacyUser.Username;
                        mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
                        mixpanelWidget.Identify(alias);*/

			//var props = new Dictionary<string,object> ();
			//props.Add("$created", DateTime.UtcNow);
			//mixpanelWidget.PeopleSet(props);

			// Go to the landing page
                     

			// Pop to user card page
			// NavigationUtils.PopToPageType(typeof(UserCardPage), Navigation);       
		}

		void OnUpdateError (string title, string message)
		{
			// Show error message
			DisplayAlert (title, message, AppResources.OK);
			LoadingView.IsVisible = false;
		}

		async void OnUpdateSuccess ()
		{
			// Show success message
			await DisplayAlert (AppResources.UpdateCardTitle, AppResources.UpdateCardSuccessMessage, AppResources.OK);

			/*    var mixpanelWidget = DependencyService.Get<IMixPanel>();
                            mixpanelWidget.Track("RegisterCard");

                            var alias = SessionData.PharmacyUser.Username;
                            mixpanelWidget.CreateAliasForDistinctId(alias, mixpanelWidget.DistinctId());
                            mixpanelWidget.Identify(alias);*/

			//var props = new Dictionary<string,object> ();
			//props.Add("$created", DateTime.UtcNow);
			//mixpanelWidget.PeopleSet(props);

			// Go to the landing page
			await Navigation.PopAsync ();
			// NavigationUtils.PushPageAndClearHistory(new UserAreaMainPage(), Navigation); 
		}

		#endregion

		#region Button Events

		public void MaleButton_Clicked (object sender, EventArgs args)
		{
			_viewModel.IsMale = true;
		}

		public void FemaleButton_Clicked (object sender, EventArgs args)
		{
			_viewModel.IsMale = false;
		}

		public void IDButton_Clicked (object sender, EventArgs args)
		{
			_viewModel.IsBISelected = true;
		}

		public void PassportButton_Clicked (object sender, EventArgs args)
		{
			_viewModel.IsBISelected = false;
		}

        public bool ValidateForm()
        {
            if (string.IsNullOrEmpty(_viewModel.Name) || string.IsNullOrEmpty(_viewModel.IDNumber) ||
                string.IsNullOrEmpty(_viewModel.Address) || string.IsNullOrEmpty(_viewModel.Locale) ||
                string.IsNullOrEmpty(_viewModel.PostalCode4) || string.IsNullOrEmpty(_viewModel.PostalCode3))
            {
                // Validate Required Fields
                OnUpdateError(AppResources.RegisterCardErrorEmptyFieldsTitle,
                    AppResources.RegisterCardErrorEmptyFieldsMessage);
                return false;
            }
            else if (_viewModel.PostalCode4.Length != 4 || _viewModel.PostalCode3.Length != 3)
            {
                // Validate Postal Code
                OnUpdateError(AppResources.RegisterCardErrorPostalCodeTitle,
                    AppResources.RegisterCardErrorPostalCodeMessage);
                return false;
            }
            else if (_viewModel.Phone.Length != 9)
            {
                // Validate Phone
                OnUpdateError(AppResources.RegisterCardErrorPhoneTitle,
                    AppResources.RegisterCardErrorPhoneMessage);
                return false;
            }
			else if ((_viewModel.IDNumber.Length < 6 || _viewModel.IDNumber.Length > 9) && (_viewModel.IsBISelected))
            {
                OnUpdateError(AppResources.RegisterCardErrorPhoneTitle,
                    AppResources.RegisterCardErrorIDNumberMessage);
                return false;
            }

            return true;
        }
		public async void SubmitButton_Clicked (object sender, EventArgs args)
		{
            LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			if (ValidateForm()){
				_viewModel.UpdateCardData ();
            }
		}

		#endregion

		#region Application Bar Settings

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType ()
		{
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle ()
		{
			return AppResources.UpdateCardTitle;
		}

		protected override bool HasAppBarUserButton ()
		{
			return false;
		}

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

		#endregion

	}
}
