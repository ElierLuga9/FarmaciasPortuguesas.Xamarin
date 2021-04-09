using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ANFAPP.Logic;
using ANFAPP.Logic.Resources;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic.BusinessLogic.BiometricData
{
    public class HeightEvaluator : BiometricEvaluator<Height>
    {

        #region Constructors

        public HeightEvaluator(Height model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced Height value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            return ColorResources.TextColorDark;
        }

        /// <summary>
        /// Returns the message title for the referenced Height value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            return null;
        }

        /// <summary>
        /// Returns the message for the referenced Height value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            return null;
        }

        #endregion

    }
}
