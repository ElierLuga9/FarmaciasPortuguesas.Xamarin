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
    public class TriglyceridesEvaluator : BiometricEvaluator<Triglycerides>
    {

        #region Constructors

        public TriglyceridesEvaluator(Triglycerides model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced Triglycerides value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            if (DataModel == null) return ColorResources.TextColorDark;

            if (DataModel.Value < 150)
            {
                return ColorResources.ANFGreen;
            }
            else
            {
                return ColorResources.ANFRed;
            }
        }

        /// <summary>
        /// Returns the message title for the referenced Triglycerides value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 150)
            {
                return AppResources.BiometricWarningCongratulationsTitle;
            }
            else
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
        }

        /// <summary>
        /// Returns the message for the referenced Triglycerides value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 150)
            {
                return AppResources.TriglyceridesWarningOKMessage;
            }
            else
            {
                return AppResources.TriglyceridesWarningHighMessage;
            }
        }

        #endregion

    }
}
