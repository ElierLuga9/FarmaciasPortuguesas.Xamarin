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
    public class BiometricGlicemiaViewModel : BiometricDataViewModel<Glicemia>
    {

        #region Properties

        #region View Bindings

        private int _valueInput;
        public int ValueInput
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

        private bool _unfed;
        public bool Unfed
        {
            get
            {
                return _unfed;
            }
            set
            {
                if (_unfed == value) return;

                _unfed = value;
                OnPropertyChanged("Unfed");
            }
        }

		private List<Glicemia> _beforeEntries;
		public List<Glicemia> BeforeEntries 
		{
			get { return _beforeEntries; }
			set { 
				_beforeEntries = value;
				OnPropertyChanged ("BeforeEntries");
			}
		}

		private List<Glicemia> _afterEntries;
		public List<Glicemia> AfterEntries 
		{
			get { return _afterEntries; }
			set { 
				_afterEntries = value;
				OnPropertyChanged ("AfterEntries");
			}
		}

        #endregion

        #endregion

        #region DB Loaders

        /// <summary>
        /// Generates a new instance of a DAO of type T.
        /// </summary>
        /// <returns></returns>
        protected override BiometricDataDAO<Glicemia> InitializeDAO()
        {
            return new GlicemiaDAO();
        }

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override BiometricEvaluator<Glicemia> InitializeEvaluator(Glicemia entry)
        {
            return new GlicemiaEvaluator(entry, SessionData.BiometricUser);
        }

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected override Glicemia BuildNewEntry()
        {
            return new Glicemia()
            {
                Value = ValueInput,
                CreationDate = DateInput,
                Unfed = Unfed,
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

            Unfed = false;
            ValueInput = 0;
        }

        /// <summary>
        /// Returns a string with the value of the last entry.
        /// </summary>s
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
            foreach (Glicemia c in Entries)
            {
                if (MinValue > c.Value) MinValue = c.Value;
                if (MaxValue < c.Value) MaxValue = c.Value;
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

		public async override Task LoadData()
		{
			await base.LoadData ();

			if (Entries == null)
				return;

			var before = new List<Glicemia> ();
			var after = new List<Glicemia> ();

			foreach (Glicemia item in Entries) {
				if (item.Unfed) {
					before.Add (item);	
				} else {
					after.Add (item);
				}
			}

			BeforeEntries = before;
			AfterEntries = after;
		}

        #endregion

    }
}
