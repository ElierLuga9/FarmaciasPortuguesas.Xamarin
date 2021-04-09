using ANFAPP.Logic.Models.In;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class AddToFamilyAccountViewModel : FamilyCardAssociationBaseViewModel
    {

        #region Abstract Method Implementation

        /// <summary>
        /// Builds the service request with the modeled inputs.
        /// </summary>
        /// <returns></returns>
        protected override string BuildRequest()
        {
            // Build request object
            FamilyCardAssociationRequestIn requestObj = new FamilyCardAssociationRequestIn()
            {
                Receiver = new FamilyCardAssociationRequestIn.PharmacyCardRequest()
                {
                    Number = Input1
                },
                Requester = new FamilyCardAssociationRequestIn.PharmacyCardRequest()
                {
                    Number = SessionData.PharmacyUser.CardNumber
                },
                FamilyHead = new FamilyCardAssociationRequestIn.PharmacyCardRequest()
                {
                    Number = Input1
                }
            };

            return JsonConvert.SerializeObject(requestObj);
        }

        /// <summary>
        /// Validates the Inputs
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInputs()
        {
            // Input cannot be null
            if (!string.IsNullOrEmpty(Input1)) return true;

            // Send error
            SendError(AppResources.AddToFamilyAccountErrorTitle, AppResources.AddToFamilyAccountErrorTitle);
            return false;
        }

        #endregion

    }
}