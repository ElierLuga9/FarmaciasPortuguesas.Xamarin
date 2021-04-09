using System;
using System.Collections.Generic;

namespace ANFAPP.Logic.Utils
{
	public class HealthDataEventArgs : EventArgs
	{
		public HealthDataType Type { get; set; }

		public List<HealthDataSample> Samples { get; set; }
	}
}

