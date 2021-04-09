using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{
	public interface IFacebookSDK
	{

		#region Methods

		void InitService(object context);

		void SendCallbackResult(int request, int result, object data);

		void Login();

		void Logout();

		bool IsLoggedOnFacebook();

		void GetUserEmail();

		string GetUserToken();

		#endregion

	}
}
