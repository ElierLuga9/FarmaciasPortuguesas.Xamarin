using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Weights")]
    public class Weight : BiometricBase
    {
        public double Value { get; set; }
    }
}
