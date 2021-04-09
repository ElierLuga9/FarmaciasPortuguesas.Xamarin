using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models;
using ANFAPP.Logic.Models.Authorization;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
	public class QuickRegisterViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
		public event OnSuccessEventHandler OnRegistrationSuccess;

        #endregion

        #region Properties

        private string _email ="";
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

		private string _passwordConfirmation;
		public string PasswordConfirmation
		{
			get { return _passwordConfirmation; }
			set
			{
				_passwordConfirmation = value;
				OnPropertyChanged("PasswordConfirmation");
			}
		}

        #endregion

        #region Loaders

        /// <summary>
        /// Validate the inputs and atempts to register the new user.
        /// </summary>
		public async void RegisterUser()
        {
            // Validate inputs
            if (!ValidateInputs()) return;

            try
            {
                // Attempt registration with the ANF WS
				await LoginWS.RegisterUser(new Models.In.UserRegistrationIn() 
				{
					Email = Email,
					Password = Password,
					PasswordConfirmation = PasswordConfirmation,
					AcceptsTermsAndConditions = true
				});

                // If the user was registred the current pharmacy should be reset,
				// because after the login the pharmacy will be changed in most cases and 
				// we don't want to nag the user with a pharmacy changed notification.
				SessionData.UpdatePharmacy(Settings.ST_MG_PHARMACY_ID_DEFAULT, null, null, null, true);

				if (OnRegistrationSuccess != null) OnRegistrationSuccess();
            }
            catch (Exception e)
            {
                // Service error
                if (OnError != null) OnError(null, e.Message);
            }
        }

        /// <summary>
        /// Validate if any of the inputs are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            var password = Password.ToCharArray();

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(PasswordConfirmation))
            {
                if (OnError != null) OnError(null, AppResources.LoginPageErrorMessage);
                return false;
			}
			else if (!string.Equals(Password, PasswordConfirmation))
			{
				if (OnError != null) OnError(null, AppResources.RegisterCardErrorPasswordMessage);
				return false;
			}
			else if(Password.Length < 12)
			{
				if(OnError != null) OnError(null, AppResources.LoginPagePasswordLengthError);
                return false;
			}

            else if (!Regex.IsMatch(Password, @"\d"))
            {
                OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                return false;
            }
            else if (!password.Any(Char.IsLower))
            {
                OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                return false;
            }
            else if (!password.Any(Char.IsUpper))
            {
                OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                return false;
            }
            else if (!password.Any(c => Char.IsSymbol(c) || Char.IsPunctuation(c)))
            {
                OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                return false;
            }

            return true;
        }

        #endregion

    }
}
