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
    public class BiometricIMCViewModel : BiometricDataViewModel<IMC>
    {

        #region Properties

        private BiometricHeightViewModel _heightVM;
        public BiometricHeightViewModel HeightVM
        {
            get
            {
                return _heightVM;
            }
            set
            {
                if (_heightVM == value) return;

                _heightVM = value;
                OnPropertyChanged("HeightVM");
            }
        }

        private BiometricWeightViewModel _weightVM;
        public BiometricWeightViewModel WeightVM
        {
            get
            {
                return _weightVM;
            }
            set
            {
                if (_weightVM == value) return;

                _weightVM = value;
                OnPropertyChanged("WeightVM");
            }
        }

        #endregion

        #region IMC Loaders

        /// <summary>
        /// Invalidate the current IMC Table and re-generate it with the new values.
        /// </summary>
        public async Task ReloadIMCTable()
        {
            // Clear IMC Table
            await _db.DeleteAll();
           
            // Get Weight and Height lists
            var weights = await new WeightDAO().GetAllOrderedByDateDesc(SessionData.BiometricUser.Id);
            var heights = await new HeightDAO().GetAllOrderedByDateDesc(SessionData.BiometricUser.Id);

            // Validate if enough data
            if (weights == null || weights.Count == 0 || heights == null || heights.Count == 0)
            {
                await LoadData();
                return;
            }

            await Task.Run(async () =>
            {
                // Generate IMC's, starting from the most recent height or weight.
				// http://issue.innovagency.com/view.php?id=20568
				// https://consulting.glintt.com/mantis/view.php?id=33717#c82855
                List<IMC> calculatedIMC = new List<IMC>();

				// Tipically, an adult height shouldn't vary...
				var fillHeight = new Height {
					Value = heights.Last().Value,
					CreationDate = DateTime.MinValue.ToUniversalTime(),
					UserId = SessionData.BiometricUser.Id
				};
				while (heights.Count < weights.Count) {
					heights.Add(fillHeight);
				}

				// Create weight/height tuples from the ordered lists.
				IEnumerable<Tuple<Weight, Height>> pairs = weights.Zip(heights, (w, h) => Tuple.Create(w, h));

				foreach (Tuple<Weight, Height> t in pairs)
				{
					Weight w = t.Item1;
					Height h = t.Item2;

					var wTs = DateTime.SpecifyKind(w.CreationDate, DateTimeKind.Utc); 
					var hTs = DateTime.SpecifyKind(h.CreationDate, DateTimeKind.Utc);

					// Generate new IMC
					calculatedIMC.Add(new IMC()
						{
							Value = CalculateIMC(w.Value, h.Value),
							CreationDate = (wTs > hTs ? wTs : hTs),
							UserId = SessionData.BiometricUser.Id
						});
				}

                // Insert all the IMC's into the DB
                if (calculatedIMC.Count > 0)
                {
                    await _db.InsertAll(calculatedIMC);
                    await LoadData();
                }
            });
        }

        /// <summary>
        /// Calculate the IMC with the referenced weight and height.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected double CalculateIMC(double weight, double height)
        {
            // Calculate the IMC and round it to 2 decimal places
            return Math.Round((weight / (Math.Pow(height, 2))), 2);
        }

        #endregion

        #region DB Loaders

        /// <summary>
        /// Generates a new instance of a DAO of type T.
        /// </summary>
        /// <returns></returns>
        protected override BiometricDataDAO<IMC> InitializeDAO()
        {
            return new IMCDAO();
        }

        /// <summary>
        /// Generates a new instance of an evaluator of type T.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override BiometricEvaluator<IMC> InitializeEvaluator(IMC entry)
        {
            return new IMCEvaluator(entry, SessionData.BiometricUser);
        }

        /// <summary>
        /// Build a new entry of type T, with the correct input parameters.
        /// </summary>
        /// <returns></returns>
        protected override IMC BuildNewEntry()
        {
            return null;
        }

        /// <summary>
        /// Validate the inputs.
        /// </summary>
        /// <returns></returns>
        public override bool InputsValid()
        {
            return true;
        }

        /// <summary>
        /// Clears the inputs.
        /// </summary>
        public override void ClearInputs() { }

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
            foreach (IMC c in Entries)
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
