using System;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Network.Services;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.DAOs;
using System.Diagnostics;
using System.Collections.Generic;
using ANFAPP.Logic.Models.Objects;
using System.Linq;
using System.Net.Http;
using ANFAPP.Logic.Exceptions;
using System.Globalization;
using Xamarin.Forms;

namespace ANFAPP.Logic.Helper
{
	public static class PharmacyHelper
	{


		/// <summary>
		/// Given a filter will return all pharmacies that match that filter
		/// </summary>
		/// <returns>The pharmacies, inside value property</returns>
		/// <param name="filter">Filter.</param>
		public static async Task<List<Pharmacy>> GetPharmacies (string filter)
		{
			var allPharmacies = new List<Pharmacy>();

            DateTime lowerBound = DateTime.UtcNow.AddDays (-1);
            var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);

            var pharmaciesOut = await PharmacyWS.GetPharmacies (string.Format (Settings.WS_PHARMACIES_BASE_FILTER, filter, isoTime));
			var pharmacies = await PharmaciesOutToDAO (pharmaciesOut);
	
			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

			allPharmacies.AddRange(pharmacies);

			// This API is paged.
			while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

				try {
					pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
					if (null != pharmaciesOut) {
						pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

						allPharmacies.AddRange(pharmacies);
					}
					// Stop subsequent requests on network errors.
				} catch (HttpRequestException) {
					break;
				} catch (NetworkingException) { 
					break;
				} catch (InvalidRequestException) { 
					break;
				} catch (TimeoutException) { 
					break;
				}
			}

