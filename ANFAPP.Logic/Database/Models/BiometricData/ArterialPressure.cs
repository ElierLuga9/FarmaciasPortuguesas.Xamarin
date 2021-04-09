using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Database.Models
{
    [Table("Arterial_Pressures")]
    public class ArterialPressure : BiometricBase
    {
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int BPM { get; set; }
    }
}
