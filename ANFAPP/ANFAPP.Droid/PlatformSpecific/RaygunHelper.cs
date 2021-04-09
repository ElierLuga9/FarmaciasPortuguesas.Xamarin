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
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Exceptions;
using ANFAPP.Droid.PlatformSpecific;
using Xamarin.Forms;

[assembly: Dependency(typeof(RaygunHelper))]
namespace ANFAPP.Droid.PlatformSpecific
{
	public class RaygunHelper : IRaygunHelper
	{

		public void LogEvent(string eventStr)
		{
			Mindscape.Raygun4Net.RaygunClient.Current.Send(new RaygunException(eventStr));
		}
	}
}