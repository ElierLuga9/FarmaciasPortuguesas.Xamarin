using System;
namespace ANFAPP.Logic.Models.In
{
	public class UpdateNotificationTokenIn
	{
		public string UserID { get; set; }
		public string NewToken { get; set; }
		public string OldToken { get; set; }
		public string DeviceType { get; set; }
		public string Card { get; set; }
	}
}
