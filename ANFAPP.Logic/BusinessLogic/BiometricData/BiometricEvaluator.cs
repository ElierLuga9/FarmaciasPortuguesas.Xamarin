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
    public abstract class BiometricEvaluator<T> : IBiometricEvaluator where T : BiometricBase
    {

        #region Properties

        public T DataModel { get; set; }
        public User User { get; set; }

        public string Message
        {
            get
            {
                return GetMessage();
            }
        }

        public string MessageTitle {
            get
            {
                return GetMessageTitle();
            }
        }

        public Color EvaluationColor {
            get
            {
                return GetWarningColor();
            }
        }

        #endregion

        #region Constructors

        public BiometricEvaluator(T model, User user) 
        {
            DataModel = model;
            User = user;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Returns the warning color for the referenced value.
        /// </summary>
        /// <returns></returns>
        public abstract Color GetWarningColor();

        /// <summary>
        /// Returns the warning title for the referenced value.
        /// </summary>
        /// <returns></returns>
        public abstract string GetMessageTitle();

        /// <summary>
        /// Returns the warning message for the referenced value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string GetMessage();

        #endregion

    }
}
