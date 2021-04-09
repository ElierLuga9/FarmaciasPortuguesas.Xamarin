using System;
using System.Collections.Generic;

namespace ANFAPP.Logic.Utils
{
	public interface IMixPanel
	{
		void SharedInstanceWithToken (string token);

		void Identify (string identifier);

		/// <summary>
		/// Tracks an event.
		/// </summary>
		/// <param name="name">The event name.</param>
		void Track (string name);

		/// <summary>
		/// Tracks an event.
		/// </summary>
		/// <param name="name">The event name.</param>
		/// <param name="properties">The event properties.</param>
		void TrackProperties (string name, IDictionary<string, string> properties);

		string DistinctId ();

		void CreateAliasForDistinctId (string alias, string distinctId);

		void PeopleSet(IDictionary<string, string> properties);

		void PeopleIncrement(string key, double value);
		//void Pushandling(string googleKey);

		void Reset();
	}
}

