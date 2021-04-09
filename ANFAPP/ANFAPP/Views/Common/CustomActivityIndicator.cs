using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Views.Common
{
	public class CustomActivityIndicator : ActivityIndicator
	{

		public CustomActivityIndicator()
		{
			// Hack for Windows Phone, otherwise the indicator will be 1px wide.
			if (Device.OS == TargetPlatform.WinPhone) WidthRequest = 400;
		}
	}
}
