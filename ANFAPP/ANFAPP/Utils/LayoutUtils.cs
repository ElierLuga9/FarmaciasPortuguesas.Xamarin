using System;
using Xamarin.Forms;

namespace ANFAPP.Utils
{
	public static class LayoutUtils
	{
		public static bool IsLandscape(Page p)
		{
			return p.Width > p.Height;
		}
	}
}

