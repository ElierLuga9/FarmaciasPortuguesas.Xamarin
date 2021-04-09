using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
	class FacebookAuthorizationIn
	{
		public string UserName { get; set; }
		public string Provider { get; set; }
		public string ProviderToken { get; set; }
		public string ApplicationId { get; set; }
	}
}
