using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public abstract class FamilyCardAssociationBaseViewModel : IViewModel
    {

        #region Properties

        private string _input1;
        public string Input1
        {
            get { return _input1;  }
            set
            {
                _input1 = value;
                OnPropertyChanged("Input1");
            }
        }

        #endregion

        #region EventHandler

        public event OnSuccessEventHandler OnSuccess;
        public event OnErrorEventHandler OnError;

        #endregion

        #region Loaders

        public async void RequestCardAssociation()
        {
            // Validate the inputs
            if (!ValidateInputs()) return;

            try
            {
                // Call webservice
                var response = await FamilyAccountWS.RequestCardAssociation(BuildRequest());

                // Load success
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception e)
            {
                // Service error
                SendError(AppResources.CreateFamilyAccountErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Send error event
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        protected void SendError(string title, string message)
        {
            if (OnError != null) OnError(title, message);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Builds the service request with the modeled inputs.
        /// </summary>
        /// <returns></returns>
        protected abstract string BuildRequest();
        
        /// <summary>
        /// Validates the Inputs
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidateInputs();
        
        #endregion
        
    }
}
