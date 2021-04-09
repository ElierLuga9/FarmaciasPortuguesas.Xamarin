using ANFAPP.Logic.BusinessLogic.BiometricData;
using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public abstract class BiometricDataViewModel<T> : IViewModel where T : BiometricBase, new()
    {

        #region Properties

        protected BiometricDataDAO<T> _db;

        private T _lastEntry;
        public T LastEntry
        {
            get { return _lastEntry; }
            set
            {
                _lastEntry = value;
                OnPropertyChanged("LastEntry");
                OnPropertyChanged("LastEntryValue");
                OnPropertyChanged("IsTableVisible");
                OnPropertyChanged("IsGraphVisible");
				OnPropertyChanged("HasData");
            }
        }

        private List<T> _entries;
        public List<T> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;
                OnPropertyChanged("Entries");
                OnPropertyChanged("DateInterval");
            }
        }

        private BiometricEvaluator<T> _entryEvaluator;
        public BiometricEvaluator<T> EntryEvaluator 
        {
            get { return _entryEvaluator; }
            set
            {
                _entryEvaluator = value;
                OnPropertyChanged("EntryEvaluator");
            }
        }

        private bool _isGraphToggled = true;
        public bool IsGraphToggled
        {
            get { return _isGraphToggled; }
            set
            {
                _isGraphToggled = value;
                OnPropertyChanged("IsGraphToggled");
                OnPropertyChanged("IsTableVisible");
                OnPropertyChanged("IsGraphVisible");
				OnPropertyChanged("HasData");
                
                // Hack, to make sure the graph renders itself
                var aux = Entries;
                Entries = null;
                Entries = aux;
                
            }
        }

        public string LastEntryValue
        {
            get
            {
                return GetLastEntryValue();
            }
        }

        public bool IsTableVisible
        {
            get { return !IsGraphToggled && LastEntry != null; }
        }

        public bool IsGraphVisible
        {
            get { return IsGraphToggled && LastEntry != null; }
        }

		public bool HasData
		{
			get { return IsTableVisible || IsGraphToggled; }
		}

        private int _maxValue;
        public int MaxValue
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

        private int _minValue;
        public int MinValue
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

        private int _valueInterval;
        public int ValueInterval
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

        public double DateInterval
        {
            get
            {
                if (Entries == null || Entries.Count == 0) return 0;

                return (Entries.Count <= 5) ? 1 : Entries.Count / 5.0;
            }
        }

		private TimeSpan _timeInput = DateTime.Now.TimeOfDay;
		public TimeSpan TimeInput
		{
			get
			{
				return _timeInput;
			}
			set
			{
				if (_timeInput == value) return;

				_timeInput = value;
				OnPropertyChanged("TimeInput");
			}
		}

        #endregion

        #region Event Handlers

        public OnSuccessEventHandler OnLoadComplete;

        #endregion

        public BiometricDataViewModel() {
            // Initialize DB
            _db = InitializeDAO();

            // Initialize empty Evaluator
            EntryEvaluator = InitializeEvaluator(null);
        }

        #region DB Loaders

        /// <summary>
        /// Load the list of entries from the database.
        /// </summary>
        public async virtual Task LoadData()
        {
			if (SessionData.BiometricUser != null) 
			{
				var newEntries = await _db.GetAllOrderedByDateDesc (SessionData.BiometricUser.Id);

				if (newEntries != null && newEntries.Count > 0) {
					// Initialize Order
					for (int i = 0; i < newEntries.Count; i++) {
						newEntries [i].Order = i + 1;
					}
				}

				Entries = newEntries;
			}

            LastEntry = (Entries != null && Entries.Count > 0) ? Entries.First() : null;
            EntryEvaluator = InitializeEvaluator(LastEntry);

            // Initialize the Max and Min Values
            InitMaxAndMinValues();

            OnPropertyChanged("LastEntryValue");
        }

        /// <summary>
        /// Insert a new entry into the database.
        /// </summary>
		public async Task InsertNewEntry(bool fromPharmacy = false, DateTime? creationDate = null)
        {
            // Validate the inputs
            if (!InputsValid()) 
			{
				if (OnLoadComplete != null) OnLoadComplete();
				return;
			}

            // Build new entry
            T newEntry = BuildNewEntry();

            // Clear the inputs
            ClearInputs();

			if (creationDate.HasValue) {	
				newEntry.CreationDate = NormalizeDate (creationDate.Value, TimeSpan.Zero);
			} else {
				// Normalize Date
				newEntry.CreationDate = NormalizeDate (newEntry.CreationDate, TimeInput);			
			}

			// Set the pharmacy flag.
			newEntry.PharmacyFlag = fromPharmacy;

			// Validate if it is an update
			//var update = await _db.ExistsWithSameDate(newEntry);

            // Insert into the database
            await _db.InsertOrUpdateByDate(newEntry);

			// Gamification Achievement! - EVENT_3 - Register a new biometric entry
			// if (!update) GameCenterWS.PostEventAsync(Settings.GAMECENTER_EVENT_BIOMETRIC_REGISTER);

            // Reload data
            await LoadData();

            // Notify
            if (OnLoadComplete != null) OnLoadComplete();
        }

        /// <summary>
        /// Remove an entry from the database.
        /// </summary>
        /// <param name="entry"></param>
        public async void RemoveEntry(T entry)
        {
            // Delete entry
            await _db.Delete(entry);

            // Reload data
            await LoadData();

            // Notify
            if (OnLoadComplete != null) OnLoadComplete();
        }

		public async Task RemoveAllEntries()
		{
			await _db.DeleteAll ();

			// Reload data
			await LoadData();

			// Notify
			if (OnLoadComplete != null) OnLoadComplete();
		}

        /// <summary>
        /// Normalize the input Date and Time components.
		/// 
		/// The input date is in local time. This methods yields the combined `date` and `timeofday` in UTC.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected DateTime NormalizeDate(DateTime date, TimeSpan timeofday)
        {
			DateTime dt;
			if (timeofday == TimeSpan.Zero) {
				dt = date;
			} else {
				dt = new DateTime(date.Year, date.Month, date.Day, timeofday.Hours, timeofday.Minutes, timeofday.Seconds, date.Kind);
			}
			return dt.ToUniversalTime ();
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Generates a new instance of a DAO of type T.
        /// </summary>
        /// <returns></returns>
        protected abstract BiometricDataDAO<T> InitializeDAO();

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected abstract BiometricEvaluator<T> InitializeEvaluator(T entry);

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected abstract T BuildNewEntry();

        /// <summary>
        /// Validate the inputs.
        /// </summary>
        /// <returns></returns>
        public abstract  bool InputsValid();

        /// <summary>
        /// Clears the inputs.
        /// </summary>
        public abstract void ClearInputs();

        /// <summary>
        /// Returns a string with the value of the last entry.
        /// </summary>
        public abstract string GetLastEntryValue();

        /// <summary>
        /// Initializes the maximum and minimum values, to be used in the Y axis for the chart.
        /// </summary>
        protected abstract void InitMaxAndMinValues();

        #endregion

    }
}
