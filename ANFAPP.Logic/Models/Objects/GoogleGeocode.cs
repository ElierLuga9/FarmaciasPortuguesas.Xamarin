using System;
using System.Collections.Generic;

namespace ANFAPP.Logic.Models.Objects
{
	public class GoogleGeocode {
		public List<Result> results { get; set; }
	}

	public class AddressComponent {
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}

	public class Result {
		public List<AddressComponent> address_components { get; set; }

	}
}

