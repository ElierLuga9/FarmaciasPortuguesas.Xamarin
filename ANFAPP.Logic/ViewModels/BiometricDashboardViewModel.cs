using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.EventHandlers;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;

namespace ANFAPP.Logic.ViewModels
{
    public class BiometricDashboardViewModel : IViewModel
    {

        #region Properties

        public BiometricIMCViewModel IMCVM { get; set; }
        
        public BiometricWeightViewModel WeightVM { get; set; }
        public BiometricHeightViewModel HeightVM { get; set; }

        private BiometricArterialPressureViewModel _arterialPressureVM;
        public BiometricArterialPressureViewModel ArterialPressureVM
        {
            get
            {
                return _arterialPressureVM;
            }
            set
            {
                if (_arterialPressureVM == value) return;

                _arterialPressureVM = value;
                OnPropertyChanged("ArterialPressureVM");
            }
        }

        private BiometricAbdominalPerimeterViewModel _abdominalPerimeterVM;
        public BiometricAbdominalPerimeterViewModel AbdominalPerimeterVM
        {
            get
            {
                return _abdominalPerimeterVM;
            }
            set
            {
                if (_abdominalPerimeterVM == value) return;

                _abdominalPerimeterVM = value;
                OnPropertyChanged("AbdominalPerimeterVM");
            }
        }

        private BiometricCholesterolViewModel _cholesterolVM;
        public BiometricCholesterolViewModel CholesterolVM
        {
            get
            {
                return _cholesterolVM;
            }
            set
            {
                if (_cholesterolVM == value) return;

                _cholesterolVM = value;
                OnPropertyChanged("CholesterolVM");
            }
        }

        private BiometricTriglyceridesViewModel _triglyceridesVM;
        public BiometricTriglyceridesViewModel TriglyceridesVM
        {
            get
            {
                return _triglyceridesVM;
            }
            set
            {
                if (_triglyceridesVM == value) return;

                _triglyceridesVM = value;
                OnPropertyChanged("TriglyceridesVM");
            }
        }

        private BiometricGlicemiaViewModel _glicemiaVM;
        public BiometricGlicemiaViewModel GlicemiaVM
        {
            get
            {
                return _glicemiaVM;
            }
            set
            {
                if (_glicemiaVM == value) return;

                _glicemiaVM = value;
                OnPropertyChanged("GlicemiaVM");
            }
        }

        #endregion

		#region EventHandlers

		public OnLoadStartEventHandler OnLoadStart;
		public OnSuccessEventHandler OnLoadComplete = delegate {};

		#endregion

		public async Task BioSync() 
		{
			if (SessionData.PharmacyUser == null || string.IsNullOrEmpty (SessionData.PharmacyUser.CardNumber)) return;

			if (null != OnLoadStart) OnLoadStart();

			try 
			{
				// DEBUG: "44426687"
				var result = await BioWS.GetBio (SessionData.PharmacyUser.CardNumber, SessionData.BiometricDataTS);
				DateTime latest = SessionData.BiometricDataTS;

				foreach (Sample bioSample in result.Samples) {

					DateTime utcRecordDate = DateTime.Parse(bioSample.RECDATE, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
					utcRecordDate = utcRecordDate.ToUniversalTime();
					if (latest < utcRecordDate) {
						latest = utcRecordDate;
					}

					DateTime utcDate = DateTime.Parse(bioSample.DATE, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
					utcDate = utcDate.ToUniversalTime();

					switch ((BiometricType)bioSample.BIOID) {
					case BiometricType.Weight:
						if (null != WeightVM) {
							WeightVM.ValueInput = bioSample.PAR1.GetValueOrDefault();
							WeightVM.DateInput = utcDate;
							WeightVM.TimeInput = utcDate.TimeOfDay;
							await WeightVM.InsertNewEntry(true, utcDate);
						}
						break;

					case BiometricType.Height:
						if (null != HeightVM) {
							HeightVM.ValueInput = bioSample.PAR1.GetValueOrDefault();
							HeightVM.DateInput = utcDate;
							HeightVM.TimeInput = utcDate.TimeOfDay;
							await HeightVM.InsertNewEntry(true, utcDate);
						}
						break;

					case BiometricType.BloodPressure:
						if (null != ArterialPressureVM) {
							ArterialPressureVM.SystolicInput = (int)bioSample.PAR1.GetValueOrDefault();
							ArterialPressureVM.DistolicInput = (int)bioSample.PAR2.GetValueOrDefault();
							ArterialPressureVM.BPMInput = (int)bioSample.PAR3.GetValueOrDefault();
							ArterialPressureVM.DateInput = utcDate;
							ArterialPressureVM.TimeInput = utcDate.TimeOfDay;
							await ArterialPressureVM.InsertNewEntry(true, utcDate);
						}
						break;

					case BiometricType.Cholesterol:
						if (null != CholesterolVM) {
							CholesterolVM.ValueInput = (int)bioSample.PAR1.GetValueOrDefault();
							CholesterolVM.DateInput = utcDate;
							CholesterolVM.TimeInput = utcDate.TimeOfDay;
							await CholesterolVM.InsertNewEntry(true, utcDate);
						}
						break;

					case BiometricType.Glicemia:
						if (null != GlicemiaVM) {
							GlicemiaVM.ValueInput = (int)bioSample.PAR1.GetValueOrDefault();
							GlicemiaVM.Unfed = bioSample.BPAR.GetValueOrDefault();
							GlicemiaVM.DateInput = utcDate;
							GlicemiaVM.TimeInput = utcDate.TimeOfDay;
							await GlicemiaVM.InsertNewEntry(true, utcDate);
						}
						break;

					case BiometricType.AbdominalPerimeter:
						if (null != AbdominalPerimeterVM) {
							AbdominalPerimeterVM.ValueInput = (int)bioSample.PAR1.GetValueOrDefault();
							AbdominalPerimeterVM.DateInput = utcDate;
							AbdominalPerimeterVM.TimeInput = utcDate.TimeOfDay;
							await AbdominalPerimeterVM.InsertNewEntry(true, utcDate);
						}

						break;
					case BiometricType.Triglycerides:
						if (null != TriglyceridesVM) {
							TriglyceridesVM.ValueInput = (int)bioSample.PAR1.GetValueOrDefault();
							TriglyceridesVM.DateInput = utcDate;
							TriglyceridesVM.TimeInput = utcDate.TimeOfDay;
							await TriglyceridesVM.InsertNewEntry(true, utcDate);
						}
						break;
					
					default:
						break;

					}
				}
					
				if (result.Samples.Count > 0) {
					// Store the synchronization date, from the most recent entry retrieved from the web service.
					// XXX: .AddMinutes() changes the Kind to unspecified.
					var ts = latest.AddMinutes(1.0);
					SessionData.BiometricDataTS = DateTime.SpecifyKind(ts, DateTimeKind.Utc);

					// The Height and/or Weight might have been updated so we need to update the IMC.
					await IMCVM.ReloadIMCTable();
				}

			} catch (Exception ex) {
				// No point in letting the user know about this... we can always try again later
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			// Sync Complete
			OnLoadComplete();
		}
    }
}
