using ANFAPP.Pages;
using ANFAPP.Views;
using ANFAPP.Logic;
using ANFAPP.Logic.Network.Services;
using Xamarin.Forms.Maps;
using System;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using System.Globalization;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Out.Ecommerce;
using ANFAPP.Logic.Utils;
using ANFAPP.Pages.Store;
using ANFAPP.Utils;
using ANFAPP.Logic.Exceptions;
using System.Linq;

namespace ANFAPP.Pages.PharmacyLocator
{
	public partial class LocatorPharmacyDetails : ANFPage
	{

		public bool _isInitialized = false;

		public Pharmacy _pharmacy;
        public LocatorPharmacyDetailsViewModel _viewModel;

		private LocatorPharmacyDetails() : base() { }

		public LocatorPharmacyDetails(Pharmacy p) : this()
		{
			_pharmacy = p;

            BindingContext = _viewModel = new LocatorPharmacyDetailsViewModel(_pharmacy);
		}

        protected override void InitPage()
        {
			InitializeComponent();
			base.InitPage();
        }

		#region Event handlers

        public void OnSuccess()
        {
            ConfigurePharmacyButton();
            LoadingView.IsVisible=false;
            MainPage.IsVisible = true;

			var tmp = new List<Pharmacy>();
			tmp.Add(_pharmacy);
			TheMap.PharmacyList = tmp;

			TheMap.MoveToRegion(new MapSpan(new Position(_pharmacy.Latitude, _pharmacy.Longitude), 0.25, 0.5));
        }

        public async Task OnStart()
        {
            MainPage.IsVisible = false;
            LoadingView.IsVisible = true;
        }

		void OnLoadError(string title, string message)
		{
			LoadingView.IsVisible = false;
			ShowErrorDialog (message);
		}

		#endregion

		#region Auxiliary methods

		public void ConfigurePharmacyButton()
		{
			if (_viewModel.FarmButtonVisible){
				if (SessionData.StorePharmacyId == _pharmacy.Code)
				{
					MyFarmButton.IsVisible = false;
					MyFarmLabel.IsVisible = true;
					GoToStoreButton.IsVisible = true;
                }
				else
				{
					MyFarmButton.IsVisible = true;
					MyFarmLabel.IsVisible = false;
				}

                separatorGoToStore.IsVisible = GoToStoreButton.IsVisible;
            }
		}

		private async void ShowErrorDialog(string message)
		{
			await DisplayAlert(null, message, AppResources.OK);
		}

		#endregion

		#region Application Bar Methods

		protected override ApplicationBar.APPBAR_LEFTBUTTON_TYPE GetAppBarLeftButtonType()
		{
			return ApplicationBar.APPBAR_LEFTBUTTON_TYPE.BACK;
		}

		protected override string GetAppBarTitle()
		{
			if (null != _pharmacy) {
				return _pharmacy.Name;
			}

			return null;
		}

		protected override string GetAppBarSubtitle()
		{
			if (null != _pharmacy) {
				/*if (_viewModel.IsPharmOpen || PharmacyUtils.GetIsInService(_pharmacy.Code))
				{
					return "FARMÁCIA ABERTA";
				}
                else
                {
                    return "FARMÁCIA FECHADA";
                }*/
			}

			return null;
		}

		#endregion

		#region tap actions

