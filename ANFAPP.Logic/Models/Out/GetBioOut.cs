using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	/// <summary>
	/// BIOID.
	/// </summary>
	public enum BiometricType
	{
		Weight = 1,
		Height,
		BloodPressure,
		Cholesterol,
		Glicemia,
		AbdominalPerimeter,
		Triglycerides,
	}

	public class Sample
	{
		public int BIOID { get; set; }
		public double? PAR1 { get; set; }
		public double? PAR2 { get; set; }
		public double? PAR3 { get; set; }
		public bool? BPAR { get; set; }
		public string DATE { get; set; }
		public string RECDATE { get; set; }
	}

	public class GetBioOut : WSTomasOut
	{
		[JsonProperty("medicoes")]
		public List<Sample> Samples { get; set; }
	}
}

