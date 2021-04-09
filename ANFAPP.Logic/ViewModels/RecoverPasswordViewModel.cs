using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class RecoverPasswordViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;

        #endregion

        #region Properties

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Validate the inputs and atempts to recover the password.
        /// </summary>
        public async void RecoverPassword()
        {
            // Validate inputs
            if (!ValidateInputs()) return;

            try
            {
				//// Get application token
				//var appAuth = await LoginWS.Login(Settings.APPLICATION_USER, Settings.APPLICATION_PASSWORD);
				//// Build the authentication bundle
				//var appBundle = new AuthorizationBundle(appAuth);

                // Call WS to recover password
                await LoginWS.RecoverPassword(Email);

                // Success
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception e)
            {
                // Service error - Post back to UI and show error
				if (OnError != null) OnError(AppResources.RecoverPasswordPageErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Validate if any of the inputs are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(Email))
            {
                if (OnError != null) OnError(AppResources.RecoverPasswordPageErrorTitle,
                    AppResources.RecoverPasswordPageErrorMessage);
                return false;
            }
            else if (!StringUtils.IsEmailValid(Email))
            {
                if (OnError != null) OnError(AppResources.RecoverPasswordPageErrorTitle,
                    AppResources.RecoverPasswordErrorEmailMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all the view forms.
        /// </summary>
        public void ClearForm()
        {
            Email = null;
        }

        #endregion

    }
}
