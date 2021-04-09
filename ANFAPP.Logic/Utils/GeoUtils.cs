using System;
using ANFAPP.Logic.Models.Objects;

namespace ANFAPP.Logic.Utils
{
	public static class GeoUtils
	{
		const double EarthRadius = 6378137.0;

		/// <summary>
		/// Calculates a new coordinate at a given distance from the source coordinate.
		/// 
		/// This method is not accurate if using long displacements.
		/// </summary>
		/// <returns>A new coordinate.</returns>
		/// <param name="origin">The origin coordinate.</param>
		/// <param name="x">The offset, in meters, along the longitude axis.</param>
		/// <param name="y">The offset, in meters, along the latitude axys.</param>
		public static Location OffsetCoordinate(Location origin, double x, double y, double dX = 0.0, double dY = 0.0)
		{
			// Coordinate offsets in radians
			var dLat = y / EarthRadius;
			var dLon = x / (EarthRadius * Math.Cos (Math.PI * origin.Latitude / 180.0));

			// OffsetPosition, decimal degrees
			double offsetLat = origin.Latitude + dLat * 180.0 / Math.PI;
			double offsetLon = origin.Longitude + dLon * 180.0 / Math.PI;

			// It looks like ANF truncates/doesn't handle long values.
			offsetLat = Math.Round(offsetLat + dY, 4, MidpointRounding.AwayFromZero);
			offsetLon = Math.Round(offsetLon + dX, 4, MidpointRounding.AwayFromZero);

			Location loc = new Location(offsetLat, offsetLon);
			return loc;
		}

		/// <summary>
		/// Converts a zoom level into latitude and longitude degrees.
		/// </summary>
		/// <param name="zoomLevel"></param>
		/// <returns></returns>
		public static double ConvertZoomLevelToDegree(int zoomLevel)
		{
			return 360 / (Math.Pow(2, zoomLevel));
		}

	}
}

