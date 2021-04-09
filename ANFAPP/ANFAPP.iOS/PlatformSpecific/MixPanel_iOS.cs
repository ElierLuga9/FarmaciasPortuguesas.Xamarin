using System.Collections.Generic;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic;
using AdSupport;

[assembly: Xamarin.Forms.Dependency (typeof(ANFAPP.iOS.MixPanel_iOS))]
namespace ANFAPP.iOS
{
	using Foundation;
	using Mixpanel;

	/// <summary>
	/// iOS MixPanel implementation.
	/// </summary>
	public class MixPanel_iOS : IMixPanel
	{
		public void SharedInstanceWithToken (string token)
		{
			Mixpanel.SharedInstanceWithToken (token);
		}

		public void Identify (string identifier)
		{
			Mixpanel.SharedInstance.Identify (identifier);
		}

		public void Track (string name)
		{
//			Mixpanel.SharedInstance.Track (name);
			TrackProperties(name, new Dictionary<string, string>());
		}

		public void TrackProperties (string name, IDictionary<string, string> properties)
		{
			NSMutableDictionary dict = new NSMutableDictionary();
			if (properties != null)
			{
				properties.Add("App/Site", "App");
				properties.Add("SessionID", ASIdentifierManager.SharedManager.AdvertisingIdentifier.AsString());

				if (SessionData.PharmacyUser != null) {
					properties["Email"] = SessionData.PharmacyUser.Username;
					properties["CardNumber"] = SessionData.PharmacyUser.CardNumber;
				}
			}

			foreach(KeyValuePair<string, string> entry in properties)
			{
				dict.Add(NSObject.FromObject(entry.Key), NSObject.FromObject(entry.Value));
			}

			Mixpanel.SharedInstance.Track (name, dict);
		}

		public string DistinctId ()
		{
			return Mixpanel.SharedInstance.DistinctId;
		}

		public void CreateAliasForDistinctId (string alias, string distinctId)
		{
			Mixpanel.SharedInstance.CreateAlias (alias, distinctId);
		}

		public void PeopleSet(IDictionary<string, string> properties)
		{
			NSMutableDictionary dict = new NSMutableDictionary();
			foreach(KeyValuePair<string, string> entry in properties)
			{
				dict.Add(NSObject.FromObject(entry.Key), NSObject.FromObject(entry.Value));
			}

			Mixpanel.SharedInstance.People.Set(dict);
		}

		public void PeopleIncrement(string key, double value)
		{
			//NSDictionary dict = NSDictionary.FromObjectAndKey(NSObject.FromObject(value), NSObject.FromObject(key));
//			Mixpanel.SharedInstance.People.Increment(key, NSNumber.FromDouble(value));
			Mixpanel.SharedInstance.People.Set(key, NSNumber.FromDouble(value));
		}

		public void Reset() {
			
			Mixpanel.SharedInstance.Reset();
		}

	}
}

