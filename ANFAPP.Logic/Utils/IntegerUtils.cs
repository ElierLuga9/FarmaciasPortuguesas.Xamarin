using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Utils
{
    public static class IntegerUtils
    {

        /// <summary>
        /// Returns the closest multiple of 10 to the referenced number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetCloserMultipleOf10(int value)
        {
            if (value % 10 == 0) return value;

            return value + (10 - (value % 10));
        }

    }
}