		public async void SetMyFarm_Click(object sender, EventArgs args)
		{
			//If user is logged or if user is anonymous and Pharmacy is not DEFAULT
			//if ((SessionData.IsLogged && SessionData.HasPharmacyCard) || string.Equals(SessionData.StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT))
			if ((SessionData.IsLogged) || string.Equals(SessionData.StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT))
			{
				LoadingView.IsVisible = true;
				try
				{
					var result = await ECommerceWS.SetMyFarm(SessionData.UserAuthentication, _pharmacy.Code);
					var setFav = await UserCardWS.SetFavMyFarm(SessionData.UserAuthentication,_pharmacy.Code,"Insert","Preferencial");
					if (result == null || result.code != 210 && result.code != 205)
					{
						LoadingView.IsVisible = false;
						throw new Exception ("De momento não é possivel alterar a sua farmácia.");
					}else {
						SessionData.UpdatePharmacy(result.code == 205 ? result.FarmId : _pharmacy.Code, _pharmacy.Name, _pharmacy.Phone, _pharmacy.Address);
						ConfigurePharmacyButton();

						var mixpanelWidget = DependencyService.Get<IMixPanel>();
						var props = new Dictionary<string, string>();
						props.Add("PharmacyID", _pharmacy.Code);
						mixpanelWidget.TrackProperties("PharmacySelected", props);
					}
				}
				catch (Exception e)
				{
					string message = e.Message;
					if (!(e is InvalidRequestException) && !(e is NetworkingException)) message = AppResources.GenericErrorMessage;
					ShowErrorDialog(message);
				}
			}
			else
			{
				ShowErrorDialog("Terá de se registar para voltar a trocar a Sua Farmácia");
			}

			LoadingView.IsVisible = false;
		}


		
        public async void GoToStoreButtonClicked(object sender, EventArgs args)
		{
			LoadingView.IsVisible = true;
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);

			//NavigationUtils.PushPageAndClearHistory(new StoreLandingPage(), Navigation);
            await Navigation.PushAsync(new StoreLandingPage());
		}



		public async void KnowFarmacyRowTap(object sender, EventArgs args)
		{
            var hourlist = _viewModel.FarmHourList.Select(l => (object)l).ToList();
            var notAvailable = _viewModel.FarmNotAvailableList.Select(l => (object)l).ToList();
            
            var knowPharmacyPage = new KnowThisPharmacy(_pharmacy, _viewModel.IsPharmOpen, hourlist, notAvailable);
	
			await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
			await Navigation.PushAsync(knowPharmacyPage);
		}

		void PhoneRowTap(object sender, EventArgs args)
		{

			string phoneUrl = "tel:" + _pharmacy.Phone;
			Device.OpenUri(new Uri(phoneUrl));

			/*
			var device = Resolver.Resolve<IDevice>();
			if (device.PhoneService != null)
			{
				string phoneUrl = "tel:" + _pharmacy.Phone;

				Device.OpenUri(new Uri(phoneUrl));
			}
			else
			{
				DisplayAlert(AppResources.DAAtencionTitle, AppResources.DAAtencionBody, "OK");
			}
			*/
		}

		async void FavouriteRowTap(object sender, EventArgs args)
		{
			LocalizationDAO lDAO = new LocalizationDAO();
			var pharmacies = await lDAO.GetFavourits();
			foreach (var p in pharmacies) 
			{
				if (p.Code == _pharmacy.Code) 
				{
					_pharmacy.IsFavourite = true;
				}
			}
			if (_pharmacy.IsFavourite)
			{

				ImgFavourite.Source = "star_unfavourite";
				try
				{

					var a = await UserCardWS.SetFavMyFarm(SessionData.UserAuthentication, "" + _pharmacy.Code, "Delete");
				}
				catch (Exception e) { }
				SessionData.FavoritePharmacies.Remove(_pharmacy);
				//System.Diagnostics.Debug.WriteLine(a.code
				           
				_pharmacy.IsFavourite = false;
			//	ImgFavourite.Source = "star_unfavourite";
			}
			else
			{
				ImgFavourite.Source = "star_favourite";
				try
				{
					var a = await UserCardWS.SetFavMyFarm(SessionData.UserAuthentication, "" + _pharmacy.Code, "Insert");
				}
				catch (Exception e) {}
				SessionData.FavoritePharmacies.Add(_pharmacy);
				//System.Diagnostics.Debug.WriteLine(a.code);
				_pharmacy.IsFavourite = true;

			}

			PharmacyDAO pDAO = new PharmacyDAO();

			await pDAO.InsertOrUpdate(_pharmacy);

		}

