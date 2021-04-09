using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
    public class CardAssociationIn
    {
        #region Properties

        public UserIn User { get; set; }
        public ClientIn Client { get; set; }

        #endregion

        #region Inner Classes

        public class UserIn 
        {
            public string UserName { get; set; }
            public string DocumentNumber { get; set; }
            public string DocumentType { get; set; }
        }

		public class ClientIn
		{
			public CardIn PharmacyCard { get; set; }
		}

        public class CardIn
        {
            public string Number { get; set; }
        }

        #endregion
    }
}
