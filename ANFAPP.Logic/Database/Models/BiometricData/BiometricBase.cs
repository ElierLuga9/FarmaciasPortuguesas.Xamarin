using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    public abstract class BiometricBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
		public bool PharmacyFlag { get; set; }

        [Ignore]
        public int Order { get; set; }

    }
}
