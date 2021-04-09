using ANFAPP.Logic.Network.Invokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Network.Services.Abstract
{
	public abstract class AnfWS
	{
		
		#region 

		protected static RestInvoker Client { get { return AnfRestClient.Instance; } }
		#endregion

	}
}
