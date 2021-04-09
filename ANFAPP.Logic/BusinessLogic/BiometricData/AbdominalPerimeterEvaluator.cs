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
    public class AbdominalPerimeterEvaluator: BiometricEvaluator<AbdominalPerimeter>
    {

        #region Constructors

        public AbdominalPerimeterEvaluator(AbdominalPerimeter model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced Abdominal Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            if (DataModel == null) return ColorResources.TextColorDark;

            if (User == null || User.IsMale)
            {
                if (DataModel.Value < 94)
                {
                    return ColorResources.ANFGreen;
                }
                else if (DataModel.Value >= 94 && DataModel.Value <= 102)
                {
                    return ColorResources.ANFOrange;
                } 
                else 
                {
                    return ColorResources.ANFRed;
                }
            }
            else
            {
                if (DataModel.Value < 80)
                {
                    return ColorResources.ANFGreen;
                }
                else if (DataModel.Value >= 80 && DataModel.Value <= 88)
                {
                    return ColorResources.ANFOrange;
                }
                else
                {
                    return ColorResources.ANFRed;
                }
            }
        }

        /// <summary>
        /// Returns the message title for the referenced Abdominal Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (User == null || User.IsMale)
            {
                if (DataModel.Value < 94)
                {
                    return AppResources.BiometricWarningCongratulationsTitle;
                }
                else if (DataModel.Value >= 94 && DataModel.Value <= 102)
                {
                    return null;
                }
                else
                {
                    return AppResources.BiometricWarningAtentionTitle;
                }
            }
            else
            {
                if (DataModel.Value < 80)
                {
                    return AppResources.BiometricWarningCongratulationsTitle;
                }
                else if (DataModel.Value >= 80 && DataModel.Value <= 88)
                {
                    return null;
                }
                else
                {
                    return AppResources.BiometricWarningAtentionTitle;
                }
            }
        }

        /// <summary>
        /// Returns the message for the referenced Abdominal Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (User == null || User.IsMale)
            {
                if (DataModel.Value < 94)
                {
                    return AppResources.AbdominalPerimeterWarningOKMessage;
                }
                else if (DataModel.Value >= 94 && DataModel.Value <= 102)
                {
                    return AppResources.AbdominalPerimeterWarningHighMessage;
                }
                else
                {
                    return AppResources.AbdominalPerimeterWarningVeryHighMessage;
                }
            }
            else
            {
                if (DataModel.Value < 80)
                {
                    return AppResources.AbdominalPerimeterWarningOKMessage;
                }
                else if (DataModel.Value >= 80 && DataModel.Value <= 88)
                {
                    return AppResources.AbdominalPerimeterWarningHighMessage;
                }
                else
                {
                    return AppResources.AbdominalPerimeterWarningVeryHighMessage;
                }
            }
        }

        #endregion

    }
}
