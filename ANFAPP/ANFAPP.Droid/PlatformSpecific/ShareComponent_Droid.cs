using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ANFAPP.Logic.Utils;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using ANFAPP.Logic;
using Org.Json;
using Xamarin.Forms;
using Xamarin.Social;
using Xamarin.Social.Services;



[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.Droid.PlatformSpecific.ShareComponent_Droid))]

namespace ANFAPP.Droid.PlatformSpecific
{
    public class ShareComponent_Droid : Java.Lang.Object, IShareComponent
    {
        #region Properties

        protected static Activity ParentActivity;
        protected static ICallbackManager Callback;
       
        #endregion

        public void InitService(object contextActivity)
        {
            if (contextActivity == null || !(contextActivity is Activity)) return;
            ParentActivity = contextActivity as Activity;
        }

        public void FacebookShare(string url) 
        {
           /* // 1. Create the service
            var facebook = new FacebookService { ClientId = ParentActivity.Resources.GetString(Resource.String.app_id) };

            // 2. Create an item to share
            var item = new Item { Text = "http://www.google.pt" };
            item.Links.Add(new Uri("http://www.google.pt"));

            // 3. Present the UI on Android
            var shareIntent = facebook.GetShareUI(ParentActivity, item, result =>
            {
                // result lets you know if the user shared the item or canceled
            });
            ParentActivity.StartActivityForResult(shareIntent, 42);*/
            

        }

        public void TwitterShare(string url) { }


    }
}