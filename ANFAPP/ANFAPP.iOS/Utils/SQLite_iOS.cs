using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using ANFAPP.iOS.Utils;
using ANFAPP.Logic.Utils;
using System.IO;
using ANFAPP.Logic;
using SQLite;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace ANFAPP.iOS.Utils
{
    
    public class SQLite_iOS : ISQLite
    {

        public SQLite_iOS() { }

        public SQLiteConnection GetConnection()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, Settings.DATABASE_NAME);

			// The previous PCL version used a string by default to represent DateTime's.
			return new SQLiteConnection(path, false);
        }
    }
}