			var sorted = await SortPharmacies(allPharmacies);
			return sorted;
		}

		/// <summary>
		/// Gets the pharmacies in an area defined by a geodetic bounding box.
        /// 
        /// This API is paged.
        /// 
		/// </summary>
		/// <returns>A list of pharmacies contained in the provided coordinates.</returns>
        /// <param name="tl">The top left corner (NW).</param>
        /// <param name="br">The bottom right cornet (SE).</param>
		public static async Task<List<Pharmacy>> GetPharmaciesInBBOX (Location tl, Location br)
		{
            var allPharmacies = new List<Pharmacy>();

			var pharmaciesOut = await PharmacyWS.GetPharmaciesInBBOX(tl, br);
            var pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);
            
			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

			allPharmacies.AddRange(pharmacies);

            // This API is paged.
            while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

                try {
                    pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
                    if (null != pharmaciesOut) {
                        pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

                        allPharmacies.AddRange(pharmacies);
                    }
                // Stop subsequent requests on network errors.
                } catch (HttpRequestException) {
                    break;
                } catch (NetworkingException) { 
                    break;
                } catch (InvalidRequestException) { 
                    break;
                } catch (TimeoutException) { 
                    break;
                }
            }

            var sorted = await SortPharmacies(allPharmacies);
            return sorted;
		}

		/// <summary>
		/// Gets the pharmacies by district.
        /// 
        /// This API is paged.
        /// 
		/// </summary>
		/// <returns>The pharmacies by district.</returns>
		/// <param name="id">district id</param>
		public static async Task<List<Pharmacy>> GetPharmaciesByDistrict (string id)
		{
            var allPharmacies = new List<Pharmacy> ();
			string auxFilter = string.Format (Settings.WS_PHARMACY_BY_DISTRICT_FILTER, id);

            DateTime lowerBound = DateTime.UtcNow.AddDays(-1);
			var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);

            var pharmaciesOut = await PharmacyWS.GetPharmacies (string.Format (Settings.WS_PHARMACIES_BASE_FILTER, auxFilter, isoTime));
            var pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

            allPharmacies.AddRange(pharmacies);

            // This API is paged.
            while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

                try {
                    pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
                    if (null != pharmaciesOut) {
                        pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

                        allPharmacies.AddRange(pharmacies);
                    }
                    // Stop subsequent requests on network errors.
                } catch (HttpRequestException) {
                    break;
                } catch (NetworkingException) { 
                    break;
                } catch (InvalidRequestException) { 
                    break;
                } catch (TimeoutException) { 
                    break;
                }
            }


            var sorted = await SortPharmacies(allPharmacies);
            return sorted;
		}

        /// <summary>
        /// Gets the pharmacies by "concelho".
        /// 
        /// This API is paged.
        /// 
        /// </summary>
        /// <returns>The pharmacies by county.</returns>
        /// <param name="idDistrict">Identifier district.</param>
        /// <param name="idCounty">Identifier county.</param>
		public static async Task<List<Pharmacy>> GetPharmaciesByCounty (string idDistrict, string idCounty)
		{
            var allPharmacies = new List<Pharmacy> ();
            string auxFilter = string.Format (Settings.WS_PHARMACY_BY_COUNTY_FILTER, idDistrict, idCounty);

            DateTime lowerBound = DateTime.UtcNow.AddDays (-1);
			var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);

            var pharmaciesOut = await PharmacyWS.GetPharmacies (string.Format (Settings.WS_PHARMACIES_BASE_FILTER, auxFilter, isoTime));
            var pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

            allPharmacies.AddRange(pharmacies);

            // This API is paged.
            while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

                try {
                    pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
                    if (null != pharmaciesOut) {
                        pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

                        allPharmacies.AddRange(pharmacies);
                    }
                // Stop subsequent requests on network errors.
                } catch (HttpRequestException) {
                    break;
                } catch (NetworkingException) { 
                    break;
                } catch (InvalidRequestException) { 
                    break;
                } catch (TimeoutException) { 
                    break;
                }
            }

            var sorted = await SortPharmacies(allPharmacies);
            return sorted;
		}

		/// <summary>
		/// Gets the pharmacies by "freguesia".
        /// 
        /// This API is paged.
        /// 
		/// </summary>
		/// <returns>The pharmacies by parish.</returns>
        /// <param name = "idDistrict"></param>
        /// <param name = "idCounty"></param>
        /// <param name = "idParish"></param>
		public static async Task<List<Pharmacy>> GetPharmaciesByParish (string idDistrict, string idCounty, string idParish)
		{
            var allPharmacies = new List<Pharmacy> ();
			if (idParish.Length == 1) {
				idParish = "0" + idParish;
			}
			string auxFilter = string.Format (Settings.WS_PHARMACY_BY_PARISH_FILTER, idDistrict, idCounty, idParish);

            DateTime lowerBound = DateTime.UtcNow.AddDays (-1);
			var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);

            var pharmaciesOut = await PharmacyWS.GetPharmacies (string.Format (Settings.WS_PHARMACIES_BASE_FILTER, auxFilter, isoTime));
            var pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

            allPharmacies.AddRange(pharmacies);

            // This API is paged.
            while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

                try {
                    pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
                    if (null != pharmaciesOut) {
                        pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

                        allPharmacies.AddRange(pharmacies);
                    }
                    // Stop subsequent requests on network errors.
                } catch (HttpRequestException) {
                    break;
                } catch (NetworkingException) { 
                    break;
                } catch (InvalidRequestException) { 
                    break;
                } catch (TimeoutException) { 
                    break;
                }
            }

            var sorted = await SortPharmacies(allPharmacies);
            return sorted;
		}

		public static async Task<List<Pharmacy>> GetPharmaciesByService (int serviceId)
		{
			DateTime lowerBound = DateTime.UtcNow.AddDays (-1);
			var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);


			//var allPharmacies = new List<Pharmacy> ();
			string auxFilter = string.Format (Settings.WS_PHARMACY_SERVICES_FILTER, serviceId, isoTime);

			//var pharmaciesOut = await PharmacyWS.GetPharmacies (string.Format (Settings.WS_PHARMACIES_BASE_FILTER, auxFilter, isoTime));
			var pharmaciesOut = await PharmacyWS.GetPharmacies (auxFilter);
			var pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

			// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
			// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
			// = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

			SessionData.allPharmacies.AddRange(pharmacies);
			SessionData.NextPharmacies = pharmaciesOut.NextLink;
			/*// This API is paged.
			while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink)) {

				try {
					pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
					if (null != pharmaciesOut) {
						pharmacies = await PharmaciesOutToDAO (pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

						allPharmacies.AddRange(pharmacies);
					}
					// Stop subsequent requests on network errors.
				} catch (HttpRequestException) {
					break;
				} catch (NetworkingException) { 
					break;
				} catch (InvalidRequestException) { 
					break;
				} catch (TimeoutException) { 
					break;
				}
			}*/

			var sorted = await SortPharmacies(SessionData.allPharmacies);
			return sorted;
		}

		public static async Task<List<Pharmacy>> GetPharmMap(int serviceId, ICollection<Pharmacy> a)
		{
			DateTime lowerBound = DateTime.UtcNow.AddDays(-1);
			var isoTime = lowerBound.ToString("yyyy-MM-ddT00:00:00Z", CultureInfo.InvariantCulture);


			//var allPharmacies = new List<Pharmacy> ();
			string auxFilter = string.Format(Settings.WS_PHARMACY_SERVICES_FILTER, serviceId, isoTime);
			//Application.Current.MainPage.
			var pharmaciesOut = await PharmacyWS.GetPharmacies(auxFilter);
			var pharmacies = await PharmaciesOutToDAO(pharmaciesOut, false);
			var allPharmacies = new List<Pharmacy>();


			while (null != pharmaciesOut && !string.IsNullOrEmpty(pharmaciesOut.NextLink))
			{

				try
				{
					pharmaciesOut = await PharmacyWS.GetPharmaciesPage(pharmaciesOut.NextLink);
					if (null != pharmaciesOut)
					{
						pharmacies = await PharmaciesOutToDAO(pharmaciesOut, false);

						// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
						// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
						//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));

						allPharmacies.AddRange(pharmacies);
						Device.BeginInvokeOnMainThread(() =>
						{
							a = allPharmacies;
						});

					}
					// Stop subsequent requests on network errors.
				}
				catch (HttpRequestException)
				{
					break;
				}
				catch (NetworkingException)
				{
					break;
				}
				catch (InvalidRequestException)
				{
					break;
				}
				catch (TimeoutException)
				{
					break;
				}

			}
			var sorted = await SortPharmacies(allPharmacies);
			return sorted;
		}
		/// <summary>
		/// Sets the load next.
		/// </summary>
		/// <value>The load nexy.</value>
		public static async void LoadNext()
		{


			try
			{



				var pharmacies = await PharmacyWS.GetPharmaciesPage(SessionData.NextPharmacies);
				if (!string.IsNullOrEmpty(pharmacies.NextLink))
				{
					//await Task.Delay(Settings.DEFAULT_LOADING_DELAY);


					//	var pharmaciess = await PharmaciesOutToDAO(pharmacies, false);

					//await Task.Delay(Settings.DEFAULT_LOADING_DELAY);
					List<Pharmacy> pharmachyOutToPharm = new List<Pharmacy>();
					List<ShiftPharmacy> pharmacyShifts = new List<ShiftPharmacy>();
					foreach (var p in pharmacies.Value)
					{
						DateTime updated = new DateTime(2000, 01, 01);
						try
						{
							updated = DateTime.Parse(p.UpdatedOn, CultureInfo.InvariantCulture);
						}
						catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine(ex.Message);
						}
						Pharmacy pharmacy;
						pharmacy = new Pharmacy()
						{
							Id = long.Parse(p.Identifier),
							Code = p.Code,
							Identifier = p.Identifier,
							Name = p.Name,
							District = p.District.GetValueOrDefault(),
							County = p.County.GetValueOrDefault(),
							Parish = p.Parish,
							PostalCode = p.PostalCode,
							Locale = p.Locale,
							Address = p.Address,
							Phone = p.Phone,
							PFPSubscriber = p.PFPSubscriber.GetValueOrDefault(),
							Latitude = p.GeoCoordinates.Latitude,
							Longitude = p.GeoCoordinates.Longitude,
							ANFStatus = p.ANFStatus,
							IsActive = p.IsActive.GetValueOrDefault(),
							Type = p.Type.GetValueOrDefault(),
							UpdatedOn = updated,
							LastVisitedOn = new DateTime(2000, 01, 01),
							WorkSchedules = p.WorkSchedules,
							NonWorkingPeriod = p.NonWorkingPeriods

						};
						SetPharmStatusTxt(pharmacy);
						pharmacy.DistanceTxt = "Distancia: " + pharmacy.Distance;
						Shift[] shiftsOut = p.Shifts;

						foreach (Shift s in p.Shifts)
						{
							ShiftPharmacy sp = new ShiftPharmacy();

							sp.Id = s.Id.GetValueOrDefault();
							sp.IDPharmacy = pharmacy.Code;
							// XXX: Humm.. WTF?
							var str = s.Date.Substring(0, 10);
							sp.ShiftDate = str;

							pharmacyShifts.Add(sp);
						}


						pharmachyOutToPharm.Add(pharmacy);

					}
					// Issue 0020223: Alteração dos critérios de pesquisa de farmácias
					// Modificar o critério de pesquisa de modo a retornar apenas farmácias que fazem parte da rede Farmácias Portuguesas, exceto se estas estiverem de serviço.
					//pharmacies = pharmacies.FindAll(o => o.PFPSubscriber || PharmacyUtils.GetIsInService(o.Code));
					var sorted = await SortPharmacies(pharmachyOutToPharm);
					//	SessionData.allPharmacies = new List<Pharmacy>();

					SessionData.allPharmacies.AddRange(sorted);
					SessionData.NextPharmacies = pharmacies.NextLink;
					foreach (var p in SessionData.allPharmacies)
					{
						if (PharmacyUtils.GetIsInService(p.Code))
						{
							p.OpenStatusTxt = AppResources.InService;
							p.OpenStatusTextColor = Color.Aqua;

							SessionData.ServiceInServicePharm.Add(p);

						}
						if (((p.LongSchedule || p.OpenFull) && p.IsOpen))
						{

							SessionData.longSchedulesInServicePharm.Add(p);

						}
					}

				}

				else
				{
					SessionData.NextPharmacies = "";
				}
				// Stop subsequent requests on network errors.
			}
			catch (HttpRequestException)
			{

			}
			catch (NetworkingException)
			{

			}
			catch (InvalidRequestException)
			{

			}
			catch (TimeoutException)
			{

			}
			catch (Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

		}

		/// <summary>
        /// Pharmacieses the out to DA.
        /// </summary>
        /// <returns>The out to DA.</returns>
        /// <param name="pOut">P out.</param>
        /// <param name="sort">If set to <c>true</c> sort.</param>
        public static async Task<List<Pharmacy>> PharmaciesOutToDAO (PharmaciesOut pOut, bool sort = true)
        {
            List<ShiftPharmacy> pharmacyShifts = new List<ShiftPharmacy>();
            List<Pharmacy> pharmaciesDB = new List<Pharmacy> ();
            ShiftPharmacyDAO spDAO = new ShiftPharmacyDAO ();
            PharmacyDAO pDAO = new PharmacyDAO ();
           List<PharmacyOut> values = pOut.Value;

            foreach (PharmacyOut p in values) {
                long pid = long.Parse(p.Identifier);
                Pharmacy pharmacy = null;
                try {
                    pharmacy = await pDAO.GetById (pid);
                } catch (Exception e) {
					System.Diagnostics.Debug.WriteLine (e.Message);

					// The pharmacy was not found. Or...
					//
					// The iOS SQLite.Net library was, by default, storing DateTime's as strings. The 
					// date binding uses a fixed string format. However, to read a column, SQLite.Net 
					// uses DateTime.Parse with the thread default culture. If the user changes the 
					// idiom, the date parser will fail.
                }
               
				DateTime updated = new DateTime (2000, 01, 01);
				try {
					updated = DateTime.Parse(p.UpdatedOn, CultureInfo.InvariantCulture);
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine (ex.Message);
				}

                if (pharmacy == null) {
                    pharmacy = new Pharmacy () {
                        Id = pid,
                        Code = p.Code,
                        Identifier = p.Identifier,
                        Name = p.Name,
						District = p.District.GetValueOrDefault(),
						County = p.County.GetValueOrDefault(),
                        Parish = p.Parish,
                        PostalCode = p.PostalCode,
                        Locale = p.Locale,
                        Address = p.Address,
                        Phone = p.Phone,
						PFPSubscriber = p.PFPSubscriber.GetValueOrDefault(),
                        Latitude = p.GeoCoordinates.Latitude,
                        Longitude = p.GeoCoordinates.Longitude,
                        ANFStatus = p.ANFStatus,
						IsActive = p.IsActive.GetValueOrDefault(),
						Type = p.Type.GetValueOrDefault(),
						UpdatedOn = updated,
                        LastVisitedOn = new DateTime(2000,01,01),
                        WorkSchedules = p.WorkSchedules,
						NonWorkingPeriod = p.NonWorkingPeriods
						                 
                    };
                } else {
                    pharmacy.Code = p.Code;
                    pharmacy.Identifier = p.Identifier;
                    pharmacy.Name = p.Name;
					pharmacy.District = p.District.GetValueOrDefault();
					pharmacy.County = p.County.GetValueOrDefault();
                    pharmacy.Parish = p.Parish;
                    pharmacy.PostalCode = p.PostalCode;
                    pharmacy.Locale = p.Locale;
                    pharmacy.Address = p.Address;
                    pharmacy.Phone = p.Phone;
					pharmacy.PFPSubscriber = p.PFPSubscriber.GetValueOrDefault();
                    pharmacy.Latitude = p.GeoCoordinates.Latitude;
                    pharmacy.Longitude = p.GeoCoordinates.Longitude;
                    pharmacy.ANFStatus = p.ANFStatus;
					pharmacy.IsActive = p.IsActive.GetValueOrDefault();
					pharmacy.Type = p.Type.GetValueOrDefault();
                    pharmacy.UpdatedOn = updated;
                    pharmacy.WorkSchedules = p.WorkSchedules;
					pharmacy.NonWorkingPeriod = p.NonWorkingPeriods;
                }

                SetPharmStatusTxt(pharmacy);

                Shift [] shiftsOut = p.Shifts;
               
                foreach (Shift s in p.Shifts) {
                  ShiftPharmacy sp = new ShiftPharmacy ();

					sp.Id = s.Id.GetValueOrDefault();
                    sp.IDPharmacy = pharmacy.Code;
                    // XXX: Humm.. WTF?
                    var str = s.Date.Substring (0, 10);
                    sp.ShiftDate = str;

                    pharmacyShifts.Add (sp);
                }
                pharmaciesDB.Add (pharmacy);
			}
		
				await pDAO.InsertOrUpdateAll(pharmaciesDB);
				await spDAO.InsertOrUpdateAll(pharmacyShifts);
		
           

            if (sort) {
                var sorted = await SortPharmacies(pharmaciesDB);
                return sorted;
            }

            return pharmaciesDB;
        }

        public static void SetPharmStatusTxt(Pharmacy pharm)
        {
            var inService = PharmacyUtils.GetIsInService(pharm.Code);


				if (inService)
	            {
	                pharm.IsOpen = true;
				// pharm.OpenStatusTxt = AppResources.InService;
					pharm.OpenStatusTextColor = Resources.ColorResources.LocatorGreenText;
	            }
	            else
	            {
					var status = PharmacyUtils.IsPharmOpen(pharm.WorkSchedules, pharm.NonWorkingPeriod);
					bool open = status.IsOpen;
	                pharm.OpenFull = status.Open24Hours;
					pharm.LongSchedule = status.LongSchedule;
						  
	                if (pharm.OpenFull)
					{
						pharm.IsOpen = true;
						pharm.OpenStatusTxt = AppResources.Open24Hours;
					}
					else if (open)
					{
						pharm.IsOpen = true;

						if (!string.IsNullOrWhiteSpace(status.CloseTime))
						{
							pharm.OpenStatusTxt = AppResources.OpenTill + " " + status.CloseTime;
						}
						else
						{
							pharm.OpenStatusTxt = AppResources.Open;
						}
					}
					else
					{
						pharm.IsOpen = false;
						pharm.OpenStatusTxt = AppResources.ClosedFemale;
					}
	           }
        }

        /// <summary>
        /// Sorts the pharmacies by the distance to the current user location.
        /// </summary>
        /// <returns>The pharmacies sorted by distance, if the current location is available.</returns>
        /// <param name="pharmacies">A list of Pharmacies</param>
        public static async Task<List<Pharmacy>> SortPharmacies (List<Pharmacy> pharmacies)
        {
            Location loc = SessionData.UserLocation;

            if (loc != null && !(loc.Latitude > -double.Epsilon && loc.Latitude < double.Epsilon)
                && !(loc.Longitude > -double.Epsilon && loc.Longitude < double.Epsilon)) {

                for (int i = 0; i < pharmacies.Count; i++) {
                    Pharmacy pTemp = pharmacies [i];
                    pTemp.Distance = Math.Round (DistanceTo (loc.Latitude, loc.Longitude, pTemp.Latitude, pTemp.Longitude), 2);
                    pTemp.DistanceTxt = "Distancia: " + pTemp.Distance + " km";
                }

                return pharmacies.OrderBy (a => a.Distance).ToList ();
            }

            return pharmacies;
        }
		public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
		{
			double rlat1 = Math.PI*lat1/180;
			double rlat2 = Math.PI*lat2/180;
			double theta = lon1 - lon2;
			double rtheta = Math.PI*theta/180;
			double dist = 
				Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * 
				Math.Cos(rlat2) * Math.Cos(rtheta);
			dist = Math.Acos(dist);
			dist = dist*180/Math.PI;
			dist = dist*60*1.1515;

			switch (unit)
			{
			case 'K': //Kilometers -> default
				return dist * 1.609344;
			case 'N': //Nautical Miles 
				return dist * 0.8684;
			case 'M': //Miles
				return dist;
			}

			return dist;
		}
	}
}
