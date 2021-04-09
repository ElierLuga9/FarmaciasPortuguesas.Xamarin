using System;

namespace ANFAPP.Logic.Models.Objects
{
	public class Location
	{
		public Location () { }

		public Location (double latitude, double longitude) : base()
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Location: Latitude={0}, Longitude={1}]", Latitude, Longitude);
		}
	}
}

