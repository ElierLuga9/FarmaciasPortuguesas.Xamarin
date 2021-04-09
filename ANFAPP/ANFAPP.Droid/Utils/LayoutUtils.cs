using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ANFAPP.Droid.Utils
{
    public static class LayoutUtils
    {

        /// <summary>
        /// Convert dp to px.
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static int DpToPx(int dp, Android.Content.Res.Resources resources)
        {
            return (int) Math.Round(resources.DisplayMetrics.Density * dp);
        }

    }
}