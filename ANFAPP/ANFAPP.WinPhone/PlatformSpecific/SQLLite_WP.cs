using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using Xamarin.Forms;
using ANFAPP.WinPhone.Utils;
using SQLite;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.WindowsPhone8;
using Windows.Storage;

[assembly: Dependency(typeof(SQLLite_WP))]

namespace ANFAPP.WinPhone.Utils
{
    
    public class SQLLite_WP : ISQLite
    {
		public SQLLite_WP() { }

        public SQLiteConnection GetConnection()
        {
			var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, Settings.DATABASE_NAME);	

            // Create the connection
            // Return the database connection 
            return new SQLiteConnection(new SQLitePlatformWP8(), path);
        }
    }
}