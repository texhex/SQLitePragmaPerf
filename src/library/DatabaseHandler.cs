using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using Bytes2you.Validation;
using NLog;
using SQLiteDBOptions;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// This class handles the database connection for the test run
    /// </summary>
    public class DatabaseHandler
    {
        private DatabaseHandler()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a database handler 
        /// </summary>
        /// <param name="folderPath">Directory where all database can be created</param>
        public DatabaseHandler(string folderPath)
        {
            Guard.WhenArgument(folderPath, "folderPath").IsNullOrWhiteSpace().Throw();

            _folderPath = Path.GetFullPath(folderPath);

            log.Debug("Created with path {0}", _folderPath);
        }

        string _folderPath = "";

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        const string DataSourceTemplate = "Data Source = {0};";

        /// <summary>
        /// Creates a database in the definied path with a random name, applies all options and returns both the connection string and the SQLiteConnection
        /// </summary>
        /// <param name="definedOptions">List of DBOptions that should be applied</param>
        /// <returns>An open database connection to the newly created database</returns>
        public Tuple<string, SQLiteConnection> CreateDatabase(DBOptions definedOptions)
        {
            //First check for duplicate inside the options. Make sure each item is only definied once.
            definedOptions.CheckForDuplicates();

            string fileName = Path.Combine(_folderPath, "SQLitePragmaPerf_" + Guid.NewGuid().ToString() + ".sqlite");


            //Create the database with all options set 
            SQLiteConnection connection = DBOptionsConnectionHandler.CreateDatabase(fileName, definedOptions);

            //Finaly, create a TABLE to make sure that SQLite considers this database not empty
            CreateTable(connection);

            //Now query all options for their current values and store it in our list
            List<CurrentDBOptionValue> currentDBOptionValues = DBOptionsConnectionHandler.GetAllDBOptionsValueList(connection);

            //Close the connection to make sure that any DBOption that requires a open/close is satisfied
            DBOptionsConnectionHandler.CloseDatabase(connection);



            //Now reopen the database again and check if we get the same values if we leave out all persistent options
            CheckPersistentDBOptions(fileName, definedOptions, currentDBOptionValues);


            //If we are here, everything has worked so far. We can open the database and return the connection
            connection = DBOptionsConnectionHandler.OpenDatabase(fileName, definedOptions);

            //Debug output of all options
            List<CurrentDBOptionValue> allValues = DBOptionsConnectionHandler.GetAllDBOptionsValueList(connection);
            log.Debug("Database {0} created, DBOptions: ", fileName);
            foreach (CurrentDBOptionValue currentValue in allValues)
            {
                log.Debug("   {0}", currentValue.ToString());
            }

            return new Tuple<string, SQLiteConnection>(fileName, connection);
        }


        private void CreateTable(SQLiteConnection connection)
        {

            using (SQLiteCommand createTable = new SQLiteCommand("CREATE TABLE ZZZ_PRAGMA_PERF_TEMP_TABLE(name TEXT);", connection))
            {
                createTable.ExecuteNonQuery();
            }


            using (SQLiteCommand addData = new SQLiteCommand("INSERT INTO ZZZ_PRAGMA_PERF_TEMP_TABLE values('JUST A TEST');", connection))
            {
                addData.ExecuteNonQuery();
            }

        }


        //Open a database but ignores all options that have IsPersistent=true and then checks the value of all options with the expected list.
        //This ensures that a persistent option is really persistent
        private void CheckPersistentDBOptions(string fileName, DBOptions options, List<CurrentDBOptionValue> expectedOptionValues)
        {
            log.Debug("Performing persistent option check...");

            //Create an options object without any option that is configured "Persistent"
            DBOptions optionsWithoutPersistent = new DBOptions();

            foreach (DBOptionBase option in options)
            {
                if (option.IsPersistent == false)
                {
                    optionsWithoutPersistent.Add(option);
                }
            }

            //string connectionString = optionsWithoutPersistent.GenerateConnectionString(fileName);
            SQLiteConnection connection = DBOptionsConnectionHandler.OpenDatabase(fileName, optionsWithoutPersistent);

            //Query all options for their values
            List<CurrentDBOptionValue> optionValueList = DBOptionsConnectionHandler.GetAllDBOptionsValueList(connection);

            DBOptionsConnectionHandler.CloseDatabase(connection);


            //Now compare the expected values with the current values
            foreach (CurrentDBOptionValue expected in expectedOptionValues)
            {
                foreach (CurrentDBOptionValue current in optionValueList)
                {
                    if (current.Name == expected.Name)
                    {
                        if (current.DisplayValue != expected.DisplayValue)
                        {
                            throw new InvalidOperationException(string.Format(
                                "The option [{0}] is marked as persistent, but reopening the database resulted in a different value than configured: Current value from  open test [{1}], expected [{2}]",
                                expected.Name, current.DisplayValue, expected.DisplayValue
                                ));
                        }
                    }
                }

            }

        }




    }
}