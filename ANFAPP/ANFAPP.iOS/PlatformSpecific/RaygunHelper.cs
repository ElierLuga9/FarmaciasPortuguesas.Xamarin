using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Exceptions;
using Xamarin.Forms;
using ANFAPP.iOS.PlatformSpecific;

[assembly: Dependency(typeof(RaygunHelper))]
namespace ANFAPP.iOS.PlatformSpecific
{
	public class RaygunHelper : IRaygunHelper
	{
		/*
		void Current_SendingMessage(object sender, Mindscape.Raygun4Net.RaygunSendingMessageEventArgs e)
		{
			throw new NotImplementedException();
		}
		*/	
		public void LogEvent(string eventStr)
		{
			//Mindscape.Raygun4Net.RaygunClient.Current.SendingMessage += Current_SendingMessage;

			Mindscape.Raygun4Net.RaygunClient.Current.Send(new RaygunException(eventStr));

		}
	}
}