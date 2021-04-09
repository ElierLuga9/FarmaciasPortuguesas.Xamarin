using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SQLite;
using Xamarin.Forms;
using ANFAPP.Logic.Utils;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic.Database
{
    class DatabaseHelper
    {

        /// <summary>
        /// Session Database Connection
        /// </summary>
        private static SQLiteConnection Database { get; set; }


        /// <summary>
        /// Returns the current Database Connection and initializes it if needed.
        /// </summary>
        /// <param name="manager"></param>
        public static SQLiteConnection GetDatabaseInstance()
        {
            // Initialize the SQL Connection if one wasn't done previously.
            if (Database == null) InitDatabase();

            return Database;
        }

        /// <summary>
        /// Initializes the database, creating a new database if one didn't already exist.
        /// </summary>
        private static void InitDatabase()
        {
            // Initialize the Database Connection
            Database = DependencyService.Get<ISQLite>().GetConnection();

            /// Crate tables
			
			// Biometric Data
			Database.CreateTable<IMC>();
			Database.CreateTable<Weight>();
            Database.CreateTable<Height>();
            Database.CreateTable<Glicemia>();
            Database.CreateTable<Cholesterol>();
			Database.CreateTable<Triglycerides>();
			Database.CreateTable<ArterialPressure>();
			Database.CreateTable<AbdominalPerimeter>();

			// Pharmacies
			Database.CreateTable<Pharmacy> ();
			Database.CreateTable<ShiftType> ();
			Database.CreateTable<ScheduleType>();
			Database.CreateTable<Localization> ();
			Database.CreateTable<ShiftPharmacy> ();

			// User
			Database.CreateTable<User>();
			Database.CreateTable<Voucher>();
			Database.CreateTable<Promotion>();


			// Dosage Scheduler
			Database.CreateTable<Dosage>();
			Database.CreateTable<Medicine>();
			Database.CreateTable<DosingSchedule>();
        }

    }
}
