using System;

namespace ANFAPP.Logic.Utils
{
	public enum HealthDataType
	{
		Count,
		Distance,
	}

	public class HealthDataSample
	{
		/// <summary>
		/// The start date of the sample.
		/// </summary>
		/// <value>A date.</value>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// The end date of the sample.
		/// </summary>
		/// <value>A date.</value>
		public DateTime EndDate { get; set; }

		/// <summary>
		/// The type of date represented in the sample.
		/// 
		/// Step samples use the HealthDataType.Count type. Walking or running 
		/// data samples use the HealthDataType.Distance type.
		/// 
		/// </summary>
		/// <value>A HealthDataType value.</value>
		public HealthDataType Type { get; set; }

		/// <summary>
		/// The sample quantity converted to the floating point representation of the 
		/// sample default unit. 
		/// </summary>
		/// <value>The quantity.</value>
		public double Quantity { get; set; }

		/// <summary>
		/// The description of the Quantity unit.
		/// </summary>
		/// <value>The type of the quantity.</value>
		public string QuantityType { get; set; }

		/// <summary>
		/// The field is set for step samples.
		/// </summary>
		/// <value><c>true</c> if this instance is scalar; otherwise, <c>false</c>.</value>
		public bool IsScalar { get; set; }

		/// <summary>
		/// The field is set if the sample has associated metadata.
		/// 
		/// This should be checked before using WasEnteredByUser.
		/// 
		/// </summary>
		/// <value><c>true</c> if this instance has metadata; otherwise, <c>false</c>.</value>
		public bool HasMetadata { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ANFAPP.Logic.Utils.HealthDataSample"/> was entered by user.
		/// </summary>
		/// <value><c>true</c> if was entered by user; otherwise, <c>false</c>.</value>
		public bool WasEnteredByUser { get; set; }
	}
}

