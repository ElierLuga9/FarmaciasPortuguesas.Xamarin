using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models
{
	public class FacebookObject
	{

		#region Properties

		public string Email { get; set; }
		public string UserId { get; set; }

		#endregion

		#region Constructors

		public FacebookObject(string email, string id)
		{
			Email = email;
			UserId = id;
		}

		#endregion

	}
}
