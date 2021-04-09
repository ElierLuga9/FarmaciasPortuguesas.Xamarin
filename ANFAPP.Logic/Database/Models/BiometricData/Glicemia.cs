using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Glicemias")]
    public class Glicemia : BiometricBase
    {
        public int Value { get; set; }
        public bool Unfed { get; set; }
    }
}
