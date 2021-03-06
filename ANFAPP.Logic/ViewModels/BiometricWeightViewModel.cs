using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class BiometricWeightViewModel : BiometricDataViewModel<Weight>
    {

        #region Properties

        #region View Bindings

        private double _valueInput;
        public double ValueInput
        {
            get
            {
                return _valueInput;
            }
            set
            {
                if (_valueInput == value) return;

                _valueInput = value;
                OnPropertyChanged("ValueInput");
            }
        }

        private DateTime _dateInput = DateTime.Now;
        public DateTime DateInput
        {
            get
            {
                return _dateInput;
            }
            set
            {
                if (_dateInput == value) return;

                _dateInput = value;
                OnPropertyChanged("DateInput");
            }
        }

        #endregion

        #endregion

        #region DB Loaders

        /// <summary>
        /// Generates a new instance of a DAO of type T.
        /// </summary>
        /// <returns></returns>
        protected override BiometricDataDAO<Weight> InitializeDAO()
        {
            return new WeightDAO();
        }

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override BiometricEvaluator<Weight> InitializeEvaluator(Weight entry)
        {
            return new WeightEvaluator(entry, SessionData.BiometricUser);
        }

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected override Weight BuildNewEntry()
        {
            // Round to 2 decimals
            return new Weight()
            {
                Value = Math.Round(ValueInput, 2),
                CreationDate = DateInput,
                UserId = SessionData.BiometricUser.Id
            };
        }

        /// <summary>
        /// Validate the inputs.
        /// </summary>
        /// <returns></returns>
        public override bool InputsValid()
        {
            // Validate Systolic
            if (ValueInput <= 0) return false;
            
            return true;
        }

        /// <summary>
        /// Clears the inputs.
        /// </summary>
        public override void ClearInputs()
        {
            DateInput = DateTime.Now;

            ValueInput = 0;
        }

        /// <summary>
        /// Returns a string with the value of the last entry.
        /// </summary>
        public override string GetLastEntryValue()
        {
            return LastEntry != null ? string.Format("{0:0.#}", LastEntry.Value) : "0";
        }

        /// <summary>
        /// Initializes the maximum and minimum values, to be used in the Y axis for the chart.
        /// </summary>
        protected override void InitMaxAndMinValues()
        {
            // Reset values
            MinValue = int.MaxValue;
            MaxValue = int.MinValue;

            if (Entries == null || Entries.Count == 0) return;

            // Find the max and min values
            foreach (Weight c in Entries)
            {
                if (MinValue > c.Value) MinValue = (int)Math.Round(c.Value);
                if (MaxValue < c.Value) MaxValue = (int)Math.Round(c.Value);
            }

            // Add and remove 20
            MinValue = IntegerUtils.GetCloserMultipleOf10(Math.Max(0, MinValue - Settings.BIOMETRIC_DATA_BASE_CHART_SCALE));
            MaxValue = IntegerUtils.GetCloserMultipleOf10(MaxValue + Settings.BIOMETRIC_DATA_BASE_CHART_SCALE);

            // Initialize value interval
            var scale = MaxValue - MinValue;
            if (scale == 0) return;

            ValueInterval = (int)Math.Round(scale / 4.0);
            MaxValue = MinValue + (ValueInterval * 4);
        }

        #endregion

    }
}
