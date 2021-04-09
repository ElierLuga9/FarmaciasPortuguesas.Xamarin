using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models
{
    public class PharmacyUser
    {

        #region Properties

        public string Name { get; set; }
        public DateTime Birthdate { get; set; }

        public string Username { get; set; }
        public string CardNumber { get; set; }

        public bool IsFamilyCard { get; set; }
        public bool IsFamilyHead { get; set; }
        public int FamilySize { get; set; }

        public int Points { get; set; }
        public int ExpiringPoints { get; set; }
        public decimal SavedAmount { get; set; }
        
        public DateTime LastUpdated { get; set; }

        public string PhotoLocation { get; set; }

		public string ContactPhone { get; set; }
		public string ClientNumber { get; set; }

        #endregion

    }
}
