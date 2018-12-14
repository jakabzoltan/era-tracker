using System;
using System.IO;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.Models;
using SQLite;
using Xamarin.Forms;

namespace Diabetes_Tracker.Database
{
    public class DatabaseContext : IDisposable
    {

        private static readonly VersionInfo DbVersion = new VersionInfo()
        {
            Id = 0,
            Version = "1.12"
        };

        private static readonly string SqlName = "LocalDiabetesTracker";
        private static string DbPath
        {
            get
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    string lib = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    return Path.Combine(lib, SqlName);
                }
                else
                {
                    string documentsPath =
                        Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                    string lib = Path.Combine(documentsPath, "..", "Library"); // Library folder instead
                    return Path.Combine(lib, SqlName);
                }
            }
        }
        public SQLiteConnection Context { get; set; }
        public DatabaseContext()
        {
            Context = new SQLiteConnection(DbPath);
        }

        public bool IsInitialized()
        {
            try
            {
                var init = Context.Get<VersionInfo>(0);
                return init.Version == DbVersion.Version || WipeDb(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
        /// <summary>
        /// Initializes the database if it has not been initialized already.
        /// </summary>
        public void Init()
        {
            if (IsInitialized()) return;
            Context.CreateTable<VersionInfo>(); //versions
            Context.CreateTable<UserContext>(); //profiles
            Context.CreateTable<LogbookItem>(); //Users Logbook Items
            Context.CreateTable<GlucoseMeasurement>(); //Glucose Items
            Context.CreateTable<GlucoseMeasurementContext>();
            // insert version info
            Context.Insert(DbVersion);
            // seed data
        }

        /// <summary>
        /// this method will contact the API that will attempt to pull data in from the server and put it into the database. The application can be set to autosync, or can be manually synced - TO BE IMPLEMTED
        /// </summary>
        public void Sync()
        {

        }
        /// <summary>
        /// Meant to wipe the database and recreate it. 
        /// </summary>
        /// <param name="reinitialize">Reinitializes the base of the database</param>
        /// <returns></returns>
        public bool WipeDb(bool reinitialize)
        {
            try
            {
                Context.DropTable<VersionInfo>(); //versions
                Context.DropTable<UserContext>(); //profiles
                Context.DropTable<LogbookItem>(); //Users Logbook Items
                Context.DropTable<GlucoseMeasurement>(); //Glucose Items
                if (reinitialize) Init();
                return true;
            }
            catch
            {
                return false;
            }

        }


        public void Dispose()
        {
            Context?.Dispose();
        }

        private class VersionInfo
        {
            public int Id { get; set; }
            public string Version { get; set; }
        }

    }
}