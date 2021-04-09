//#define PROD
//#undef PROD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace ANFAPP.Logic
{
    public static class Settings
    {

        /// <summary>
        /// Application Settings.
        /// </summary>
		public static ISettings AppSettings
		{
			get { return CrossSettings.Current; }
		}

		/// <summary>
		/// Sets the state of the gamification functionality in the application
		/// </summary>
		public static readonly bool IS_GAMIFICATION_ACTIVE = true;

		public static readonly bool IS_GAMIFICATION_MENU_ACTIVE = false;

		/// <summary>
		/// Google Analytics Key
		/// </summary>
		public static readonly string GOOGLE_ANALYTICS_KEY = "UA-60916603-1";

        /// <summary>
        /// Insights Key
        /// </summary>
        public static readonly string XAMARIN_INSIGHTS_KEY = "7cd0086f9460ca1892d28910b1b5760ba7bdf91b";

        /// <summary>
        /// GCM Sender ID, for push notifications
        /// </summary>
		public static readonly string ANDROID_GCM_SENDER_ID = "50056245461";




		public static readonly int CURRENT_DISTRICTS_DB_VERSION = 3;
		
		/**
		 * !! SERVICE KEYS !! 
		 */
		public static readonly string BRANCHIO_API_KEY = "key_live_ikoZruaLBWcy1bZgDkBrVdpcwBilrDMv";

#if PROD
		/* PRODUCTION - FINAL */
		public static readonly string MIXPANEL_KEY = "289cad177b9119477488dd19792580ce";
		public static readonly string PARSE_APPLICATION_ID = "TNdhtqIbmgfpbVfOVSEYyOLsAWiGIMiQp8vq8GpC";
		public static readonly string PARSE_REST_API_KEY = "mVUCT8LvvyEqfwAXImcryyXuHFFICGMzN6EB1k5T";
		public static readonly string PARSE_DOTNET_API_KEY = "IPOw6SI9JmNwzT2oymUYIzsmLqMjTXXBi9UPQdrn";

		public static readonly string AZURE_CONNECTION_STRING = "Endpoint=sb://sifarma.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=22IojcX7H6D4hhRc3PcCr1PZjM52xhAmOx61ZtE7Tfw=";
	//	public static readonly string AZURE_CONNECTION_STRING = "Endpoint=sb://sifarma.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=22IojcX7H6D4hhRc3PcCr1PZjM52xhAmOx61ZtE7Tfw=";
		public static readonly string AZURE_NOTIFICATION_HUB = "farmaciaspt";

        public static readonly string AZURE_CONNECTION_ENDPOINT = "sb://sifarma.servicebus.windows.net/farmaciaspt";
		//public static readonly string AZURE_CONNECTION_ENDPOINT = "https://sifarma.servicebus.windows.net/farmaciaspt";
        public static readonly string AZURE_CONNECTION_AUTHORIZATION = "22IojcX7H6D4hhRc3PcCr1PZjM52xhAmOx61ZtE7Tfw=";
        public static readonly string USERCARD_PROMO_ENDPOINT = "https://www.farmaciasportuguesas.pt/promos/promo.json";

#else
        /* QA */
        public static readonly string MIXPANEL_KEY = "e1ec4855fd72dab739f55332cbef74ed";
		public static readonly string PARSE_APPLICATION_ID = "is9fzKuIWEpR7ewRrz4s4PUhrfrtAcSL7FouFDCQ";
		public static readonly string PARSE_REST_API_KEY = "dtXZn8hZQQoq2uRjm0e40EnR4kuwQjG60zSVtI9z";
		public static readonly string PARSE_DOTNET_API_KEY = "ykXSHHUOn4LxCZFcZXwUY7zxywmxKpJEaXQwI37Z";
				
		public static readonly string AZURE_CONNECTION_STRING = "Endpoint=sb://sifarma.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=wI6ZT6iFOnaKjqRt0t9bQbXs6gsXKch3CP/M9piRu7Y=";
		public static readonly string AZURE_NOTIFICATION_HUB = "farmaciasptqa";

        public static readonly string AZURE_CONNECTION_ENDPOINT = "https://sifarma.servicebus.windows.net/farmaciasptqa";
        public static readonly string AZURE_CONNECTION_AUTHORIZATION = "wI6ZT6iFOnaKjqRt0t9bQbXs6gsXKch3CP/M9piRu7Y=";
#endif

        /**
		 * !! MAIN ENDPOINTS !!
		 */

#if PROD
		/* PRODUCTION - FINAL */
		public static readonly string ENDPOINT_ANF = "https://services.anf.pt/REST/V4/pfpodata/";
		public static readonly string ENDPOINT_ANF_FAVORITES = "https://services.anf.pt/REST/Gateway/External/V1/api/crm/clients/";
		public static readonly string ENDPOINT_MAGENTO_BASE = "https://services-b2c-ecommerce.farmaciasportuguesas.pt";
		public static readonly string TOMAS_ENDPOINT = "https://services-b2c.farmaciasportuguesas.pt/wstomas/anftomasmedicamentos.asmx/";
		public static readonly string GAMIFICATION_ENDPOINT = "https://services-b2c.farmaciasportuguesas.pt/wsgamification/gamification.asmx/";
		public static readonly string BIO_ENDPOINT = "https://services-b2c.farmaciasportuguesas.pt/wsdadosbiometricos/anfdadosbiometricos.asmx/";
        /*promotions testes*/
        public static readonly string PROMOTIONS_ENDPOINT = "http://wsfpp.anf.pt/industryWS/api/ActivePromos";

		public static readonly string USER_PROMO_VOUCHER = "http://services-qa.anf.pt/REST/v4/PfpOData/";
#else
        /* QA */
        public static readonly string ENDPOINT_ANF = "https://services-qa.anf.pt/REST/v4/PfpOData/";

		public static readonly string ENDPOINT_ANF_FAVORITES = "https://services-qa.anf.pt/REST/Gateway/External/V1/api/crm/clients/";

		//public static readonly string ENDPOINT_MAGENTO_BASE = "http://magento-qa.farmaciasportuguesas.pt";
		public static readonly string ENDPOINT_MAGENTO_BASE = "https://magento-qa.anf.pt";

		public static readonly string TOMAS_ENDPOINT = "http://web-qa.farmaciasportuguesas.pt/wstomas/anftomasmedicamentos.asmx/";
		public static readonly string GAMIFICATION_ENDPOINT = "http://web-qa.farmaciasportuguesas.pt/wsgamification/gamification.asmx/";
		public static readonly string BIO_ENDPOINT = "http://web-qa.farmaciasportuguesas.pt/wsdadosbiometricos/anfdadosbiometricos.asmx/";
        //testes
        public static readonly string PROMOTIONS_ENDPOINT = "http://web-qa.farmaciasportuguesas.pt/IndustryWS/api/ActivePromos";
		public static readonly string USER_PROMO_VOUCHER = "http://services-qa.anf.pt/REST/v4/PfpOData/";

		public static readonly string USERCARD_PROMO_ENDPOINT = "http://web-qa.farmaciasportuguesas.pt/industryWS/promo.json";

#endif


		/// DEV
		//public static readonly string ENDPOINT_ANF = "https://services-qa.anf.pt/REST/v4/PfpOData/";
		//public static readonly string ENDPOINT_MAGENTO_BASE = "http://anf.toogasliveweb1.com";
		//public static readonly string TOMAS_ENDPOINT = "http://wstomasv2.anf.innovagencyhost.com/ANFTomasMedicamentos.asmx/";
		//public static readonly string GAMIFICATION_ENDPOINT = "http://wsgamification.anf.innovagencyhost.com/Gamification.asmx/";
		//public static readonly string BIO_ENDPOINT = "http://wsdadosbiometricos.anf.innovagencyhost.com/AnfDadosBiometricos.asmx/";
		//public static readonly string USER_PROMO_VOUCHER = "http://services-qa.anf.pt/REST/v4/PfpOData/";

		/// <summary>
		/// The Magento endpoint
		/// </summary>
		public static readonly string ENDPOINT_MAGENTO = ENDPOINT_MAGENTO_BASE + "/catalogo/webservice/";

		/// <summary>
		/// The Magento Promo CRM
		/// </summary>
		public static readonly string PROMO_VOUCHER = ENDPOINT_ANF + "Pfp-Campaign('{0}')";

		/// <summary>
		/// Partnership URL
		/// </summary>
#if PROD
		public static readonly string PARTNERSHIP_ENDPOINT = "https://www.farmaciasportuguesas.pt/parcerias-app";
#else
		public static readonly string PARTNERSHIP_ENDPOINT = "http://magento-qa.farmaciasportuguesas.pt/parcerias-app";
		#endif

		public static readonly string GENERAL_CONDITIONS_URL = "https://www.farmaciasportuguesas.pt/condicoes-gerais";

		public static readonly string PRIVACY_POLICY_URL = "https://www.farmaciasportuguesas.pt/politica-de-privacidade";

        /// <summary>
        /// Application Id
        /// </summary>
		public static readonly string APPLICATION_ID = "YXBwLmIyYy5tb2JpbGU=";	/// PROD


		/// <summary>
		/// Application ID for the I9 WS
		/// </summary>
		public static readonly string I9WS_APPLICATION_ID = "SM5OK5WR64CJWEIGYG8XD5LMLFZZ8MJQ";

        /// <summary>
        /// Application User
        /// </summary>
        public static readonly string APPLICATION_USER = "anf.pfp.mobile";

        /// <summary>
        /// Application Password
        /// </summary>
        public static readonly string APPLICATION_PASSWORD = "anf.pfp.mobile!123";

        /// <summary>
        /// Database
        /// </summary>
        public static readonly string DATABASE_NAME = "ANF_DB.db3";

		/// <summary>
		/// Default Locale
		/// </summary>
		public static readonly string DEFAULT_LOCALE = "pt-PT";

		/// <summary>
		/// Default DateFormat
		/// </summary>
		public static readonly string DEFAULT_DATEFORMAT = "dd/MM/yyyy HH:mm:ss";

		#region Facebook

		public static readonly string[] FACEBOOK_PERMISSIONS = { "email", "public_profile" };

		public static readonly string FACEBOOOK_PROVIDER = "Facebook";

		public static readonly string FACEBOOK_PROFILE_PICTURE_GRAPH = "https://graph.facebook.com/{0}/picture?type=large";

		#endregion

		#region WebServices

		/// <summary>
		/// Authorization WebService (Login)
		/// </summary>
        public static readonly string WS_AUTHORIZATION = ENDPOINT_ANF + "AuthorizationRequest";

		/// <summary>
		/// Facebook Authorization WebService (Facebook Login)
		/// </summary>
		public static readonly string WS_FACEBOOK_AUTHORIZATION = ENDPOINT_ANF + "ProviderAuthorizationRequest?$select=Authorization";

		/// <summary>
		/// Authorization Renewal WebService (Refresh Token)
		/// </summary>
		public static readonly string WS_AUTHORIZATION_REFRESH = ENDPOINT_ANF + "AuthorizationRefresh";

		/// <summary>
		/// User WebService (Get Pharmacies, not authenticated)
		/// </summary>
		public static readonly string WS_PHARMACIES = ENDPOINT_ANF + "Pharmacy";

        public static readonly string WS_PHARMACIES_BASE_FILTER = "?$expand=NonWorkingPeriods,WorkSchedules,Shifts($filter=Date ge {1})&$filter=IsActive and (ANFSituation eq 16 or ANFSituation eq 14) and Type ne 4 and ({0})";
        public static readonly string WS_PHARMACY_BY_DISTRICT_FILTER = "District eq {0}";
        public static readonly string WS_PHARMACY_BY_COUNTY_FILTER = "District eq {0} and County eq {1}";
        public static readonly string WS_PHARMACY_BY_PARISH_FILTER = "District eq {0} and County eq {1} and Parish eq '{2}'";
        public static readonly string WS_PHARMACIES_GEO_FILTER = "?$expand=NonWorkingPeriods,WorkSchedules,Shifts($filter=Date ge {4})&$filter=IsActive and (ANFSituation eq 16 or ANFSituation eq 14) and Type ne 4 and (GeoCoordinates/Latitude lt {0}M and GeoCoordinates/Longitude gt {1}M) and (GeoCoordinates/Latitude gt {2}M and GeoCoordinates/Longitude lt {3}M)";
        public static readonly string WS_PHARMACIES_DETAIL_DATA = "?$expand=WorkSchedules,NonWorkingPeriods";
		public static readonly string WS_PHARMACIES_IS_OPEN_24H = "/.Is24HourWorkSchedule";


        /// <summary>
        /// Query pharmacies by service. See Section 3.9.3 of 'Solução de Integração Viva+ Fase II'.
        /// </summary>
        public static readonly string WS_PHARMACY_SERVICES_FILTER = "?$filter=IsActive and (ANFSituation eq 16 or ANFSituation eq 14) and Type ne 4 and PharmacyServices/any(s:s/Id eq {0})&$expand=PharmacyServices,WorkSchedules,Shifts($filter=Date ge {1})";
        //public static readonly string WS_PHARMACY_SERVICES_FILTER = "?$filter=PharmacyServices/any(s:s/Id eq {0})&$expand=PharmacyServices,Shifts($filter=Date ge {1})";

        public static readonly string WS_GOOGLE_GEO = "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&$language=pt-PT&country=pt";

        public static readonly string WS_GET_PHARMACY=WS_PHARMACIES+"('{0}')";
        #region User Card

        /// <summary>
        /// API User WS
        /// </summary>
        public static readonly string WS_API_USER = ENDPOINT_ANF + "ApiUserRegistration?$expand=User&$select=User";

		/// <summary>
		/// Create a CRM client (Viva+ 3.11.1, 5.11.2).
		/// </summary>
		public static readonly string WS_CREATE_CRM_CLIENT = ENDPOINT_ANF + "Pfp-Client";

		/// <summary>
		/// Get a CRM client (Viva+ 3.11.3).
		/// </summary>
		public static readonly string WS_GET_CRM_CLIENT = ENDPOINT_ANF + "Pfp-Client('{0}')";

		/// <summary>
		/// Create a CRM profile (Viva+ 5.14.4).
		/// </summary>
		public static readonly string WS_CREATE_CRM_PROFILE = ENDPOINT_ANF + "Pfp-Profile?$expand=Client,User";

        /// <summary>
        /// Card Association WS
        /// </summary>
        public static readonly string WS_CARD_ASSOCIATION = ENDPOINT_ANF + "Pfp-Profile?$expand=User,Client($expand=PharmacyCard)";

        /// <summary>
        /// Password Recovery WS.
        /// </summary>
        public static readonly string WS_PASSWORD_RECOVERY = ENDPOINT_ANF + "ApiUser('{0}')/.ResetPassword";

        /// <summary>
        /// Change Password WS.
        /// </summary>
        public static readonly string WS_CHANGE_PASSWORD = ENDPOINT_ANF + "ApiUser('{0}')/.ChangePassword";

        /// <summary>
        /// Pharmacy Card WS
        /// </summary>
        public static readonly string WS_PHARMACY_CARD = ENDPOINT_ANF + "Pfp-PharmacyCard";

		/// <summary>
		/// GetFavorites
		/// </summary>
		public static readonly string WS_FAVORITES_PHARM = ENDPOINT_ANF_FAVORITES + "getClientPharmacies";
		public static readonly string WS_SET_FAVORITES_PHARM = ENDPOINT_ANF_FAVORITES + "upsertClientPharmacy";

        /// <summary>
        /// User Profile WS
        /// </summary>
		public static readonly string WS_USER_PROFILE = ENDPOINT_ANF + "Pfp-Profile('{0}')?$expand=User,Client($expand=PharmacyCard($expand=OfferedVouchers,Wallet))";

		/// <summary>
		/// Get the profile for a user that does not have a profile.
		/// </summary>
		public static readonly string WS_USER_PROFILE_NO_CARD = ENDPOINT_ANF + "Pfp-Profile('{0}')?$expand=User,Client";

        /// <summary>
        /// User Wallet WS
        /// </summary>
        public static readonly string WS_USER_WALLET = ENDPOINT_ANF + "Pfp-ClientWallet('{0}')?$expand=Vouchers($expand=BurnCondition($expand=Products))";

		public static readonly string WS_USER_VOUCHER = ENDPOINT_ANF + "Pfp-ClientWallet('{0}')/Vouchers('{1}')";
        /// <summary>
        /// Point Movement for user
        /// </summary>
		public static readonly string WS_USER_POINTS_MOVEMENT = WS_PHARMACY_CARD + "(Number='{0}')/Movements?$expand=Card&$filter=Date ge {1} and Date le {2}";

        /// <summary>
        /// Point Movement for user
        /// </summary>
        public static readonly string WS_USER_CARD_UPDATE = ENDPOINT_ANF + "Pfp-PharmacyCard('{0}')";

        /// <summary>
        /// Get user card information 
        /// </summary>
        public static readonly string WS_USER_SUGESTED_FARM = ENDPOINT_ANF + "Pfp-PharmacyCard('{0}')?$expand=Client($expand=PreferentialPharmacy)";

        /// <summary>
        /// Get user profile information with card
        /// </summary>
        public static readonly string WS_GET_USER_PROFILE_WITH_CARD = ENDPOINT_ANF + "Pfp-Profile('{Username}')? $expand=Client($expand=PharmacyCard($expand=Wallet,OfferedVouchers)),User";

        /// <summary>
        /// Get user profile information with card
        /// </summary>
        public static readonly string WS_GET_USER_PROFILE_WITHOUT_CARD = ENDPOINT_ANF + "Pfp-Profile('{Username}')? $expand=Client($expand=PharmacyCard($expand=Wallet,OfferedVouchers)),User";

        /// <summary>
        /// Create user profile 
        /// </summary>
        public static readonly string WS_CREATE_USER_PROFILE_WITHOUT_CARD = ENDPOINT_ANF + "Pfp-Profile?$expand=Client,User";


        #region Vouchers

        /// <summary>
        /// WS that returns the matrix of voucher offers for point trading
        /// </summary>
        public static readonly string WS_VOUCHERS_POINTS_MATRIX = ENDPOINT_ANF + "Pfp-VoucherConversionMatrix";

        /// <summary>
        /// WS that requests a voucher aquisition
        /// </summary>
        public static readonly string WS_VOUCHER_AQUISITION = ENDPOINT_ANF + "Pfp-VoucherPurchase?$expand=Voucher";

        #endregion

        #endregion

        #region Family Account

        /// <summary>
        /// Request Card Association WS.
        /// </summary>
        public static readonly string WS_FAMILY_REQUEST_CARD_ASSOCIATION = ENDPOINT_ANF + "Pfp-PharmacyCardAssociation";

        /// <summary>
        /// Respond to Card Association Request WS.
        /// </summary>
        public static readonly string WS_FAMILY_RESPOND_TO_CARD_ASSOCIATION = ENDPOINT_ANF + "Pfp-PharmacyCardAssociation('{0}')";

        /// <summary>
        /// Get the list of family members.
        /// </summary>
        public static readonly string WS_FAMILY_MEMBERS = ENDPOINT_ANF + "Pfp-PharmacyCard('{0}')/AssociatedCards";

        /// <summary>
        /// Get the list association requests to the family account.
        /// </summary>
        public static readonly string WS_FAMILY_ASSOCIATION_REQUESTS = ENDPOINT_ANF + "Pfp-PharmacyCard('{0}')/RequestedAssociations?$expand=Requester";

		/// <summary>
		/// Get the list association requests to the family account.
		/// </summary>
		public static readonly string WS_GET_VOUCHER_EXPANDED_DETAILS = ENDPOINT_ANF + "Pfp-ClientWallet('{0}')?$expand=Vouchers($expand= BurnCondition($expand = ExclusivePharmacies))";

        #endregion

		#region Schedules

		/// <summary>
		/// WebService that returns the list of medicines for the referenced user.
		/// </summary>
		public static readonly string WS_SCHEDULES_GET_MEDICINE = TOMAS_ENDPOINT + "GetUpdatedMeds";

		/// <summary>
		/// WebService that returns the list of dosing schedules for the referenced user.
		/// </summary>
		public static readonly string WS_SCHEDULES_GET_DOSING_SCHEDULES = TOMAS_ENDPOINT + "GetUpdatedPlans";

		/// <summary>
		/// WebService that returns the list of dosages for the referenced user.
		/// </summary>
		public static readonly string WS_SCHEDULES_GET_DOSAGES = TOMAS_ENDPOINT + "GetUpdatedTomas";

		/// <summary>
		/// WebService that updates a list of medicines.
		/// </summary>
		public static readonly string WS_SCHEDULES_UPDATE_MED = TOMAS_ENDPOINT + "UpdateMed";

		/// <summary>
		/// WebService that updates a list of dosing schedules.
		/// </summary>
		public static readonly string WS_SCHEDULES_UPDATE_SCHEDULES = TOMAS_ENDPOINT + "UpdatePlanoTomas";

		/// <summary>
		/// WebService that updates a list of dosages.
		/// </summary>
		public static readonly string WS_SCHEDULES_UPDATE_DOSAGE = TOMAS_ENDPOINT + "UpdateToma";

		/// <summary>
		/// WebService that adds a dosage to a plan/med/user
		/// </summary>
		public static readonly string WS_SCHEDULES_NEW_TOMAS_TO_USER = TOMAS_ENDPOINT + "AddTomasToUser";

		#endregion

		#region Gamification

		/// <summary>
		/// WebService for the game event center
		/// </summary>
		public static readonly string WS_GAMIFICATION_GAMECENTER = GAMIFICATION_ENDPOINT + "GamEvent";

		#endregion

        #endregion

        #region Settings

        #region User Data

		public static string DISTRICTS_DB_VERSION = "districts_db_version";
		public static readonly int? DISTRICTS_DB_VERSION_DEFAULT = null;


		public static string LAST_LOGIN = "last_login";
		public static readonly string LAST_LOGIN_DEFAULT = null;

        /// <summary>
        /// User image local location
        /// </summary>
        public static string USER_LOCAL_IMG = "user_image_source";
        public static readonly string USER_LOCAL_IMG_DEFAULT = null;


        /// <summary>
        /// Latest access token
        /// </summary>
        public const string ST_USER_ACCESS_TOKEN = "user_access_token";
        public static readonly string ST_USER_ACCESS_TOKEN_DEFAULT = null;

        /// <summary>
        /// Refresh token for the user
        /// </summary>
        public const string ST_USER_REFRESH_TOKEN = "user_refresh_token";
        public static readonly string ST_USER_REFRESH_TOKEN_DEFAULT = null;

        /// <summary>
        /// Access Token TTL (Time-to live)
        /// </summary>
        public const string ST_USER_TOKEN_TTL = "user_token_ttl";
        public static readonly string ST_USER_TOKEN_TTL_DEFAULT = null;

		/// <summary>
		/// Magento token for the user
		/// </summary>
		public const string ST_MG_AUTH_TOKEN = "store_authentication_token";
		public static readonly string ST_MG_AUTH_TOKEN_DEFAULT = null;

		/// <summary>
		/// Magento cookie for the user
		/// </summary>
		public const string ST_MG_AUTH_COOKIE = "store_authentication_cookie";
		public static readonly string ST_MG_AUTH_COOKIE_DEFAULT = null;

		/// <summary>
		/// The selected pharmacy identifier.
		/// </summary>
		public const string ST_MG_PHARMACY_ID = "store_pharmacy_id";
		public static readonly string ST_MG_PHARMACY_ID_DEFAULT = "99999";

		/// <summary>
		/// The name of the selected pharmacy.
		/// </summary>
		public const string ST_MG_PHARMACY_NAME = "store_pharmacy_name";
		public static readonly string ST_MG_PHARMACY_NAME_DEFAULT = "Farmácia ANF";

		/// <summary>
		/// The phone number of the selected pharmacy.
		/// </summary>
		public const string ST_MG_PHARMACY_PHONE = "store_pharmacy_phone";
		public static readonly string ST_MG_PHARMACY_PHONE_DEFAULT = null;

		/// <summary>
		/// The address of the selected pharmacy.
		/// </summary>
		public const string ST_MG_PHARMACY_ADDRESS = "store_pharmacy_address";
		public static readonly string ST_MG_PHARMACY_ADDRESS_DEFAULT = null;

        /// <summary>
        /// Serialized User Object
        /// </summary>
        public const string ST_SERIALIZED_USER_OBJECT = "serialized_user_object";
        public static readonly string ST_SERIALIZED_USER_OBJECT_DEFAULT = null;

        /// <summary>
        /// Defines if this is the app's first run
        /// </summary>
        public const string ST_IS_FIRST_RUN = "is_first_run";
		public static readonly bool ST_IS_FIRST_RUN_DEFAULT = true;

		/// <summary>
		/// I9 authentication token for the user
		/// </summary>
		public const string ST_I9_AUTH_TOKEN = "i9_authentication_token";
		public static readonly string ST_I9_AUTH_TOKEN_DEFAULT = null;
		

        #endregion

		#region Device Data

		/// <summary>
		/// Stores the Parse installation identifier, which keeps track of push subscriptions
		/// </summary>
		public const string ST_PARSE_INSTALLATION_ID = "installation";
		public static readonly string ST_PARSE_INSTALLATION_ID_DEFAULT = null;

		/// <summary>
		/// Stores the name of the currently installed notification service
		/// </summary>
		public const string ST_NOTIFICATIONS_SERVICE = "notification_service";
		public static readonly string ST_NOTIFICATIONS_SERVICE_DEFAULT = null;


		/// <summary>
		/// The Google Analytics anonymous identifier.
		/// </summary>
		public const string ST_GA_CID = "ga_cid";
		public static readonly string ST_GA_CID_DEFAULT = null;

		#endregion

        #region Pharmacy Search

        //key for pharmacy type
		public const string ST_IS_ALL_PHARMACIS = "username_key";
		public static readonly bool ST_IS_ALL_PHARMACIS_DEFAULT = true;

        public const string ST_IS_ONLY_SERVICE_PHARMACIES = "isOnlyServicePharmacies";
        public static readonly bool ST_IS_ONLY_SERVICE_PHARMACIES_DEFAULT = true;

        public const string ST_IS_ONLY_LONG_SCHEDULE_PHARMACIES = "isOnlyLongSchedulePharmacies";
        public static readonly bool ST_IS_ONLY_LONG_SCHEDULE_PHARMACIES_DEFAULT = false;

        //key for type of map/list view of pharmacies
        public const string ST_IS_MAP = "isMap";
		public static readonly bool ST_IS_MAP_DEFAULT = true;

		//last saved user latitude
		public const string ST_LATITUDE = "latitude";
		public static readonly double ST_LATITUDE_DEFAULT = 0;

		//last saved user longitude
		public const string ST_LONGITUDE = "longitude";
		public static readonly double ST_LONGITUDE_DEFAULT = 0;

        #endregion

        #endregion

        #region Key Values

		/// <summary>
		/// Delay value (in miliseconds) used to render the loading view before a page transition starts.
		/// This delay is used because of the slowness of Xamarin.Forms during page transitions.
		/// </summary>
		public static readonly int DEFAULT_LOADING_DELAY = 200;

		/// <summary>
		/// Error code for when the session has expired
		/// </summary>
		public static readonly int MAGENT_EXPIRED_SESSION_ERROR_CODE = 475;

		/// <summary>
		/// Error code for when the FarmID has changed.
		/// </summary>
		public static readonly int MAGENT_EXPIRED_SESSION_FARMID_CODE = 415;

		/// <summary>
		/// Error code for when the ANF refresh token disappears.
		/// </summary>
		public static readonly int MAGENT_EXPIREDANF_REFRESHTOKEN = 409;

		#region Map Values

		/// <summary>
		/// Default radius for the near me feature of the Locator Map (in meters).
		/// </summary>
		public static readonly int LOCATOR_MAP_NEARME_RADIUS = 5000;

		/// <summary>
		/// Default radius for the near me zoom level.
		/// </summary>
		public static readonly int LOCATOR_MAP_NEARME_ZOOMLEVEL = 15;

		#endregion

        #region Biometric Data

        public static readonly int BIOMETRIC_DATA_BASE_CHART_SCALE = 10;

		/// <summary>
		/// The timestamp of the last synchronization with the Biometric Data WS.
		/// </summary>
		public const string ST_SYNCBIOTS = "getbiots";
		public static readonly long ST_SYNCBIOTS_DEFAULT = new DateTime(2015, 1, 1, 0, 0, 1, DateTimeKind.Utc).Ticks;

        #endregion

        #region Family Association Request Status

        public static readonly string FAMILY_ASSOCIATION_REQUEST_STATUS_PENDING = "Pending";

        public static readonly string FAMILY_ASSOCIATION_REQUEST_STATUS_ACCEPTED = "Accepted";

        public static readonly string FAMILY_ASSOCIATION_REQUEST_STATUS_REJECTED = "Rejected";

        #endregion

        #region Document Type

        public static readonly string DOCUMENT_TYPE_NATIONAL_ID = "NationalIdentification";

        public static readonly string DOCUMENT_TYPE_PASSPORT = "Passport";

        #endregion

        #region Gender

        public static readonly string GENDER_MALE = "Male";

        public static readonly string GENDER_FEMALE = "Female";

        #endregion

        #region Voucher Type

        public static readonly string VOUCHER_TYPE_CASH = "Cash";
        
        public static readonly string VOUCHER_TYPE_PERCENTAGE = "Percentage";

        public static readonly string VOUCHER_TYPE_VOUCHER = "Voucher";

		public static readonly string VOUCHER_TYPE_GIFT = "Gift";

		public static readonly string VOUCHER_TYPE_SPONSORED = "Sponsored";

		public static readonly string VOUCHER_TYPE_PHARMACY = "Pharmacy";
		public static readonly string VOUCHER_TYPE_NOTDEFINED = "NotDefined";

        #endregion

        #region Voucher State

        public static readonly string VOUCHER_STATE_AVAILABLE = "Available";

        public static readonly string VOUCHER_STATE_USED = "Used";

        public static readonly string VOUCHER_STATE_EXPIRED = "Expired";

        public static readonly string VOUCHER_STATE_CANCELED = "Canceled";

        #endregion

        #region Voucher Aquisition

        public static readonly string VOUCHER_AQUISITION_PAYMENT_TYPE = "Points";

        public static readonly string VOUCHER_AQUISITION_PHARMACY_CODE = "V0001";

        #endregion

		#region Checkout 

		/// <summary>
		/// ID used for the monetary aquisition type.
		/// </summary>
		public static readonly int CHECKOUT_AQUISITIONTYPE_MONEY = 0;

		/// <summary>
		/// ID used for the points aquisition type.
		/// </summary>
		public static readonly int CHECKOUT_AQUISITIONTYPE_POINTS = 1;

		/// <summary>
		/// Home delivery.
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_HOME = "toogas_anfcarrier";

		/// <summary>
		/// Pharmacy pick-up delivery.
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_PHARMACY = "flatrate";

		/// <summary>
		/// ATM Payment
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_ATM = "mb";

		/// <summary>
		/// Hipay Payment.
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_HIPAY = "hipay";

		/// <summary>
		/// MBWAY Payment
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_MBWAY = "mbway";

		/// <summary>
		/// On-Delivery Payment.
		/// </summary>
		public static readonly string CHECKOUT_DELIVERY_ON_DELIVERY = "cashondelivery";


		#endregion

        #endregion

        #region Messaging center

        public const string MS_LOCATOR_GOT_LOCATION = "got_location";
		public const string MS_LOCATOR_NOT_AUTORIZED = "not_autorized";
		public const string MS_LOCATOR_PHARMACY_TAPPED = "pharmacy_tapped";

		public const string MS_FACEBOOK_LOGIN_SUCCESS = "facebook_login_success";
		public const string MS_FACEBOOK_LOGIN_CANCEL = "facebook_login_cancel";
		public const string MS_FACEBOOK_LOGIN_ERROR = "facebook_login_error";
		public const string MS_FACEBOOK_GET_EMAIL = "facebook_get_email";

		#endregion

		#region Game Center Events

		/// <summary>
		/// Achievement that happens when the user associates or registers a card.
		/// </summary>
		public static readonly string GAMECENTER_EVENT_CARD_ASSOCIATION = "1";

		/// <summary>
		/// Achievement that happens when a user registers a new biometric entry.
		/// </summary>
		public static readonly string GAMECENTER_EVENT_BIOMETRIC_REGISTER = "3";

		/// <summary>
		/// Achievement that happens when the app launches
		/// </summary>
		public static readonly string GAMECENTER_EVENT_APP_LAUNCH = "5";

		/// <summary>
		/// Dashboard event
		/// </summary>
		public static readonly string GAMECENTER_EVENT_DASHBOARD = "99";

		#endregion

		#region Game Center Data

		public const string GAMECENTER_LAST_DISTANCE_CHECK = "gamecenter_health_ping";
		public static readonly long GAMECENTER_LAST_DISTANCE_CHECK_DEFAULT = 0;

		public const string GAMECENTER_HAS_FITNESS_ENABLED = "gamecenter_has_fitness_enabled";
		public static readonly bool GAMECENTER_HAS_FITNESS_ENABLED_DEFAULT = false;

		#endregion

		#region Notification Center

		public const string AZURE_NOTIFICATION_SERVICE = "AZURE";

		#endregion

    }
}
