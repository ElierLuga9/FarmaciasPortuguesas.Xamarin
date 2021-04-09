using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ANFAPP.Logic.Database.Models
{
	
		[Table("Promotions")]
		public class Promotion
		{
			[PrimaryKey]
			public int Id { get; set; }
			public bool Read { get; set;}
			

		}
	
}


