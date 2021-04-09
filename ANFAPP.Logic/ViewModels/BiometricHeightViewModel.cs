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
    public class BiometricHeightViewModel : BiometricDataViewModel<Height>
    {

        #region Properties

        #region View Bindings

		private double _valueInterval;
		public double ValueInterval
		{
			get
			{
				return _valueInterval;
			}
			set
			{
				_valueInterval = value;
				OnPropertyChanged("ValueInterval");
			}
		}

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

        private double _maxValue;
        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        private double _minValue;
        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }

        #endregion

        #endregion


        #region DB Loaders

        /// <summary>
        /// Generates a new instance of a DAO of type T.
        /// </summary>
        /// <returns></returns>
        protected override BiometricDataDAO<Height> InitializeDAO()
        {
            return new HeightDAO();
        }

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override BiometricEvaluator<Height> InitializeEvaluator(Height entry)
        {
            return new HeightEvaluator(entry, SessionData.BiometricUser);
        }

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected override Height BuildNewEntry()
        {
            // Round to 2 decimals
            return new Height()
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
            return LastEntry != null ? LastEntry.Value + string.Empty : "0";
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
            foreach (Height h in Entries)
            {
                if (MinValue > h.Value) MinValue = h.Value;
                if (MaxValue < h.Value) MaxValue = h.Value;
            }

            // Add and remove 20
            MinValue = Math.Max(0, MinValue - 0.10);
            MaxValue = MaxValue + 0.10;

			// Initialize value interval
			var scale = MaxValue - MinValue;
			if (scale == 0) return;

			ValueInterval = Math.Round(scale / 4.0, 2);
			MaxValue = MinValue + (ValueInterval * 4);
        }

        #endregion

    }
}
