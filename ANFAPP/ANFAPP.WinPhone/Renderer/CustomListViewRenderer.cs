using ANFAPP.Views.Common;
using ANFAPP.WinPhone.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace ANFAPP.WinPhone.Renderer
{
	public class CustomListViewRenderer : ListViewRenderer
	{

		#region Rendering 

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);

			if (Control != null) Control.Style = App.Current.Resources["CustomListViewStyle"] as System.Windows.Style;
		}

		#endregion

	}
}
