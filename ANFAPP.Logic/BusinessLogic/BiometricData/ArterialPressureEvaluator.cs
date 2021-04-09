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
    public class ArterialPressureEvaluator : BiometricEvaluator<ArterialPressure>
    {

        #region Constructors

        public ArterialPressureEvaluator(ArterialPressure model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced Arterial Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            if (DataModel == null) return ColorResources.TextColorDark;

            if (DataModel.Systolic < 80 || DataModel.Diastolic < 40)
            {
                return ColorResources.ANFRed;
            }
            else if ((DataModel.Systolic >= 160) || (DataModel.Diastolic >= 100))
            {
                return ColorResources.ANFRed;
            }
            else if ((DataModel.Systolic >= 140 && DataModel.Systolic < 160) || (DataModel.Diastolic >= 90 && DataModel.Diastolic < 100))
            {
                return ColorResources.ANFOrange;
            }
            else if ((DataModel.Systolic >= 130 && DataModel.Systolic < 140) || (DataModel.Diastolic >= 85 && DataModel.Diastolic < 90))
            {
                return ColorResources.ANFDarkYellow;
            }
            else if ((DataModel.Systolic >= 120 && DataModel.Systolic < 130) || (DataModel.Diastolic >= 80 && DataModel.Diastolic < 85))
            {
                return ColorResources.ANFYellow;
            }
            else
            {
                return ColorResources.ANFGreen;
            }
        }

        /// <summary>
        /// Returns the message title for the referenced Arterial Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (DataModel.Systolic < 80 || DataModel.Diastolic < 40)
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
            else if ((DataModel.Systolic >= 160) || (DataModel.Diastolic >= 100))
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
            else if ((DataModel.Systolic >= 140 && DataModel.Systolic < 160) || (DataModel.Diastolic >= 90 && DataModel.Diastolic < 100))
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
            else if ((DataModel.Systolic >= 130 && DataModel.Systolic < 140) || (DataModel.Diastolic >= 85 && DataModel.Diastolic < 90))
            {
                return null;
            }
            else if ((DataModel.Systolic >= 120 && DataModel.Systolic < 130) || (DataModel.Diastolic >= 80 && DataModel.Diastolic < 85))
            {
                return null;
            }
            else
            {
                return AppResources.BiometricWarningCongratulationsTitle;
            }
        }

        /// <summary>
        /// Returns the message for the referenced Arterial Pressure value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (DataModel.Systolic < 80 || DataModel.Diastolic < 40)
            {
                return AppResources.ArterialPressureWarningLowMessage;
            }
            else if ((DataModel.Systolic >= 160) || (DataModel.Diastolic >= 100))
            {
                return AppResources.ArterialPressureWarningDangerMessage;
            }
            else if ((DataModel.Systolic >= 140 && DataModel.Systolic < 160) || (DataModel.Diastolic >= 90 && DataModel.Diastolic < 100))
            {
                return AppResources.ArterialPressureWarningVeryHighMessage;
            }
            else if ((DataModel.Systolic >= 130 && DataModel.Systolic < 140) || (DataModel.Diastolic >= 85 && DataModel.Diastolic < 90))
            {
                return AppResources.ArterialPressureWarningHighMessage;
            }
            else if ((DataModel.Systolic >= 120 && DataModel.Systolic < 130) || (DataModel.Diastolic >= 80 && DataModel.Diastolic < 85))
            {
                return AppResources.ArterialPressureWarningOKMessage;
            }
            else
            {
                return AppResources.ArterialPressureWarningGoodMessage;
            }
        }

        #endregion

    }
}
