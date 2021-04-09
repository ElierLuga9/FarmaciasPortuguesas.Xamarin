using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Heights")]
    public class Height : BiometricBase
    {
        public double Value { get; set; }
    }
}
