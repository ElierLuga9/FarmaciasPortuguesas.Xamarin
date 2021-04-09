using System;
using System.Collections.Generic;
using System.Globalization;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Models.Objects;
using ANFAPP.Logic.Models.Out;
using Newtonsoft.Json;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Models.Out.Ecommerce;

namespace ANFAPP.Logic
{
	public static class SessionData
	{
        private static List<Dictionary<string, object>> _reports = new List<Dictionary<string, object>> ();
        private static List<Dictionary<string, object>> Reports { get { return _reports; }} 




	    /// <summary>
        /// Current Biometric User.
        /// </summary>
        public static User BiometricUser { get; set; }

        /// <summary>
        /// Logged ANF User.
        /// </summary>
        public static PharmacyUser PharmacyUser { get; set; }

		public static Nullable<int> SAFEDistrictId { get; set; }

        /// <summary>
        /// User authentication bundle
        /// </summary>
        public static AnfAuthorizationBundle UserAuthentication { get; set; }

		/// <summary>
		/// User authentication bundle
		/// </summary>
		public static bool AuthANFavorites = false;

		/// <summary>
		///  Master basket, used to synchronize the various checkout pages
		/// </summary>
		public static CheckoutStartOut MasterCheckoutBasket { get; set; }

		/// <summary>
		/// User Profile, used to get the default addresses during checkout
		/// </summary>
		public static UserProfileOut UserProfileForCheckout { get; set; }

		/// <summary>
		/// Returns if the logged user has a pharmacy card.
		/// </summary>
		public static bool HasPharmacyCard { get { return PharmacyUser != null && !string.IsNullOrEmpty(PharmacyUser.CardNumber); } }

		/// <summary>
		/// Returns if a user is logged in the application.
		/// </summary>
		public static bool IsLogged { get { return PharmacyUser != null; } }

		public static bool FacebookLogin = false;
		public static string FaceBookEmail ="";

		/// <summary>
		/// Gets or sets the opened voucher.
		/// </summary>
		/// <value>The opened voucher.</value>
		public static Voucher OpenedVoucher { get; set; }


		public static List<Pharmacy> allPharmacies = new List<Pharmacy> ();
		public static List<Pharmacy> ServiceInServicePharm = new List<Pharmacy>();
		public static List<Pharmacy> longSchedulesInServicePharm = new List<Pharmacy>();
		public static List<Pharmacy> FavoritePharmacies = new List<Pharmacy>();


		public static string NextPharmacies = "";

