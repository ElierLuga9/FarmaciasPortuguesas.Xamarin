using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ANFAPP.Utils
{
	public static class UIUtils
	{
		public static ContentPage FindParentPage(Element el = null)
		{
			if (el == null) return null;
			
			return (el is ContentPage) ? (ContentPage)el
					: (el.Parent != null) ? FindParentPage(el.Parent)
					: null;
		}
	}
}
