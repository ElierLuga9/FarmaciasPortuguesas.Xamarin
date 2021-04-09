using ANFAPP.Logic;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Models.Out.Ecommerce;
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
using ANFAPP.Logic.Models.Authorization;
using XLabs.Platform.Services.Media;
using XLabs.Ioc;
using XLabs.Platform.Device;
using ANFAPP.Logic.Models.Wrappers;

namespace ANFAPP.Pages.Store.Checkout
{
    public partial class CheckoutDeliveryMethodPage : ANFPage
    {

		#region Properties

		private CheckoutDeliveryMethodViewModel _viewModel = new CheckoutDeliveryMethodViewModel();

		#endregion

        #region Page Initialization

		public CheckoutDeliveryMethodPage(CheckoutStartOut basket)
			: base() 
		{ 
			_viewModel.Basket = basket;
			_viewModel.UpdateAddress();
			_viewModel.UpdateBindings();
			_viewModel.UpdateBindableData();

		}

        protected override void InitPage()
        {
            InitializeComponent();
            base.InitPage();

			BindingContext = _viewModel;
        }

        #endregion

		#region Lifecycle Events

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			_viewModel.OnLoadStart += OnLoadStart;
			_viewModel.OnLoadError += OnLoadError;
			_viewModel.OnLoadSuccess += OnLoadSuccess;
			_viewModel.OnDeliveryChangeSuccess += OnDeliveryChangeSuccess;
			_viewModel.OnImageRemovedSuccess += OnImageRemovedSuccess;

			LoadingView.IsVisible = false;

			_viewModel.RefreshBasket();
			// Force an update to reload the basket (because the pharmacy may change the delivery flag).
			await _viewModel.SetHomeDelivery (_viewModel.IsHomeDelivery);

			if (string.IsNullOrEmpty(xAdress.Text.ToString()) || string.IsNullOrEmpty(xPostalCode4.Text.ToString()) || string.IsNullOrEmpty(xLocation.Text.ToString()))
			{
				AdressEditMode.IsVisible = true;
				AdressShortInfo.IsVisible = false;
			}
			else 
			{
				AdressEditMode.IsVisible = false;
				AdressShortInfo.IsVisible = true;
			}
			if (string.IsNullOrEmpty(xBindingName.Text.ToString()) || string.IsNullOrEmpty(xBindingNIF.ToString()))
			{
				//BilingDataShortInfo.IsVisible = false;
				BilingDataEditMode.IsVisible = true;
			}
			else
			{
				//BilingDataShortInfo.IsVisible = true;
				BilingDataEditMode.IsVisible = true;
			}

		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			_viewModel.OnLoadStart -= OnLoadStart;
			_viewModel.OnLoadError -= OnLoadError;
			_viewModel.OnLoadSuccess -= OnLoadSuccess;
			_viewModel.OnDeliveryChangeSuccess -= OnDeliveryChangeSuccess;
			_viewModel.OnImageRemovedSuccess -= OnImageRemovedSuccess;
		}

		#endregion

		#region Event Handlers

		void OnTakeRecipePhotoButtonClicked(object sender, EventArgs args)
		{
			_viewModel.TakePhoto();
		}

		async void OnConfirmButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;

			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			if(string.IsNullOrEmpty(xPhone.Text))
			{
				await DisplayAlert(null,AppResources.CheckoutPhoneEmptyFields,AppResources.OK);
				LoadingView.IsVisible = false;
				return;
			}
			else if(string.IsNullOrEmpty(xBindingName.Text) || string.IsNullOrWhiteSpace(xBindingName.Text) )
			{
				await DisplayAlert(null, "É obrigatório o preenchimento do Nome para prosseguir com a encomenda.", AppResources.OK);
				LoadingView.IsVisible = false;
				return;
			}
			else if(string.IsNullOrEmpty(xBindingNIF.Text))
			{
				await DisplayAlert(null, "É obrigatório o preenchimento do NIF para prosseguir com a encomenda.", AppResources.OK);
				LoadingView.IsVisible = false;
				return;
			}
			else
			{

				if (xBindingNIF.Text.Length < 9)
				{
					await DisplayAlert(null, AppResources.CheckoutInvalidNIFErrorMessage, AppResources.OK);
					LoadingView.IsVisible = false;
					return;
				}
				else if (xPhone.Text.Length < 9)
				{
					await DisplayAlert(null, "Telefone deve ser numérico com 9 dígitos.", AppResources.OK);
					LoadingView.IsVisible = false;
					return;
				}
				else
				{
					await _viewModel.ConfirmDeliveryMethod();
					await _viewModel.UpdatePhoneNumber();
					await _viewModel.UpdateBillingAddress();
				}

			}
		}

		async void OnPhotoRemoveButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			if (sender == null || !(sender is View)) return;
			
			var photo = (sender as View).BindingContext;
			if (photo == null || !(photo is PhotoFile)) return;

			await _viewModel.RemovePhoto(photo as PhotoFile);
		}

		void OnDeliveryModeToggleClicked(object sender, EventArgs args)
		{
			_viewModel.SetHomeDelivery (!_viewModel.IsHomeDelivery);
		}

		void OnChangeAdressDataTapped(object sender, EventArgs arg)
		{
			AdressEditMode.IsVisible = true;
			AdressShortInfo.IsVisible = false;
		}
		void OnChangeBilingDataTapped(object sender, EventArgs arg)
		{
			//BilingDataShortInfo.IsVisible = false;
			BilingDataEditMode.IsVisible = true;
		}


		async Task OnLoadStart()
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
		}

		async void OnLoadSuccess()
		{
			var mixpanelWidget = DependencyService.Get<IMixPanel>();
			var props = new Dictionary<string, string>();
			props.Add("DeliveryPicked", _viewModel.IsHomeDelivery ? "HomeDelivery" : "PharmacyPickUp");
			mixpanelWidget.TrackProperties("CheckoutDeliveryMethodConfirmed", props);
			mixpanelWidget.Track("CheckoutPhoneConfirmed");



			await Navigation.PushAsync(new CheckoutPaymentMethodPage(_viewModel.Basket));

		}

		async void OnDeliveryChangeSuccess()
		{
			LoadingView.IsVisible = false;

			if (_viewModel.Basket.HasMSRM && _viewModel.IsHomeDelivery)
			{
				await DisplayAlert(null, AppResources.CheckoutDeliveryMethodHomeAlert, AppResources.OK);
			}
		}

		void OnImageRemovedSuccess()
		{
			LoadingView.IsVisible = false;
		}

		void OnLoadError(string title, string message)
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
			return AppResources.CheckoutPageTitle;
        }

        #endregion

    }
}
