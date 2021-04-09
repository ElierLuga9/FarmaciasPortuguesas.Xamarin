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
    public class CholesterolEvaluator : BiometricEvaluator<Cholesterol>
    {

        #region Constructors

        public CholesterolEvaluator(Cholesterol model, User user) : base(model, user) { }

        #endregion

        #region Evaluators

        /// <summary>
        /// Returns the warning color for the referenced Cholesterol value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Color GetWarningColor()
        {
            if (DataModel == null) return ColorResources.TextColorDark;

            if (DataModel.Value < 190)
            {
                return ColorResources.ANFGreen;
            }
            else
            {
                return ColorResources.ANFRed;
            }
        }

        /// <summary>
        /// Returns the message title for the referenced Cholesterol value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessageTitle()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 190)
            {
                return AppResources.BiometricWarningCongratulationsTitle;
            }
            else
            {
                return AppResources.BiometricWarningAtentionTitle;
            }
        }

        /// <summary>
        /// Returns the message for the referenced Cholesterol value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string GetMessage()
        {
            if (DataModel == null) return null;

            if (DataModel.Value < 190)
            {
                return AppResources.CholesterolWarningOKMessage;
            }
            else
            {
                return AppResources.CholesterolWarningHighMessage;
            }
        }

        #endregion

    }
}
