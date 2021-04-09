using System.Collections.Generic;
using ANFAPP.Logic.Utils;
using Mixpanel.Android.MPMetrics;
using Org.Json;
using ANFAPP.Logic;
using Android.Gms.Ads.Identifier;
using Java.Util;

[assembly: Xamarin.Forms.Dependency (typeof(ANFAPP.Droid.MixPanel_Droid))]
namespace ANFAPP.Droid
{

    /// <summary>
    /// iOS MixPanel implementation.
    /// </summary>
    public class MixPanel_Droid : IMixPanel
    {

        #region Properties

        private MixpanelAPI ApiSharedInstance { get; set; }

        #endregion

        public void SharedInstanceWithToken (string token)
        {
            ApiSharedInstance = MixpanelAPI.GetInstance(ANFApplication.GetInstance().BaseContext, token);

		}

        public void Identify (string identifier)
        {
            if (ApiSharedInstance == null) return;

            ApiSharedInstance.People.Identify(identifier);
			ApiSharedInstance.Identify(identifier);
        }

        public void Track (string name)
        {
            if (ApiSharedInstance == null) return;

//            ApiSharedInstance.Track(name, null);
			TrackProperties(name, new Dictionary<string, string>());
        }

        public void TrackProperties (string name, IDictionary<string, string> properties)
        {
            if (ApiSharedInstance == null) return;
			if (properties != null)
			{
				properties.Add("App/Site", "App");
				properties.Add("SessionID", ANFApplication.AAID);

				if (SessionData.PharmacyUser != null) {
					properties["Email"] = SessionData.PharmacyUser.Username;
					properties["CardNumber"] = SessionData.PharmacyUser.CardNumber;
				}
			}

            JSONObject jsonObj = new JSONObject();
            foreach (KeyValuePair<string, string> entry in properties)
            {
                jsonObj.Put(entry.Key, entry.Value);
            }

            ApiSharedInstance.Track(name, jsonObj);
        }

        public string DistinctId ()
        {
            return ApiSharedInstance != null ? ApiSharedInstance.DistinctId : null;
        }

        public void CreateAliasForDistinctId (string alias, string distinctId)
        {
            if (ApiSharedInstance == null) return;

            ApiSharedInstance.Alias(alias, distinctId);
        }

        public void PeopleSet(IDictionary<string, string> properties)
        {
            if (ApiSharedInstance == null) return;

            JSONObject jsonObj = new JSONObject();
            foreach (KeyValuePair<string, string> entry in properties)
            {
                jsonObj.Put(entry.Key, entry.Value);
            }

            ApiSharedInstance.People.Set(jsonObj);
        }

        public void PeopleIncrement(string key, double value)
        {
            if (ApiSharedInstance == null) return;

			ApiSharedInstance.People.Set(key, value);
        }

		public void Reset() {
			ApiSharedInstance.ClearSuperProperties();
			var newID = UUID.RandomUUID().ToString();
			Identify(newID);
		}
    }
}

