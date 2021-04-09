using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Triglycerides")]
    public class Triglycerides : BiometricBase
    {
        public int Value { get; set; }
    }
}
