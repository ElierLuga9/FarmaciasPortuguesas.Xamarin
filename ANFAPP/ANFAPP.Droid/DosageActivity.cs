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
using ANFAPP.Logic.ViewModels;

namespace ANFAPP.Droid
{
	[Activity(NoHistory=true, ExcludeFromRecents=true)]
	public class DosageActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var id = Intent.GetStringExtra(MainActivity.PARSE_INTENT_CONTEXTID);
			if (!string.IsNullOrEmpty(id)) 
			{
				var vm = new DosageViewModel();
				vm.MarkAsDone(id);
			}

			Finish();
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			return false;
		}
		
	}
}