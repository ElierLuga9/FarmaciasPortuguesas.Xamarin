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
using System.IO;
using Xamarin.Forms;
using ANFAPP.Droid.Utils;
using ANFAPP.Logic;
using ANFAPP.Logic.Utils;
using SQLite;

[assembly: Dependency(typeof(SQLite_Android))]

namespace ANFAPP.Droid.Utils
{
    
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }

        public SQLiteConnection GetConnection()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, Settings.DATABASE_NAME);

            // Create the connection
            // Return the database connection 
            return new SQLiteConnection(path);
        }
    }
}