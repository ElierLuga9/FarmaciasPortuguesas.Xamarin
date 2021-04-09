using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	public class GetTokenIn
	{
		public string UserID { get; set; }
		public string Card { get; set; }
		public string Device { get; set; }
		public string DeviceType { get; set; }
	}
}
