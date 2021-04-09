using System;

namespace ANFAPP.Logic.Models.In
{
	public class SendReportIn
	{
		public string UserID { get; set; }
		public int ReportType { get; set; }
		public bool Active { get; set; }
	}
}
