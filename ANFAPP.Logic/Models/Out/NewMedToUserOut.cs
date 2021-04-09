﻿
using Newtonsoft.Json;
namespace ANFAPP.Logic.Models.Out
{
    public class NewMedToUserOut : WSTomasOut
    {
        public int MedID { get; set; }

		[JsonProperty("IsPilula")]
		public bool AutoGeneratedSchedule { get; set; }
    }
}