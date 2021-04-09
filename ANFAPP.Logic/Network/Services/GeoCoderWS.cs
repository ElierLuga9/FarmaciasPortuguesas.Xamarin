using System;
using System.Globalization;
using System.Threading.Tasks;
using ANFAPP.Logic.Models.Objects;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using ANFAPP.Logic.Exceptions;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using ANFAPP.Logic.Network.Services.Abstract;

namespace ANFAPP.Logic.Network.Services
{
	public class GeoCoderWS : AnfWS
	{
		public static async Task<Tuple<string, string, string>> GetReverseGeocoding(Location userLocation) {

			string request = Settings.WS_GOOGLE_GEO;
			request = request.Replace ("{0}", userLocation.Latitude.ToString(CultureInfo.InvariantCulture));
			request = request.Replace ("{1}", userLocation.Longitude.ToString(CultureInfo.InvariantCulture));

			Debug.WriteLine (" -> Requesting: {0}", request);

			var response = await Client.Get (request);
			var responseOut = JsonConvert.DeserializeObject<GoogleGeocode>(response.Message);

			string district = null;
			string county = null;
			string parish = null;

			List<Result> results = responseOut.results;
			foreach (Result r in results) {
				List<AddressComponent> acList = r.address_components;
				foreach (AddressComponent ac in acList) {
					foreach (string strType in ac.types) {
						if (strType == "administrative_area_level_1") {
							
							district = ac.long_name;
						}
						if (strType == "administrative_area_level_2") {

							county = ac.long_name;
						}
						if (strType == "administrative_area_level_3") {

							parish = ac.long_name;
						}
					}

					if (district == "Lisbon") 
					{
						district = "Lisboa";
					}
					if (county == "Lisbon") 
					{
						county = "Lisboa";
					}
					if (district == "Oporto") 
					{
						district = "Porto";
					}
					if (county == "Oporto") 
					{
						county = "Porto";
					}

					if (district != null && county != null && parish != null)
						break;
				}
			}

			return new Tuple<string,string,string>(district, county, parish);
		}

	}
}

