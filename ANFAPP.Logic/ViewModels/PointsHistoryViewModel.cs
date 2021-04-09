using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class PointsHistoryViewModel : IViewModel
    {

        #region Constants

        /// <summary>
        /// 1 Year ago from now.
        /// </summary>
		protected readonly DateTime HISTORY_MIN_YEAR = DateTime.Now.AddMonths(-12).ToUniversalTime();

        /// <summary>
        /// Today
        /// </summary>
		protected readonly DateTime HISTORY_MAX_YEAR = DateTime.Now.ToUniversalTime();

        #endregion

        #region Event Handlers

        public event OnSuccessEventHandler OnLoadFinished;
        public event OnErrorEventHandler OnLoadError;
        
        #endregion 

        #region Properties

        private List<PointsMovement> _movements;
        public List<PointsMovement> Movements
        {
            get { return _movements; }
            set
            {
                _movements = value;
                OnPropertyChanged("Movements");
            }
        }

        private bool _noResults;
        public bool NoResults
        {
            get { return _noResults; }
            set
            {
                _noResults = value;
                OnPropertyChanged("NoResults");
            }
        }
        
        #endregion

        #region Loaders

        /// <summary>
        /// Load the points movement for the last year.
        /// </summary>
        public async void LoadData()
        {
            try 
            {
                // Call webservice
                var response = await UserCardWS.GetPointsMovement(SessionData.PharmacyUser.CardNumber, HISTORY_MIN_YEAR, HISTORY_MAX_YEAR);
                
                if (response == null || response.Value == null || response.Value.Count == 0)
                {
                    // No results
                    this.NoResults = true;
                    Movements = null;
                }
                else
                {
                    // Order results by date
                    Movements = response.Value.OrderByDescending(p => p.Date).ToList();

                    // Initialize Order
                    for (int i = 0; i < Movements.Count; i++)
                    {
                        Movements[i].Order = i + 1;
                        Movements[i].MovementType = GetMovementType(Movements[i].Type);
                    }
                }

                // Load success
                if (OnLoadFinished != null) OnLoadFinished();
            }
            catch (Exception e)
            {
                // Service error
                this.NoResults = true;
                if (OnLoadError != null) OnLoadError(AppResources.PointsHistoryErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Returns the equivalent movement type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetMovementType(string type)
        {
            if (string.IsNullOrEmpty(type)) return null;

            switch (type)
            {
                case "AddSale": return AppResources.PointsHistoryMovementTypeAddSale;
                case "UndoSale": return AppResources.PointsHistoryMovementTypeUndoSale;
                case "RedeemPoints": return AppResources.PointsHistoryMovementTypeRedeemPoints;
                case "UndoRedeem": return AppResources.PointsHistoryMovementTypeUndoRedeem;
                case "BuyVoucher": return AppResources.PointsHistoryMovementTypeBuyVoucher;
                case "UndoBuyVoucher": return AppResources.PointsHistoryMovementTypeUndoBuyVoucher;
				case "Gamification": return AppResources.PointsHistoryMovementTypeGamification;
            }

            return null;
        }

        #endregion

    }
}
