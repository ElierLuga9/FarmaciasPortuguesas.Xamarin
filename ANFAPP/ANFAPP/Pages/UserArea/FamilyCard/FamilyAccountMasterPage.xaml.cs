using ANFAPP.Logic;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.ViewModels;
using ANFAPP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Pages.UserArea.FamilyCard
{
    public partial class FamilyAccountMasterPage : ANFPage
    {

        #region Properties

        private FamilyAccountMasterViewModel _viewModel;

        #endregion

        #region Page Initialization

        public FamilyAccountMasterPage() : base() { }

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

            BindingContext = _viewModel = new FamilyAccountMasterViewModel();
            
            // Show first loading
            LoadingView.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnError += OnErrorEvent;
            _viewModel.OnSuccess += OnSuccessEvent;
            _viewModel.OnAssociationSuccess += OnAssociationSuccessEvent;

            // Reload on page showing
            _viewModel.LoadData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnError -= OnErrorEvent;
            _viewModel.OnSuccess -= OnSuccessEvent;
            _viewModel.OnAssociationSuccess -= OnAssociationSuccessEvent;
        }

        #endregion

        #region Event Handlers

        public void OnErrorEvent(string title, string message)
        {
            LoadingView.IsVisible = false;
            DisplayAlert(title, message, AppResources.OK);
        }

        public void OnSuccessEvent()
        {
            LoadingView.IsVisible = false;
        }

        public void OnAssociationSuccessEvent()
        {
            LoadingView.IsVisible = false;

            if (!SessionData.PharmacyUser.IsFamilyCard)
            {
                // Not a family card, go back to previous page
                Navigation.PopAsync();
            }
        }

        void AddCardToFamily_Clicked(object sender, EventArgs args)
        {
            // Go to page
            Navigation.PushAsync(new AddCardsToFamilyAccountPage());
        }

        void OnAcceptAssociationRequest_Clicked(object sender, EventArgs args)
        {
            if (sender == null || !(sender is Button)) return;

            // Validate if the binding context is of type AssociationRequestOut
            var request = ((Button)sender).BindingContext;
            if (request != null && request is AssociationRequestOut)
            {
                LoadingView.IsVisible = true;
                _viewModel.AcceptAssociationRequest(request as AssociationRequestOut);
            }
        }

        void OnCancelAssociationRequest_Clicked(object sender, EventArgs args)
        {
            if (sender == null || !(sender is Button)) return;

            // Validate if the binding context is of type AssociationRequestOut
            var request = ((Button)sender).BindingContext;
            if (request != null && request is AssociationRequestOut)
            {
                LoadingView.IsVisible = true;
                _viewModel.CancelAssociationRequest(request as AssociationRequestOut);
            }
        }

        #endregion

        #region Buttons



        #endregion

        #region Application Bar Settings

        protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
        {
            return ANFAPP.Views.ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
        }

        protected override string GetAppBarTitle()
        {
            return AppResources.FamilyAccountMasterPageTitle;
        }

        //testes
        protected override bool HasCardButton()
        {
            return true;
        }

        #endregion

    }
}
