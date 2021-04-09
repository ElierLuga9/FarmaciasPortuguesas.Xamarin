using System;

namespace ANFAPP.Logic.Utils
{
	public class PharmacyChangedEventArgs : EventArgs
	{
		public string OldPharmacyId { get; set; }

		public string NewPharmacyId { get; set; }
	}
}

