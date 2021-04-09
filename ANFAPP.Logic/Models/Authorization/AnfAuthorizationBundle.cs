using ANFAPP.Logic.Models.Out;
using System;

namespace ANFAPP.Logic.Models.Authorization
{	
	public class AnfAuthorizationBundle : IAuthorizationBundle
	{
		public static readonly string MG_PUBLIC_TOKEN = "47f9adeaf5c22ab89901ad0751b75334";

		#region Properties

		/// <summary>
		/// Access Token, to be used in authenticated services.
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		/// Refresh Token, to be used in the renewal of the AccessToken.
		/// </summary>
		public string RefreshToken { get; set; }

		/// <summary>
		/// Time To Live for the current AccessToken.
		/// </summary>
		public DateTime TTL { get; set; }

		/// <summary>
		/// The Magento token identifier.
		/// </summary>
		private string _storeToken;
		public string StoreToken { 
			get { return _storeToken; }
			set { _storeToken = value ?? MG_PUBLIC_TOKEN; } 
		}

		public string StoreCookie { get; set; }

		/// <summary>
		/// Token for the I9 WebServices (Gamification, Schedulers and Biometrics)
		/// </summary>
		public string I9Token { get; set; }

		#endregion

		#region Constructors

		public AnfAuthorizationBundle() 
		{ 
			StoreToken = MG_PUBLIC_TOKEN;
		}

		public AnfAuthorizationBundle(AuthorizationOut outModel) : this()
        {
            this.AccessToken = outModel.Authorization.AccessToken;
            this.RefreshToken = outModel.Authorization.RefreshToken;
            this.TTL = DateTime.Now.AddMinutes(outModel.Authorization.ExpiresIn);
        }

		#endregion

		public string GetAuthorizationString() 
		{
			return AccessToken;
		}

		/// <summary>
		/// Returns the token that authenticates the user with the e-commerce web services.
		/// </summary>
		public string GetECAuthorizationString()
		{
			return StoreToken;
		}
	}
}

