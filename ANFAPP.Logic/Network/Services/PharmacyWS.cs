using System;
using System.Diagnostics;
using System.Globalization;
using ANFAPP.Logic.Exceptions;
using System.Collections.Generic;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Models.Out;
using ANFAPP.Logic.Models.Objects;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using ANFAPP.Logic.Network.Services.Abstract;

namespace ANFAPP.Logic.Network.Services
{
    public class PharmacyWS : AnfWS
    {
        private static async Task<PharmaciesOut> GetRequest(string request)
        {
            PharmaciesOut responseOut = null;

            var response = await Client.Get (request);
            try {
                responseOut = JsonConvert.DeserializeObject<PharmaciesOut> (response.Message);
            } catch (JsonSerializationException ex) {
                // Catch unexpected service changes..
				throw new ServiceErrorException (AppResources.GenericErrorMessage);
            } finally { }

            return responseOut;
        }

        /// <summary>
        /// Querys pharmacy API
        /// </summary>
        /// <returns>The pharmacies.</returns>
        /// <param name="uriParameters">URI parameters.</param>
        public static async Task<PharmaciesOut> GetPharmacies (string uriParameters)
        {
		    var request = Settings.WS_PHARMACIES + uriParameters; 
            var result = await GetRequest (request);
            return result;
        }

		[Obsolete("The pharmacy detail should be obtained from the Magento web services if the pharmacy has Ecommerce.")]
		public static async Task<PharmacyOut> GetPharmacyDetail(string code)
		{
			ANFAPP.Logic.Models.Out.PharmacyOut responseOut = null;
			var request = Settings.WS_PHARMACIES + string.Format("('{0}')", code) + Settings.WS_PHARMACIES_DETAIL_DATA; 

			var response = await Client.Get (request);
			try {
				responseOut = JsonConvert.DeserializeObject<PharmacyOut> (response.Message);
			} catch (JsonSerializationException) {
				// Catch unexpected service changes..
				throw new ServiceErrorException (AppResources.GenericErrorMessage);
			} 

			return responseOut;
		}

		public static async Task<PharmacyScheduleFor24h> GetPharmacyScheduleFor24h(string code) 
		{
			PharmacyScheduleFor24h responseOut = null;
			var request = Settings.WS_PHARMACIES + string.Format("('{0}')", code) + Settings.WS_PHARMACIES_IS_OPEN_24H;

			var response = await Client.Get(request);
			try {
				responseOut = JsonConvert.DeserializeObject<PharmacyScheduleFor24h> (response.Message);
			} catch (JsonSerializationException) {
				// Catch unexpected service changes..
				throw new ServiceErrorException(AppResources.GenericErrorMessage);
			} 

			return responseOut;
		}

        /// <summary>
        /// Query the Pharmacy API for all the pharmacies inside a given bounding box.
        /// </summary>
        /// <returns>The pharmacies inside the bounding box.</returns>
        /// <param name="tl">The top left coordinate.</param>
        /// <param name="br">The bottom right coordinate.</param>
        public static async Task<PharmaciesOut> GetPharmaciesInBBOX (Location tl, Location br)
        {
            DateTime lowerBound = DateTime.UtcNow.AddDays(-1);
            var isoTime = lowerBound.ToString ("yyyy-MM-ddT00:00:00Z");
            var filter = String.Format (CultureInfo.InvariantCulture, Settings.WS_PHARMACIES_GEO_FILTER, tl.Latitude, tl.Longitude, br.Latitude, br.Longitude, isoTime);
		
            var request = Settings.WS_PHARMACIES + "/" + filter; 

            var result = await GetRequest (request);
            return result;
        }

        /// <summary>
        /// Gets the pharmacies page.
        /// </summary>
        /// <returns>A list of pharmacies.</returns>
        /// <param name="url">An URL returned by a previous request to the Pharmacy API.</param>
        public static async Task<PharmaciesOut> GetPharmaciesPage (string url)
        {
            var result = await GetRequest (url);
            return result;
        }         
    }
}
