using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class FamilyAccountMasterViewModel : IViewModel
    {

        #region Properties

        private ObservableCollection<AssociationRequestOut> _associationRequests;
        public ObservableCollection<AssociationRequestOut> AssociationRequests
        {
            get { return _associationRequests; }
            set
            {
                _associationRequests = value;
                OnPropertyChanged("AssociationRequests");
            }
        }

        private List<FamilyMemberOut> _familyMembers;
        public List<FamilyMemberOut> FamilyMembers
        {
            get { return _familyMembers; }
            set
            {
                _familyMembers = value;
                OnPropertyChanged("FamilyMembers");
            }
        }

        public bool IsFamilyHead
        {
            get { return SessionData.PharmacyUser.IsFamilyHead; }
        }

        #endregion

        #region Events

        public OnErrorEventHandler OnError;
        public OnSuccessEventHandler OnSuccess;
        public OnSuccessEventHandler OnAssociationSuccess;
        
        #endregion

        #region Loaders

        /// <summary>
        /// Call the webservice and get the list of family members for the current user.
        /// </summary>
        public async void LoadData()
        {
            try
            {
                if (SessionData.PharmacyUser.IsFamilyCard)
                {
                    // Load Family Members
                    FamilyMembers = await LoadFamilyMembers();
                }

                // Load Association Requests
                AssociationRequests = await LoadAssociationRequests();
                if (AssociationRequests != null && AssociationRequests.Count == 0) AssociationRequests = null;
                
                // Load Success.
                if (OnSuccess != null) OnSuccess();
            }
            catch (Exception e)
            {
                // Service Error
                OnError(AppResources.FamilyAccountMasterErrorTitle, e.Message);
            }

        }

        /// <summary>
        /// Load the family members.
        /// </summary>
        /// <returns></returns>
        public async Task<List<FamilyMemberOut>> LoadFamilyMembers()
        {
            // Get family members
            var response = await FamilyAccountWS.GetFamilyMembers(SessionData.PharmacyUser.CardNumber);

            // Initialize Order
            var familyMembers = response.Value;
            if (familyMembers != null)
            {
                for (int i = 0; i < familyMembers.Count; i++)
                {
                    // Starts with 1
                    familyMembers[i].Order = i + 1;
                }
            }

            return familyMembers;
        }

        /// <summary>
        /// Load association requests.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<AssociationRequestOut>> LoadAssociationRequests()
        {
            // Get association requests
            var requestsResponse = await FamilyAccountWS.GetAssociationPendingRequests(SessionData.PharmacyUser.CardNumber);

            var requests = requestsResponse.Value;
            if (requests != null)
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    // Starts with 0
                    requests[i].Order = i;
                }
            }

            // Transform it into an observable collection
            return new ObservableCollection<AssociationRequestOut>(requests);
        }

        #endregion

        #region Association Requests

        /// <summary>
        /// Cancel the referenced association request.
        /// </summary>
        /// <param name="request"></param>
        public async void CancelAssociationRequest(AssociationRequestOut request) 
        {
            if (request == null) return;

            // Build request
            AssociationRequestResponseIn requestObj = new AssociationRequestResponseIn()
            {
                Status = Settings.FAMILY_ASSOCIATION_REQUEST_STATUS_REJECTED
            };

            try
            {
                // Call Webservice
                await FamilyAccountWS.RespondToCardAssociation(request.Id, JsonConvert.SerializeObject(requestObj));

                // Sucessful - remove it from the list
                RemoveAssociationRequestFromList(request);

                if (OnAssociationSuccess != null) OnAssociationSuccess();
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(AppResources.FamilyAccountMasterErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Accept the referenced association request.
        /// </summary>
        /// <param name="request"></param>
        public async void AcceptAssociationRequest(AssociationRequestOut request) 
        {
            if (request == null) return;

            // Build request
            AssociationRequestResponseIn requestObj = new AssociationRequestResponseIn()
            {
                Status = Settings.FAMILY_ASSOCIATION_REQUEST_STATUS_ACCEPTED
            };

            try
            {
                // Call Webservice
                await FamilyAccountWS.RespondToCardAssociation(request.Id, JsonConvert.SerializeObject(requestObj));

                // Sucessful - remove it from the list
                RemoveAssociationRequestFromList(request);

                if (OnAssociationSuccess != null) OnAssociationSuccess();
            }
            catch (Exception e)
            {
                if (OnError != null) OnError(AppResources.FamilyAccountMasterErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Removes the referenced association request from the list. </br>
        /// Sets the list to null, if the element was the last one in it.
        /// </summary>
        /// <param name="request"></param>
        protected void RemoveAssociationRequestFromList(AssociationRequestOut request)
        {
            if (AssociationRequests == null || request == null) return;

            // Remove request from the list
            AssociationRequests.Remove(request);

            // If list is empty, set it as null
            if (AssociationRequests.Count == 0)
            {
                AssociationRequests = null;
            }
            else
            {
                // Recalculate order id's
                for (int i = 0; i < AssociationRequests.Count; i++)
                {
                    // Starts with 0
                    AssociationRequests[i].Order = i;
                }

                // Force list to update
                var aux = AssociationRequests;
                AssociationRequests = null;
                AssociationRequests = aux;
            }
        }

        #endregion

    }
}