		void DirectionsRowTap(object sender, EventArgs args)
		{

			string url = "";
			/*
			#if __IOS__
			url = "http://maps.apple.com/?saddr={0}&daddr={1}";
			#elif __ANDROID__
			url = "http://maps.google.com/maps?saddr={0}&daddr={1}&mode=driving"
			#else
					throw new Exception("No device type compile-time directive found");
			#endif
			*/
			/*
			Device.OnPlatform (
					url = "http://maps.apple.com/?saddr={0}&daddr={1}",
					url = "http://maps.google.com/maps?saddr={0}&daddr={1}&mode=driving",
					url = "null",
					url = "http://www.google.com"
			);*/

			Device.OnPlatform(
					iOS: () => url = "http://maps.apple.com/?saddr={0}&daddr={1}",
					Android: () => url = "http://maps.google.com/maps?saddr={0}&daddr={1}&mode=driving",
					WinPhone: () => url = "http://bing.com/maps/default.aspx?rtp=pos.{0}~pos.{1}"
			);

			string coordsSeparator = (Device.OS == TargetPlatform.WinPhone) ? "_" : ",";
			string endPoint = _pharmacy.Latitude.ToString(CultureInfo.InvariantCulture) + coordsSeparator + _pharmacy.Longitude.ToString(CultureInfo.InvariantCulture);
			string startingPoint = string.Empty;
			if (null == SessionData.UserLocation)
			{
				var locationServices = DependencyService.Get<ILocationServices>();
                var currentLocation= locationServices.CurrentUserLocation();
                if (currentLocation==null)
                {
                    DisplayAlert("Erro de Conexão", "Por favor, ligue os serviços de localização", "OK");
                    return;
                }
                SessionData.UserLocation = currentLocation;
			}

			if (null != SessionData.UserLocation)
			{
				startingPoint = SessionData.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture) + coordsSeparator + SessionData.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture);
			}
			//else
			//{
			//   startingPoint = endPoint;
			//}

			url = url.Replace("{0}", startingPoint);
			url = url.Replace("{1}", endPoint);

			Device.OpenUri(new Uri(url));
		}

		#endregion


		#region Page Lifecylce

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            _viewModel.OnLoadStart += OnStart;
            _viewModel.OnLoadSuccess += OnSuccess;
			_viewModel.OnLoadError += OnLoadError;

            var locationServices = DependencyService.Get<ILocationServices>();
            locationServices.StartUpdatingLocation();

			if (!_isInitialized)
			{			
				ApplicationBar.SetTitle(GetAppBarTitle());
				ApplicationBar.SetSubtitle(GetAppBarSubtitle());
				await _viewModel.LoadData();

				if (_viewModel.IsPharmOpen || PharmacyUtils.GetIsInService(_pharmacy.Code))
				{
                    InServiceLabel.IsVisible = true;
                    ApplicationBar.SetSubtitle("FARMÁCIA ABERTA");
                }
                else
                {
                    ApplicationBar.SetSubtitle("FARMÁCIA FECHADA");
                }

                Scheduler.IsVisible = _viewModel.FarmHourList != null && _viewModel.FarmHourList.Count > 0;

                OutServiceLabel.IsVisible = !InServiceLabel.IsVisible;

                _isInitialized = true;
			}
		}

        protected override void OnDisappearing()
		{
			base.OnDisappearing();

            //testes
            LoadingView.IsVisible = false;

            _viewModel.OnLoadSuccess -= OnSuccess;
            _viewModel.OnLoadStart -= OnStart;
			_viewModel.OnLoadError -= OnLoadError;

            var locationServices = DependencyService.Get<ILocationServices>();
            locationServices.StopUpdatingLocation();
		}

		#endregion
	}
}