		public static bool IsServicePagePharm = false;
		public static int SelectedService;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:ANFAPP.Logic.SessionData"/> sucess to card voucher prod.
		/// </summary>
		/// <value><c>true</c> if sucess to card voucher prod; otherwise, <c>false</c>.</value>
		/// 
		public static bool SucessToCardVoucherProd { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:ANFAPP.Logic.SessionData"/> already to card voucher prod.
		/// </summary>
		/// <value><c>true</c> if already to card voucher prod; otherwise, <c>false</c>.</value>
		public static bool AlreadyToCardVoucherProd { get; set; }


		/// <summary>
		/// Returns true if the user is logged in and the selected pharmacy is not the default pharmacy.
		/// </summary>
		/// <value><c>true</c> if is authenticated with pharmacy; otherwise, <c>false</c>.</value>
		public static bool IsAuthenticatedWithPharmacy { 
				get { 
					return IsAuthenticated && !string.Equals(StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT); 
				} 
		}

		public static bool IsAuthenticated { 
			get { 
				return UserAuthentication != null 
					&& !string.IsNullOrEmpty(UserAuthentication.StoreToken)
					&& !string.Equals(UserAuthentication.StoreToken, AnfAuthorizationBundle.MG_PUBLIC_TOKEN);
			} 
		}

		/// <summary>
		/// Returns true the user, authenticated or not, has a pharmacy set (other than the default pharmacy).
		/// </summary>
		/// <value><c>true</c> if has pharmacy set; otherwise, <c>false</c>.</value>
		public static bool IsPharmacySelected { 
			get { 
				return !string.Equals(StorePharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT); 
			} 
		}

		public static event EventHandler OnPharmacyChanged = delegate {};
		public static event EventHandler OnCatastrophicSessionLost = delegate { };

        #region user sharedpreferences, userdefaults

		public static bool IsAllPharmacies
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_ALL_PHARMACIS, Settings.ST_IS_ALL_PHARMACIS_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_ALL_PHARMACIS, value); }
		}

        public static bool IsOnlyServicePharmacies
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_ONLY_SERVICE_PHARMACIES, Settings.ST_IS_ONLY_SERVICE_PHARMACIES_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_ONLY_SERVICE_PHARMACIES, value); }
        }

        public static bool isOnlyLongSchedulePharmacies
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_ONLY_LONG_SCHEDULE_PHARMACIES, Settings.ST_IS_ONLY_LONG_SCHEDULE_PHARMACIES_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_ONLY_LONG_SCHEDULE_PHARMACIES, value); }
        }

        public static bool IsMapView
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_MAP, Settings.ST_IS_MAP_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_MAP, value); }
		}

		/*
		public static double MapHeight
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_MAP, Settings.ST_IS_MAP_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_MAP, value); }
		}*/

		public static Location UserLocation
		{
			get { 
				double latitude = Settings.AppSettings.GetValueOrDefault (Settings.ST_LATITUDE, Settings.ST_LATITUDE_DEFAULT);
				double longitude = Settings.AppSettings.GetValueOrDefault (Settings.ST_LONGITUDE, Settings.ST_LONGITUDE_DEFAULT);

                if (latitude >= -double.Epsilon && latitude <= double.Epsilon && longitude >= -double.Epsilon && longitude <= double.Epsilon) {
					return null;
				}

				Location l = new Location () {
					Latitude = latitude,
					Longitude = longitude
				};
				return l;
			}
			set { 
				Settings.AppSettings.AddOrUpdateValue(Settings.ST_LATITUDE, value.Latitude); 
				Settings.AppSettings.AddOrUpdateValue(Settings.ST_LONGITUDE, value.Longitude); 
			}
		}

		/// <summary>
		/// Current installed districts DB version
		/// </summary>
		public static int? DistrictsDBVersion
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.DISTRICTS_DB_VERSION, Settings.DISTRICTS_DB_VERSION_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.DISTRICTS_DB_VERSION, value); }
		}

		#endregion

        #region User Data

        /// <summary>
        /// Access Token
        /// </summary>
        private static string UserAccessToken
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_USER_ACCESS_TOKEN, Settings.ST_USER_ACCESS_TOKEN_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_USER_ACCESS_TOKEN, value); }
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        private static string UserRefreshToken
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_USER_REFRESH_TOKEN, Settings.ST_USER_REFRESH_TOKEN_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_USER_REFRESH_TOKEN, value); }
        }

		private static string StoreAuthenticationToken
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_AUTH_TOKEN, Settings.ST_MG_AUTH_TOKEN_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_AUTH_TOKEN, value); }
		}

		/// <summary>
		/// Access Token TTL
		/// </summary>
		private static string StoreAuthenticationCookie
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_AUTH_COOKIE, Settings.ST_MG_AUTH_COOKIE_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_AUTH_COOKIE, value); }
		}

		private static string I9AuthenticationToken
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_I9_AUTH_TOKEN, Settings.ST_I9_AUTH_TOKEN_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_I9_AUTH_TOKEN, value); }
		}

        /// <summary>
        /// Access Token TTL
        /// </summary>
        private static string UserTokenTTL
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_USER_TOKEN_TTL, Settings.ST_USER_TOKEN_TTL_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_USER_TOKEN_TTL, value); }
        }

        /// <summary>
        /// Serialized user object.
        /// </summary>
        private static string UserSerializedObject
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_SERIALIZED_USER_OBJECT, Settings.ST_SERIALIZED_USER_OBJECT_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_SERIALIZED_USER_OBJECT, value); }
        }

        /// <summary>
        /// Serialized user object.
        /// </summary>
        public static bool IsFirstRun
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_IS_FIRST_RUN, Settings.ST_IS_FIRST_RUN_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_IS_FIRST_RUN, value); }
        }

		public static string ParseInstallationId
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_PARSE_INSTALLATION_ID, Settings.ST_PARSE_INSTALLATION_ID_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_PARSE_INSTALLATION_ID, value); }
		}

		public static string NotificationService {
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_NOTIFICATIONS_SERVICE, Settings.ST_NOTIFICATIONS_SERVICE_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_NOTIFICATIONS_SERVICE, value); }
		}

		public static string StorePharmacyId
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_PHARMACY_ID, Settings.ST_MG_PHARMACY_ID_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_PHARMACY_ID, value); }
		}

		public static string StorePharmacyName
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_PHARMACY_NAME, Settings.ST_MG_PHARMACY_NAME_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_PHARMACY_NAME, value); }
		}

		public static string StorePharmacyPhone
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_PHARMACY_PHONE, Settings.ST_MG_PHARMACY_PHONE_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_PHARMACY_PHONE, value); }
		}

		public static string StorePharmacyAddress
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.ST_MG_PHARMACY_ADDRESS, Settings.ST_MG_PHARMACY_ADDRESS_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.ST_MG_PHARMACY_ADDRESS, value); }
		}

		private static long BiometricDataTSRepr
		{
			get { return Settings.AppSettings.GetValueOrDefault (Settings.ST_SYNCBIOTS, Settings.ST_SYNCBIOTS_DEFAULT); }
			set { 
				if (value > Settings.ST_SYNCBIOTS_DEFAULT) {
					Settings.AppSettings.AddOrUpdateValue (Settings.ST_SYNCBIOTS, value); 
				} else {
					Settings.AppSettings.AddOrUpdateValue (Settings.ST_SYNCBIOTS, Settings.ST_SYNCBIOTS_DEFAULT); 
				}
			}
		}

		public static DateTime BiometricDataTS
		{
			get { return new DateTime (BiometricDataTSRepr, DateTimeKind.Utc); }
			set { BiometricDataTSRepr = value.Ticks; }
		}

        public static string UserPhotoLocation
        {
            get { return Settings.AppSettings.GetValueOrDefault(Settings.USER_LOCAL_IMG, Settings.USER_LOCAL_IMG_DEFAULT); }
            set { Settings.AppSettings.AddOrUpdateValue(Settings.USER_LOCAL_IMG, value); }
        }

		public static long GamecenterLastUpdate
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.GAMECENTER_LAST_DISTANCE_CHECK, Settings.GAMECENTER_LAST_DISTANCE_CHECK_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.GAMECENTER_LAST_DISTANCE_CHECK, value); }
		}

		public static bool HasFitnessServicesEnabled
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.GAMECENTER_HAS_FITNESS_ENABLED, Settings.GAMECENTER_HAS_FITNESS_ENABLED_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.GAMECENTER_HAS_FITNESS_ENABLED, value); }
		}

		public static string LastLoginName
		{
			get { return Settings.AppSettings.GetValueOrDefault(Settings.LAST_LOGIN, Settings.LAST_LOGIN_DEFAULT); }
			set { Settings.AppSettings.AddOrUpdateValue(Settings.LAST_LOGIN, value); }
		}

        #endregion

        #region User Data Loaders

        /// <summary>
        /// Load the user data from cache.
        /// </summary>
        public static bool LoadUser()
        {
            // Validate data
            if (string.IsNullOrEmpty(UserSerializedObject) ||
                string.IsNullOrEmpty(UserAccessToken) ||
                string.IsNullOrEmpty(UserRefreshToken) ||
                string.IsNullOrEmpty(UserTokenTTL))
            {
				// Create the default authentication bundle for anonymous store users.
				UserAuthentication = new AnfAuthorizationBundle()
				{
					StoreToken = AnfAuthorizationBundle.MG_PUBLIC_TOKEN,
					StoreCookie = StoreAuthenticationCookie
				};

                return false;
            }

            // Initialize user
            PharmacyUser = JsonConvert.DeserializeObject<PharmacyUser>(UserSerializedObject);

            // Initialize authentication bundle
			UserAuthentication = new AnfAuthorizationBundle()
            {
                AccessToken = UserAccessToken,
                RefreshToken = UserRefreshToken,
				TTL = DateTime.ParseExact(UserTokenTTL, Settings.DEFAULT_DATEFORMAT, null),
				StoreToken = StoreAuthenticationToken,
				StoreCookie = StoreAuthenticationCookie,
				I9Token = I9AuthenticationToken
            };

            return true;
        }

		/// <summary>
        /// Save the user information into the persistent storage
        /// </summary>
		public static void SaveUser(UserProfileOut user, AnfAuthorizationBundle authBundle)
        {
			if (user == null) return;
			var pharmacyCard = user.Client != null ? user.Client.PharmacyCard : null;
            
			DateTime date = new DateTime(1);
			if (null != user.Client) {
				date = DateTime.Parse(user.Client.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
				date = date.ToUniversalTime();
			}

			SaveUser(
				// User
				new PharmacyUser()
				{
					Username = user.User != null ? user.User.UserName : user.Id,
					CardNumber = pharmacyCard != null ? pharmacyCard.Number : null,
					IsFamilyCard = pharmacyCard != null ? pharmacyCard.IsFamilyCard : false,
					IsFamilyHead = pharmacyCard != null ? pharmacyCard.IsFamilyHead : false,
					Points = pharmacyCard != null ? pharmacyCard.Wallet.PointsBalance : 0,
					ExpiringPoints = pharmacyCard != null ? pharmacyCard.Wallet.PointsToPrescribe : 0,
					SavedAmount = pharmacyCard != null ? pharmacyCard.Wallet.TotalValueFolded : 0,
                    
					Name = pharmacyCard != null ? user.Client.Name : null,
					Birthdate = date,
                    PhotoLocation = SessionData.UserPhotoLocation,

					ContactPhone = user.Client != null ? user.Client.ContactPhone : null,
					ClientNumber = user.Client != null ? user.Client.Number : null,

					// Set now as last updated
					LastUpdated = DateTime.Now
				}, authBundle);
		}

        /// <summary>
        /// Save the user information into the persistent storage
        /// </summary>
		public static void SaveUser(PharmacyUser user, AnfAuthorizationBundle authBundle)
        {
            // Try to load the user data from storage.
            if (user == null) return;

            // Save the user
            SaveUser(user);

			if (authBundle == null) return;

            // Save Authorization
			SaveAuthorization(authBundle.AccessToken, authBundle.RefreshToken, authBundle.TTL, authBundle.StoreToken, authBundle.StoreCookie, authBundle.I9Token);
        }

        /// <summary>
        /// Save the user information into the persistent storage
        /// </summary>
        public static void SaveUser()
        {
            // Save the user in session
            SaveUser(PharmacyUser);
        }

        /// <summary>
        /// Save the user information into the persistent storage
        /// </summary>
        /// <param name="user"></param>
        public static void SaveUser(PharmacyUser user)
        {
            // Initialize user
            PharmacyUser = user;

            // Save properties
            if (IsLogged) UserSerializedObject = JsonConvert.SerializeObject(PharmacyUser);
        }

        /// <summary>
        /// Save the authorization
        /// </summary>
        /// <param name="bundle"></param>
		public static void SaveAuthorization(IAuthorizationBundle bundle, bool createUser = true)
        {
            if (bundle == null || !(bundle is AnfAuthorizationBundle)) return;
			var authBundle = bundle as AnfAuthorizationBundle;

            // Save Bundle
			SaveAuthorization(authBundle.AccessToken, authBundle.RefreshToken, authBundle.TTL, authBundle.StoreToken, authBundle.StoreCookie, authBundle.I9Token, createUser);
        }

        /// <summary>
        /// Save the authorization
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <param name="ttl"></param>
		/// <param name="storeToken"></param> 
		public static void SaveAuthorization(string accessToken, string refreshToken, DateTime ttl, string storeToken, string storeCookie, string i9Token, bool createUser = true)
        {
            UserAccessToken = accessToken;
            UserRefreshToken = refreshToken;
			UserTokenTTL = ttl.ToString(Settings.DEFAULT_DATEFORMAT);
			StoreAuthenticationToken = storeToken;
			StoreAuthenticationCookie = storeCookie;
			I9AuthenticationToken = i9Token;

            // Initialize authentication
			UserAuthentication = new AnfAuthorizationBundle()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TTL = ttl,
				StoreToken = storeToken,
				StoreCookie = storeCookie,
				I9Token = i9Token
            };

            // Initialize the user if needed
			// XXX: Do/should we really instanciate the PharmacyUser here??
			if (PharmacyUser == null && createUser) PharmacyUser = new PharmacyUser();
        }

        /// <summary>
        /// Clears the current user's data.
        /// </summary>
        public static void ClearUser()
        {
            UserAccessToken = Settings.ST_USER_ACCESS_TOKEN_DEFAULT;
            UserRefreshToken = Settings.ST_USER_ACCESS_TOKEN_DEFAULT;
            UserTokenTTL = Settings.ST_USER_ACCESS_TOKEN_DEFAULT;
            UserSerializedObject = Settings.ST_SERIALIZED_USER_OBJECT_DEFAULT;
			StoreAuthenticationToken = Settings.ST_MG_AUTH_TOKEN_DEFAULT;
			StoreAuthenticationCookie = Settings.ST_MG_AUTH_COOKIE_DEFAULT;
			StorePharmacyId = Settings.ST_MG_PHARMACY_ID_DEFAULT;
			I9AuthenticationToken = Settings.ST_I9_AUTH_TOKEN_DEFAULT;
            UserPhotoLocation = Settings.USER_LOCAL_IMG_DEFAULT;

            PharmacyUser = null;
            UserAuthentication = null;
			UserProfileForCheckout = null;
			MasterCheckoutBasket = null;

			// Restore the default authentication bundle for anonymous store users.
			UserAuthentication = new AnfAuthorizationBundle()
			{
				StoreToken = AnfAuthorizationBundle.MG_PUBLIC_TOKEN
			};

			HasFitnessServicesEnabled = false;

			SaveUser ();
        }

		public static List<Models.Out.VoucherOut> VoucherWithPharmDetail = null;

		/// <summary>
		/// Updates the local session pharmacy.
		/// </summary>
		/// <param name="pharmacyId">The identifier of the current pharmacy.</param>
		/// <param name="pharmacyName">The name of the current pharmacy.</param>
		/// <param name="pharmacyPhone">The contact phone of the current pharmacy.</param>
		/// <param name="ignoreEv">If set to <c>true</c> the OnPharmacyChanged handler will not be called.</param>
		public static void UpdatePharmacy(string pharmacyId, string name = null, string phone = null, string address = null, bool ignoreEv = false)
		{
			var prevPharmacy = SessionData.StorePharmacyId;

			SessionData.StorePharmacyId = pharmacyId;
			if (name == null && string.Equals (pharmacyId, Settings.ST_MG_PHARMACY_ID_DEFAULT)) {
				SessionData.StorePharmacyName = Settings.ST_MG_PHARMACY_NAME_DEFAULT;
				SessionData.StorePharmacyPhone = Settings.ST_MG_PHARMACY_PHONE_DEFAULT;
				SessionData.StorePharmacyAddress = Settings.ST_MG_PHARMACY_ADDRESS_DEFAULT;
			} else {
				SessionData.StorePharmacyName = name;
				SessionData.StorePharmacyPhone = phone;
				SessionData.StorePharmacyAddress = address;
			}

			if (!ignoreEv) OnPharmacyChanged(null, new PharmacyChangedEventArgs() { OldPharmacyId = prevPharmacy, NewPharmacyId = pharmacyId });
		}

		public static void ForceEmergencyLogout()
		{
			if (OnCatastrophicSessionLost != null) OnCatastrophicSessionLost(null, new OnLogoutEventArgs() { Message = AppResources.CatastrophicLogoutErrorMessage });
		}

        #endregion

        #region Debug/Profiling

        public static void AppendReport(string labelOrUrl, DateTime start, long elapsed)
        {
        }

        public static void FlushReports()
        {
        }

        #endregion
    }
}

