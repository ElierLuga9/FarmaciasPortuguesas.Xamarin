using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.In
{
    public class ChangePasswordIn
    {

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

    }
}
