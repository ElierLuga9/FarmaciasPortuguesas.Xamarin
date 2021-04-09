using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Logic.BusinessLogic.BiometricData
{
    public class IMCEvaluator : BiometricEvaluator <IMC>
    {

        #region Constructors

        public IMCEvaluator(IMC model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced IMC value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            if (DataModel == null) return ColorResources.TextColorDark;

            if (DataModel.Value < 18.5)
            {
                // Orange
                return ColorResources.ANFOrange;
            }
            else if (DataModel.Value >= 18.5 && DataModel.Value < 25)
            {
                // Green
                return ColorResources.ANFGreen;
            }
            else if (DataModel.Value >= 25 && DataModel.Value < 30)
            {
                // Orange
                return ColorResources.ANFOrange;
            }
            else
            {
                // Red
                return ColorResources.ANFRed;
            }
        }

        /// <summary>
        /// Returns the message title for the referenced IMC value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 18.5)
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
            else if (DataModel.Value >= 18.5 && DataModel.Value < 25)
            {
                return AppResources.BiometricWarningCongratulationsTitle;
            }
            else if (DataModel.Value >= 25 && DataModel.Value < 30)
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
            else
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
        }

        /// <summary>
        /// Returns the message for the referenced IMC value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 18.5)
            {
                return AppResources.IMCWarningLowMessage;
            }
            else if (DataModel.Value >= 18.5 && DataModel.Value < 25)
            {
                return AppResources.IMCWarningOKMessage;
            }
            else if (DataModel.Value >= 25 && DataModel.Value < 30)
            {
                return AppResources.IMCWarningHighMessage;
            }
            else
            {
                return AppResources.IMCWarningVeryHighMessage;
            }
        }

        #endregion

    }
}
