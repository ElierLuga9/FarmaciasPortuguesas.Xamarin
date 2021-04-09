using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Helpers
{
	public class BindablePickerItem
	{

		#region Properties
		
		public int Code { get; set; }
		public string Name { get; set; }

		#endregion

		/// <summary>
		/// Implement toString so the name shows on the picker.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}

	}
}
