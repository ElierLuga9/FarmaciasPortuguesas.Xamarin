using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Abdominal_Perimeters")]
    public class AbdominalPerimeter : BiometricBase
    {
        public int Value { get; set; }
    }
}
