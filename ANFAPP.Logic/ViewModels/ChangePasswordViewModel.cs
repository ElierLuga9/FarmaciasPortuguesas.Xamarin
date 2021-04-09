using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class ChangePasswordViewModel : IViewModel
    {

        #region Events

        public event OnErrorEventHandler OnError;
        public event OnSuccessEventHandler OnSuccess;

        #endregion

        #region Properties

        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged();
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _newPasswordConfirmation;
        public string NewPasswordConfirmation
        {
            get { return _newPasswordConfirmation; }
            set
            {
                _newPasswordConfirmation = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Validate the inputs and atempts to recover the password.
        /// </summary>
        public async void ChangePassword()
        {
            // Validate inputs
            if (!ValidateInputs()) return;

            try
            {
                // Call WS to change the password
                await LoginWS.ChangePassword(
                    SessionData.PharmacyUser.Username,
                    OldPassword,
                    NewPassword,
                    SessionData.UserAuthentication);

                // Success
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception e)
            {
                // Service error - Post back to UI and show error
                if (OnError != null) OnError(AppResources.RecoverPasswordMessageTitle, e.Message);
            }
        }

        /// <summary>
        /// Validate if any of the inputs are filled.
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(NewPasswordConfirmation))
            {
                // Validate inputs
                if (OnError != null) OnError(AppResources.ChangePasswordMessateTitle,
                    AppResources.ChangePasswordEmptyFieldsMessage);

                return false;
            }
            else
            {
                if (!NewPassword.Equals(NewPasswordConfirmation))
                {
                    // Validate if the new password equals its confirmation
                    if (OnError != null) OnError(AppResources.ChangePasswordMessateTitle,
                        AppResources.ChangePasswordErrorMessage);

                    return false;
                }
                if (NewPassword.Length < 12)
                {
                    if (OnError != null) OnError(AppResources.ChangePasswordMessateTitle,
                        AppResources.ChangePasswordSizeErrorMessage);

                    return false;
                }

                if (!Regex.IsMatch(NewPassword, @"\d"))
                {
                    OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                    return false;
                }

                var password = NewPassword.ToCharArray();

                if (!password.Any(Char.IsLower))
                {
                    OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                    return false;
                }

                if (!password.Any(Char.IsUpper))
                {
                    OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                    return false;
                }

                if (!password.Any(c => Char.IsSymbol(c) || Char.IsPunctuation(c)))
                {
                    OnError?.Invoke(null, AppResources.LoginPageRequirementsError);
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}
