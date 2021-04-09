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
    public class GlicemiaEvaluator : BiometricEvaluator<Glicemia>
    {

        #region Constructors

        public GlicemiaEvaluator(Glicemia model, User user) : base(model, user) { }

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

            if (DataModel.Unfed)
            {
                if (DataModel.Value < 70)
                {
                    return ColorResources.ANFOrange;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 110)
                {
                    return ColorResources.ANFGreen;
                }
                else if (DataModel.Value >= 110 && DataModel.Value < 126)
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
                if (DataModel.Value < 70)
                {
                    return ColorResources.ANFOrange;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 140)
                {
                    return ColorResources.ANFGreen;
                }
                else if (DataModel.Value >= 140 && DataModel.Value < 200)
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
        /// Returns the message title for the referenced IMC value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (DataModel.Unfed)
            {
                if (DataModel.Value < 70)
                {
                    return AppResources.BiometricWarningAtentionTitle;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 110)
                {
                    return AppResources.BiometricWarningCongratulationsTitle;
                }
                else if (DataModel.Value >= 110 && DataModel.Value < 126)
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
                if (DataModel.Value < 70)
                {
                    return AppResources.BiometricWarningAtentionTitle;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 140)
                {
                    return AppResources.BiometricWarningCongratulationsTitle;
                }
                else if (DataModel.Value >= 140 && DataModel.Value < 200)
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
        /// Returns the message for the referenced IMC value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (DataModel.Unfed)
            {
                if (DataModel.Value < 70)
                {
                    return AppResources.GlicemiaWarningLowMessage;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 110)
                {
                    return AppResources.GlicemiaWarningOKMessage;
                }
                else if (DataModel.Value >= 110 && DataModel.Value < 126)
                {
                    return AppResources.GlicemiaWarningHighMessage;
                }
                else
                {
                    return AppResources.GlicemiaWarningVeryHighMessage;
                }
            }
            else
            {
                if (DataModel.Value < 70)
                {
                    return AppResources.GlicemiaWarningLowMessage;
                }
                else if (DataModel.Value >= 70 && DataModel.Value < 140)
                {
                    return AppResources.GlicemiaWarningOKMessage;
                }
                else if (DataModel.Value >= 140 && DataModel.Value < 200)
                {
                    return AppResources.GlicemiaWarningHighMessage;
                }
                else
                {
                    return AppResources.GlicemiaWarningVeryHighMessage;
                }
            }
        }

        #endregion

    }
}
