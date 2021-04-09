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
    public class BiometricArterialPressureViewModel : BiometricDataViewModel<ArterialPressure>
    {

        #region Properties

        #region View Bindings

        private int _systolicInput;
        public int SystolicInput
        {
            get
            {
                return _systolicInput;
            }
            set
            {
                if (_systolicInput == value) return;

                _systolicInput = value;
                OnPropertyChanged("SystolicInput");
            }
        }

        private int _distolicInput;
        public int DistolicInput
        {
            get
            {
                return _distolicInput;
            }
            set
            {
                if (_distolicInput == value) return;

                _distolicInput = value;
                OnPropertyChanged("DistolicInput");
            }
        }

        private int _bpmInput;
        public int BPMInput
        {
            get
            {
                return _bpmInput;
            }
            set
            {
                if (_bpmInput == value) return;

                _bpmInput = value;
                OnPropertyChanged("BPMInput");
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
        protected override BiometricDataDAO<ArterialPressure> InitializeDAO()
        {
            return new ArterialPressureDAO();
        }

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override BiometricEvaluator<ArterialPressure> InitializeEvaluator(ArterialPressure entry)
        {
            return new ArterialPressureEvaluator(entry, SessionData.BiometricUser);
        }

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected override ArterialPressure BuildNewEntry()
        {
            return new ArterialPressure()
            {
                BPM = BPMInput,
                Systolic = SystolicInput,
                Diastolic = DistolicInput,
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
            if (SystolicInput <= 0) return false;

            // Validate Dystolic
            if (DistolicInput <= 0) return false;

            // Validate BPM
            if (BPMInput <= 0) return false;
            
            return true;
        }

        /// <summary>
        /// Clears the inputs.
        /// </summary>
        public override void ClearInputs()
        {
            DateInput = DateTime.Now;

            SystolicInput = 0;
            DistolicInput = 0;
            BPMInput = 0;
        }

        /// <summary>
        /// Returns a string with the value of the last entry.
        /// </summary>
        public override string GetLastEntryValue()
        {
            return LastEntry != null ? LastEntry.Systolic + "/" + LastEntry.Diastolic : "0";
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
            foreach (ArterialPressure c in Entries)
            {
                if (MinValue > c.Systolic) MinValue = c.Systolic;
                if (MinValue > c.Diastolic) MinValue = c.Diastolic;
                if (MinValue > c.BPM) MinValue = c.BPM;

                if (MaxValue < c.Systolic) MaxValue = c.Systolic;
                if (MaxValue < c.Diastolic) MaxValue = c.Diastolic;
                if (MaxValue < c.BPM) MaxValue = c.BPM;
            }

            // Add and remove 20
            MinValue = IntegerUtils.GetCloserMultipleOf10(Math.Max(0, MinValue - Settings.BIOMETRIC_DATA_BASE_CHART_SCALE));
            MaxValue = IntegerUtils.GetCloserMultipleOf10(MaxValue + Settings.BIOMETRIC_DATA_BASE_CHART_SCALE);

            // Initialize value interval
            var scale = MaxValue - MinValue;
            if (scale == 0) return;

            ValueInterval = (int) Math.Round(scale / 4.0);
            MaxValue = MinValue + (ValueInterval * 4);
        }

        #endregion

    }
}
