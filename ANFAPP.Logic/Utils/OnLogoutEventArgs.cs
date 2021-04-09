using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{
	public class OnLogoutEventArgs : EventArgs
	{
		public string Message { get; set; }
	}
}
