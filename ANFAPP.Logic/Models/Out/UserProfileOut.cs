using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out
{
	/**
	 * Implements the model for @odata.context":"https://services-qa.anf.pt/REST/v3/PfpOData/$metadata#Pfp-Profile/$entity . 
	 */
	public class UserProfileOut
	{
		public string Id { get; set; }
        public UserOut User { get; set; }
		public ClientOut Client { get; set; }

		public class UserOut
		{
			public string UserName { get; set; }
			public string Email { get; set; }
			public string CreatedOn { get; set; }
			public string UpdatedOn { get; set; }
			public string DocumentNumber { get; set; }
			public string DocumentType { get; set; }
			public string GivenName { get; set; }
			public string FamilyName { get; set; }
			public string ImageId { get; set; }
		}
	}
}
