using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ANFAPP.Logic;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.ViewModels;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;


namespace ANFAPP.Logic.ViewModels
{
    public class UserAreaViewModel : IViewModel
    {
        #region Properties

		private readonly string DEFAULT_USER_IMAGE = "img_user.png";

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _gender;
        public string gender
        {
            get { return _gender; }
            set { _gender = value; OnPropertyChanged(); }
        }

        private DateTime _birthdate;
        public DateTime Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; OnPropertyChanged(); }
        }

        private bool _haspharmacycard;
        public bool HasPharmacyCard
        {
            get { return _haspharmacycard; }
            set { _haspharmacycard = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        private string _myfarmacyname;
        public string MyFarmacyName
        {
            get { return _myfarmacyname; }
            set { _myfarmacyname = value; OnPropertyChanged(); }
        }

        private string _myfarmacyaddress;
        public string MyFarmacyAddress
        {
            get { return _myfarmacyaddress; }
            set { _myfarmacyaddress = value; OnPropertyChanged(); }
        }

        private string _useraddress;
        public string UserAddress
        {
            get { return _useraddress; }
            set { _useraddress = value; OnPropertyChanged(); }
        }

        private string _userphone;
        public string UserPhone
        {
            get { return _userphone; }
            set { _userphone = value; OnPropertyChanged(); }
        }

        private bool _isBISelected;
        public bool IsBISelected
        {
            get { return _isBISelected; }
            set
            {

                _isBISelected = value;
                OnPropertyChanged();
                OnPropertyChanged("IsPassportSelected");
            }
        }

        public bool IsPassportSelected
        {
            get { return !_isBISelected; }
        }


        private string _idnumber;
        public string IdNumber
        {
            get { return _idnumber; }
            set
            {
                _idnumber = value;
                OnPropertyChanged();
            }
        }

        private int _familymembers;
        public int FamilyMembers
        {
            get { return _familymembers; }
            set
            {
                _familymembers = value;
                OnPropertyChanged();
            }

        }

        /// <summary>
        /// The image source.
        /// </summary>
        private string _imageSource;
        public string ImageSource
        {
            get
            {
                return _imageSource;

            }
            set
            {
                if (value != Settings.USER_LOCAL_IMG_DEFAULT) { _imageSource = value; } else { _imageSource = Settings.USER_LOCAL_IMG_DEFAULT; }

                OnPropertyChanged();
            }
        }


        #endregion
        #region MediaPicker
        /// <summary>
        /// The picture chooser.
        /// </summary>
        private IMediaPicker _mediaPicker;

        /// <summary>
        /// The select picture command.
        /// </summary>
        private Command _selectPictureCommand;
        public Command SelectPictureCommand
        {
            get
            {
                return _selectPictureCommand ?? (_selectPictureCommand = new Command(
                                                                           async () => await SelectPicture(),
                                                                           () => true));
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            private set { _status = value; }
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        private void Setup()
        {
            if (_mediaPicker != null)
            {
                return;
            }

            var device = Resolver.Resolve<IDevice>();

            ////RM: hack for working on windows phone? 
            _mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
        }

        /// <summary>
        /// Selects the picture.
        /// </summary>
        /// <returns>Select Picture Task.</returns>
        private async Task SelectPicture()
        {
			if (OnStart != null) await OnStart();

            Setup();

            try
            {
				// XXX: MediaPicker never calls OnMediaSelected...
                var mediaFile = await _mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                {
                    MaxPixelDimension = 600,
					SaveMediaOnCapture = true,
                });
				
                string imgSource = string.Empty;
				if (Device.OS == TargetPlatform.iOS) // || Device.OS == TargetPlatform.WinPhone)  
				{
					imgSource = DependencyService.Get<IMediaWriter>().WriteMedia(mediaFile.Path);
				} 
				else if (Device.OS == TargetPlatform.Android) 
				{
					imgSource = mediaFile.Path;
				}
				else if (Device.OS == TargetPlatform.WinPhone)
				{
					//imgSource = await DependencyService.Get<IMediaWriter>().WriteMedia(mediaFile.Source, mediaFile.Path);
					imgSource = "file:\\" + mediaFile.Path;
				}
                SessionData.UserPhotoLocation = imgSource;
                ImageSource = imgSource;
            }
            catch (System.Exception ex)
            {
                Status = ex.Message;
            }

            if (OnSuccess != null) OnSuccess();
        }
        #endregion

        #region Events
        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;
        public event OnLoadStartEventHandler OnStart;

        /*Image Events
        public event OnErrorEventHandler OnPhotoLoadError;
        public event OnSuccessEventHandler OnPhotoSuccessError;*/
        #endregion
        
		#region Helpers

		public static string GetPlatformPhotoLocation()
		{
			string location = null;
			if (Device.OS == TargetPlatform.iOS) {
				var storedImg = SessionData.UserPhotoLocation;
				if (string.IsNullOrWhiteSpace (storedImg) || string.Equals (storedImg, Settings.USER_LOCAL_IMG_DEFAULT)) {
					location = Settings.USER_LOCAL_IMG_DEFAULT;
				} else if (!storedImg.StartsWith ("http")) {
					var root = DependencyService.Get<IMediaWriter> ().GetMediaRoot ();
					location = Path.Combine (root, Path.GetFileName (storedImg));
				} else {
					location = storedImg;
				}
			} else {
				location = SessionData.UserPhotoLocation;
			}

			return location;
		}

		#endregion

		#region Loaders

		public async void LoadData()
		{
			// Start Loading
			if (OnStart != null) await OnStart();

			// Load cached data first
			LoadUserCachedData();

			try
			{
				// Load user info
				if (SessionData.HasPharmacyCard) await LoadUserCardData();
			}
			catch (Exception e)
			{
				if (OnError != null) OnError(null, e.Message);
			}

			try
			{
				// Load Pharmacy
				await LoadPharmacyData();
			}
			catch { }
			finally
			{
				if (OnSuccess != null) OnSuccess();
			}
		}

		public async Task LoadUserCardData()
        {
            //Load user Profile
            var userProfile = await UserCardWS.GetUserProfile(SessionData.PharmacyUser.Username, SessionData.UserAuthentication);
			string userImage = UserAreaViewModel.GetPlatformPhotoLocation();

			DateTime bday = new DateTime(1);
			if (null != userProfile.Client && null != userProfile.Client.BirthDate) {
				bday = DateTime.Parse(userProfile.Client.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
				bday = bday.ToUniversalTime();
			}


            HasPharmacyCard = true;
			Name = userProfile.Client.PharmacyCard.Name;
			Email = userProfile.Client.Email;
            UserAddress = userProfile.Client.Address;
            UserPhone = userProfile.Client.ContactPhone;
			Birthdate = bday;
			ImageSource = !string.IsNullOrEmpty(userImage) ? userImage : DEFAULT_USER_IMAGE;
            IsBISelected = (userProfile.Client.DocumentType == Settings.DOCUMENT_TYPE_NATIONAL_ID) ? true : false;
            IdNumber = userProfile.Client.DocumentNumber;
            FamilyMembers = userProfile.Client.HouseholdSize;

            SessionData.SaveUser(userProfile, null);
        }

        public void LoadUserNoCardData()
        {
            //Load user Profile
            HasPharmacyCard = false;
            Email = SessionData.PharmacyUser.Username;

			string userImage = UserAreaViewModel.GetPlatformPhotoLocation();
			ImageSource = !string.IsNullOrEmpty(userImage) ? userImage : DEFAULT_USER_IMAGE;
        }

		public void LoadUserCachedData()
		{
			LoadUserNoCardData();

			// Load available data
			HasPharmacyCard = SessionData.HasPharmacyCard;
			Name = SessionData.PharmacyUser.Name;
			Email = SessionData.PharmacyUser.Username;
			UserPhone = SessionData.PharmacyUser.ContactPhone;
			Birthdate = SessionData.PharmacyUser.Birthdate;
			FamilyMembers = SessionData.PharmacyUser.FamilySize;
		}

        public async Task LoadPharmacyData()
        {
			if (SessionData.StorePharmacyId == Settings.ST_MG_PHARMACY_ID_DEFAULT)
            {
				// No need to load pharmacy.
				MyFarmacyName = "Não tem Farmácia definida.";
			}
			else
			{
				//Load Pharmacy Profile
				var pharmacyprofile = await UserCardWS.GetUserPharmacy(SessionData.StorePharmacyId);
				MyFarmacyName = pharmacyprofile.Name;
				MyFarmacyAddress = pharmacyprofile.Address;
			}
        }

        #endregion

    }
}
