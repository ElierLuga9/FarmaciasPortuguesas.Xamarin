using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	public class PharmacyScheduleFor24h
	{
		[JsonProperty("value")]
		public bool IsOpen24H { get; set; }
	}
}
