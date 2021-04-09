using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ANFAPP.Logic;
using ANFAPP.Logic.Models.In;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services.Abstract;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using System.Globalization;

namespace ANFAPP.Logic.Network.Services
{
    public class SchedulerWS : I9WS
    {
        public static async Task<NewMedToUserOut> NewMedToUser(string CNP, string NomeMed, string Dosagem, string Quantidade, bool AvisoMedicamento = false, string FormaFarmaceutica = null, string Nota = null, string ActivePrinciple = null)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "NewMedToUser";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new NewMedToUserIn()
                {
					UserID = SessionData.PharmacyUser.Username,
                    CNP = CNP,
                    NomeMed = NomeMed,
                    AvisoMedicamento = AvisoMedicamento.ToString(),
                    FormaFarmaceutica = FormaFarmaceutica,
                    Nota = Nota,
                    Dosagem = Dosagem,
                    Embalagem = Quantidade,
					PrincipioAtivo = ActivePrinciple
                });


            // Call web service async
            var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<NewMedToUserOut> (response.Message);

            return responseOut;
        }

        // XXX: change the input to NewPlanToMedIn?
        public static async Task<NewPlanToMedOut> NewPlanToMed(
			string username,
            int MedId, 
            string NomeUser, 
            DateTime DataInicio, 
            int IntervaloQtd, 
            int IntervaloEscala, 
            int DuracaoQtd,
            int DuracaoEscala,
            double Qtd,
            string Nota = null)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "NewPlanToMed";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new NewPlanToMedIn()
                {
					UserID = username,
                    MedID = MedId.ToString(),
                    NomeUser = NomeUser,
                    DataInicio = DataInicio.ToString("o"),
                    IntervaloQtd = IntervaloQtd,
                    IntervaloEscala = IntervaloEscala,
                    DuracaoQtd = DuracaoQtd,
                    DuracaoEscala = DuracaoEscala,
                    Qtd = Qtd,
                    Nota = Nota
				}, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });

            // Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<NewPlanToMedOut> (response.Message);

            return responseOut;
        }

		public static async Task<NewTomaToPlanOut> NewTomaToPlan(
			string username,
			int dosageId,
			DateTime date,
			double quantity)
		{
			string request = JsonConvert.SerializeObject(new NewTomaToPlanIn()
			{
				UserID = username,
				PlanoTomasID = dosageId,
				Qtd = quantity,
				DataHora = date.ToUniversalTime().ToString("o")
			}, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });

			var response = await Client.PostJson(Settings.WS_SCHEDULES_NEW_TOMAS_TO_USER, request, SessionData.UserAuthentication);
			var responseOut = JsonConvert.DeserializeObject<NewTomaToPlanOut>(response.Message);

			return responseOut;
		}

        public static async Task<WSTomasOut> DelToma(string UserID, int TomaID)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "DelToma";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new DelTomaIn()
                {
					UserID = UserID,
                    TomaID = TomaID,
                });


            // Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<WSTomasOut> (response.Message);

            return responseOut;
        }

        public static async Task<WSTomasOut> DelPlanoTomas(string UserID, int PlanoTomasID)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "DelPlanoTomas";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new DelPlanoTomasIn()
                {
					UserID = UserID,
                    PlanoTomasID = PlanoTomasID,
                });


            // Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<WSTomasOut> (response.Message);

            return responseOut;
        }

        public static async Task<WSTomasOut> DelMed(string UserID, int MediID)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "DelMed";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new DelMedIn()
                {
					UserID = UserID,
                    MediID = MediID,
                });

            // Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<WSTomasOut> (response.Message);

            return responseOut;
        }

        #region Options

        public static async Task<WSTomasOut> PNConfig(string UserID, string Device, bool SendPN)
        {
            string requestUrl = Settings.TOMAS_ENDPOINT + "PNConfig";

            // Build request message
            string requestBody = JsonConvert.SerializeObject(new PNConfigIn()
                {
					UserID = UserID,
                    Device = Device,
                    SendPN = SendPN.ToString(),
                });

            // Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
            var responseOut = JsonConvert.DeserializeObject<WSTomasOut> (response.Message);

            return responseOut;
        }

		public static async Task<WSTomasOut> DeactivateToken(string UserID, string Device)
		{
			string requestUrl = Settings.TOMAS_ENDPOINT + "DelToken";

			// Build request message
			string requestBody = JsonConvert.SerializeObject(new DelTokenIn()
			{
				UserID = UserID,
				Device = Device,
			});

			// Call web service async
			var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
			var responseOut = JsonConvert.DeserializeObject<WSTomasOut>(response.Message);

			return responseOut;
		}

		public static async Task<WSTomasOut> SendReport(string UserID, int ReportType, bool onlyActive)
		{
			string requestUrl = Settings.TOMAS_ENDPOINT + "SendReport2";

			// Build request message
			string requestBody = JsonConvert.SerializeObject(new SendReportIn()
				{
					UserID = UserID,
					ReportType = ReportType,
					Active = onlyActive

				});

			// Call web service async


			try
			{
				var response = await Client.PostJson(requestUrl, requestBody, SessionData.UserAuthentication);
				var responseOut = JsonConvert.DeserializeObject<WSTomasOut>(response.Message);
				return responseOut; 
			} 
			catch(Exception ex) 
			{
				WSTomasOut a = new WSTomasOut();
				if (onlyActive)
				{
					a.ErrorMessage = "Não existem Planos Tomas ativos para enviar";
				}
				else
				{
					a.ErrorMessage = "Não existem Planos Tomas para enviar";
				}

				return a;
			}
		}

        #endregion

		#region Synchronization

		/// <summary>
		/// Retrieves the list of medicines for the referenced user.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<UpdatedMedicineOut> GetMedicines(string username, string cardNumber, string deviceToken)
		{
			string request = JsonConvert.SerializeObject(new GetUpdatedMedicineIn()
			{
				UserID = username,
				Card = cardNumber,
				Device = deviceToken ?? ""
			});
			
			var response = await Client.PostJson(Settings.WS_SCHEDULES_GET_MEDICINE, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<UpdatedMedicineOut>(response.Message);
		}

		/// <summary>
		/// Retrieves the list of dosing schedules for the referenced user.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<UpdatedDosingSchedulesOut> GetDosingSchedules(string username)
		{
			string request = JsonConvert.SerializeObject(new GetUpdatedSchedulesIn()
			{
				UserID = username
			});

			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
			};

			var response = await Client.PostJson(Settings.WS_SCHEDULES_GET_DOSING_SCHEDULES, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<UpdatedDosingSchedulesOut>(response.Message, settings);
		}

		/// <summary>
		/// Retrieves the list of dosages for the referenced user.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<UpdatedDosagesOut> GetDosages(string username)
		{
			string request = JsonConvert.SerializeObject(new GetUpdatedSchedulesIn()
			{
				UserID = username
			});
						
			var response = await Client.PostJson(Settings.WS_SCHEDULES_GET_DOSAGES, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<UpdatedDosagesOut>(response.Message);
		}

		/// <summary>
		/// Update a medicine.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateMedicine(string username, Medicine medicine)
		{
			return await UpdateMedicines(username, new List<Medicine>() { medicine });
		}

		/// <summary>
		/// Updates a list of medicines.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateMedicines(string username, List<Medicine> medicines)
		{
			var meds = new List<UpdateMedsIn.MedIn>();
			if (medicines != null)
			{
				// Parse medicines
				foreach (var med in medicines)
				{
					// XXX: the update date should be defined by the web service.
					string lastUpdated = null;
					if (null != med.LastUpdated) {
						lastUpdated = med.LastUpdated.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:sszzz");
					}

					meds.Add(new UpdateMedsIn.MedIn() 
					{
						MedID = med.Id,
						Nota = med.Notes,
						FormaFarmaceutica = med.PharmaceuticalUnit,
						CNP = med.CNP,
						AvisoMedicamento = med.WarningFlag,
						DataHoraUpdate = lastUpdated,
					});
				}
			}
			
			// Build request object
			string request = JsonConvert.SerializeObject(new UpdateMedsIn()
			{
				UserID = username,
				Medicamentos = meds
			});

			var response = await Client.PostJson(Settings.WS_SCHEDULES_UPDATE_MED, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<WSTomasOut>(response.Message);
		}

		/// <summary>
		/// Update a schedule.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateDosingSchedule(string username, DosingSchedule schedule)
		{
			return await UpdateDosingSchedules(username, new List<DosingSchedule>() { schedule });
		}

		/// <summary>
		/// Updates a list of schedules.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateDosingSchedules(string username, List<DosingSchedule> schedules)
		{
			var schedulesIn = new List<UpdateDosingSchedulesIn.DosingScheduleIn>();
			if (schedulesIn != null)
			{
				// Parse schedules
				foreach (var schedule in schedules)
				{
					schedulesIn.Add(new UpdateDosingSchedulesIn.DosingScheduleIn()
					{
						PlanoTomasID = schedule.Id,
						NomeUser = schedule.Description,
						Nota = schedule.Notes
					});
				}
			}

			// Build request object
			string request = JsonConvert.SerializeObject(new UpdateDosingSchedulesIn()
			{
				UserID = username,
				Planos = schedulesIn
			});

			var response = await Client.PostJson(Settings.WS_SCHEDULES_UPDATE_SCHEDULES, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<WSTomasOut>(response.Message);
		}

		/// <summary>
		/// Update a dosage.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateDosage(string username, Dosage dosage)
		{
			return await UpdateDosages(username, new List<Dosage>() { dosage });
		}

		/// <summary>
		/// Updates a list of dosages.
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> UpdateDosages(string username, List<Dosage> dosages)
		{
			var dosagesIn = new List<UpdateDosageIn.DosageIn>();
			if (dosages != null)
			{
				// Parse dosages
				foreach (var dosage in dosages)
				{
					dosagesIn.Add(new UpdateDosageIn.DosageIn()
					{
						TomaID = dosage.Id,
						DataHora = dosage.Date.ToUniversalTime().ToString("o"),
						Qtd = dosage.Quantity,
						FlagTomado = dosage.Done
					});
				}
			}

			// Build request object
			string request = JsonConvert.SerializeObject(new UpdateDosageIn()
			{
				UserID = username,
				Tomas = dosagesIn
			}, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });

			var response = await Client.PostJson(Settings.WS_SCHEDULES_UPDATE_DOSAGE, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<WSTomasOut>(response.Message);
		}

		/// <summary>
		/// Mark a dosage as done
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<WSTomasOut> MarkDosageAsDone(string username, string dosageId)
		{
			var id = 0;
			int.TryParse(dosageId, out id);

			var dosagesIn = new List<UpdateDosageIn.DosageIn>();
			dosagesIn.Add(new UpdateDosageIn.DosageIn() {
				TomaID = id,
				FlagTomado = true
			});

			// Build request object
			string request = JsonConvert.SerializeObject(new UpdateDosageIn()
			{
				UserID = username,
				Tomas = dosagesIn
			}, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture, NullValueHandling = NullValueHandling.Ignore });

			var response = await Client.PostJson(Settings.WS_SCHEDULES_UPDATE_DOSAGE, request, SessionData.UserAuthentication);
			return JsonConvert.DeserializeObject<WSTomasOut>(response.Message);
		}

		#endregion

    }
}
