using ANFAPP.Logic.Utils;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(ANFAPP.WinPhone.MixPanel_WP))]
namespace ANFAPP.WinPhone
{

    /// <summary>
    /// WP MixPanel implementation.
    /// </summary>
    public class MixPanel_WP : IMixPanel
    {

        public void SharedInstanceWithToken (string token)
        {
            
        }

        public void Identify (string identifier)
        {
            
        }

		public void Reset() { }

        public void Track (string name)
        {
           
        }

        public void TrackProperties (string name, IDictionary<string, string> properties)
        {

        }

        public string DistinctId ()
        {
			return null;
        }

        public void CreateAliasForDistinctId (string alias, string distinctId)
        {

        }

        public void PeopleSet(IDictionary<string, string> properties)
        {

        }

        public void PeopleIncrement(string key, double value)
        {

        }
    }
}

