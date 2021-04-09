using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.BusinessLogic.BiometricData
{
    public interface IBiometricEvaluator
    {

        /// <summary>
        /// Returns the warning color for the referenced value.
        /// </summary>
        /// <returns></returns>
        Color GetWarningColor();

        /// <summary>
        /// Returns the warning title for the referenced value.
        /// </summary>
        /// <returns></returns>
        string GetMessageTitle();

        /// <summary>
        /// Returns the warning message for the referenced value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetMessage();

    }
}
