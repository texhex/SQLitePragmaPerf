using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using Bytes2you.Validation;
using NLog;

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

        Logger log = LogManager.GetCurrentClassLogger();

        const string DataSourceTemplate = "Data Source = {0};";

        /// <summary>
        /// Creates a database in the definied path with a random name, applies all options and returns both the filename and the SQLiteConnection
        /// </summary>
        /// <param name="definedOptions">List of DBOptions that should be applied</param>
        /// <returns>An open database connection to the newly created database</returns>
        public Tuple<string, SQLiteConnection> CreateDatabase(List<DBOptionBase> definedOptions)
        {
            string fileName = Path.Combine(_folderPath, "SQLitePragmaPerf_" + Guid.NewGuid().ToString() + ".sqlite");

            //First create the database with all options set and store what those functions have returned as values
            List<DBOptionValue> optionValues = CreateDatabase(fileName, definedOptions);

            //This commands will result in an exception if ENCODING is already used
            //DBOptionValue crashBoomBang = new DBOptionValue("Encoding", "JUST FOR FUN");
            //optionValues.Add(crashBoomBang);

            //Make sure each item is only definied once.
            CheckOptionValueListForDuplicates(optionValues);
            
            //Now reopen the database again and check if we get the same values if we leave out all persistent options
            CheckPersistentDBOptions(fileName, definedOptions, optionValues);

            //If we are here, everything has worked so far. We can open the database and return the connection
            Tuple<string, SQLiteConnection> openResult = OpenDatabase(fileName, definedOptions);

            return openResult;
        }


        //Creates a database, as defined by the options and closes it again
        private List<DBOptionValue> CreateDatabase(string fileName, List<DBOptionBase> definedOptions)
        {
            log.Debug("Creating database...");

            string connectionString = string.Format(DataSourceTemplate, fileName);

            //First: Get all options that are definied as connection string parameters so they get applied from the start
            connectionString = ConstructConnectionString(connectionString, definedOptions, ignorePersistentOptions: false);
            log.Debug("Connection string: {0}", connectionString);

            //Create the database
            SQLiteConnection.CreateFile(fileName);

            //Open it
             SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            //Now apply all PRAGMA options
            ApplyOptions(connection, definedOptions, ignorePersistentOptions: false);

            //Finaly, create a TABLE to make sure that SQLite considers this database not empty
            SQLiteCommand createTable = new SQLiteCommand("CREATE TABLE ZZZ_PRAGMA_PERF_TEMP_TABLE(name TEXT);", connection);
            createTable.ExecuteNonQuery();

            SQLiteCommand addData = new SQLiteCommand("INSERT INTO ZZZ_PRAGMA_PERF_TEMP_TABLE values('JUST A TEST');", connection);
            addData.ExecuteNonQuery();


            //Now query all options for their current values and store it in our list
            List<DBOptionValue> optionValueList = GetOptionValueList(connection, definedOptions);

            //Close the connection
            connection.Close();

            //Make sure the connection is not held in any pool 
            SQLiteConnection.ClearAllPools();

            return optionValueList;
        }

        private Tuple<string, SQLiteConnection> OpenDatabase(string fileName, List<DBOptionBase> definedOptions)
        {
            log.Debug("Opening database...");

            string connectionString = string.Format(DataSourceTemplate, fileName);

            connectionString = ConstructConnectionString(connectionString, definedOptions, ignorePersistentOptions: false);
            log.Debug("Connection string: {0}", connectionString);

            //Open it
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            //Now apply all PRAGMA options
            ApplyOptions(connection, definedOptions, ignorePersistentOptions: false);

            return new Tuple<string, SQLiteConnection>(connectionString, connection);
        }

        //Open a database but ignores all options that have IsPersistent=true and then checks the value of all options with the expected list.
        //This ensures that a persistent option is really persistent
        private void CheckPersistentDBOptions(string fileName, List<DBOptionBase> definedOptions, List<DBOptionValue> expectedOptionValues)
        {
            log.Debug("Performing persistent option check...");

            string connectionString = string.Format(DataSourceTemplate, fileName);

            //Construct connection string without any persistent option
            connectionString = ConstructConnectionString(connectionString, definedOptions, ignorePersistentOptions: true);
            log.Debug("Connection string without persistent options: {0}", connectionString);

            //Open the database
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            //Apply options but ignore any persistent option
            ApplyOptions(connection, definedOptions, ignorePersistentOptions: true);

            //Query all options for their current values 
            List<DBOptionValue> optionValueList = GetOptionValueList(connection, definedOptions);

            //Close the connection
            connection.Close();

            //Make sure the connection is not held in any pool 
            SQLiteConnection.ClearAllPools();

            //Now compare the expected values with the current values
            foreach (DBOptionValue expected in expectedOptionValues)
            {
                foreach (DBOptionValue current in optionValueList)
                {
                    if (current.Name == expected.Name)
                    {
                        if (current.DisplayValue != expected.DisplayValue)
                        {
                            throw new InvalidOperationException(string.Format(
                                "The option [{0}] is marked as persistent, but reopening the database resulted in a different value than configured: Actual [{1}], expected [{2}]",
                                expected.Name, current.DisplayValue, expected.DisplayValue
                                ));
                        }
                    }
                }

            }

        }



        /// <summary>
        /// Returns a list of all known options and their values
        /// </summary>
        /// <param name="connection">TList of DBOptionValue</param>
        /// <returns></returns>
        public List<DBOptionValue> GetAllOptionValueList(SQLiteConnection connection)
        {
            return GetOptionValueList(connection, DBOptionSets.AllKnownOptions());
        }


        private List<DBOptionValue> GetOptionValueList(SQLiteConnection connection, List<DBOptionBase> definedOptions)
        {
            List<DBOptionValue> optionValueList = new List<DBOptionValue>();

            foreach (DBOptionBase option in definedOptions)
            {
                optionValueList.Add(option.ExportActiveValue(connection));
            }

            return optionValueList;
        }


        //Checks the given list for duplicates in order to avoid strange effects
        private void CheckOptionValueListForDuplicates(List<DBOptionValue> list)
        {
            foreach (DBOptionValue itemOuterLoop in list)
            {
                //now search the inner list for the outer item

                int counter = 0;
                foreach (DBOptionValue itemInnerLoop in list)
                {
                    if (itemInnerLoop.Name == itemOuterLoop.Name)
                    {
                        counter++;
                    }
                }

                //If counter>1, we have an duplicate item
                if (counter > 1)
                {
                    throw new NotSupportedException(string.Format("Only one DBOption of the same type is supported, but [{0}] was used more than once", itemOuterLoop.Name));
                }

            }

        }

        //Construct a connection string based on the list of options
        private string ConstructConnectionString(string connectionString, List<DBOptionBase> definedOptions, bool ignorePersistentOptions)
        {
            foreach (DBOptionBase option in definedOptions)
            {
                if (option is IDBOptionConnectionStringParameter)
                {
                    if (option.TargetValueSet)
                    {

                        if (ignorePersistentOptions && option.IsPersistent)
                        {
                            //do nothing
                        }
                        else
                        {
                            log.Debug("Applying ConnectionStringParameter option: {0} ", option.OptionName);

                            IDBOptionConnectionStringParameter connectionStringOption = (IDBOptionConnectionStringParameter)option;
                            connectionString = connectionStringOption.ApplyToConnectionString(connectionString);
                        }

                    }
                }
            }

            return connectionString;
        }

        //Applies DBoptions to a connection 
        private void ApplyOptions(SQLiteConnection connection, List<DBOptionBase> definedOptions, bool ignorePersistentOptions)
        {
            foreach (DBOptionBase option in definedOptions)
            {
                if (option is IDBOptionPragma)
                {
                    if (option.TargetValueSet)
                    {

                        if (ignorePersistentOptions && option.IsPersistent)
                        {
                            //do nothing
                        }
                        else
                        {
                            log.Debug("Applying PRAGMA option: {0} ", option.OptionName);

                            IDBOptionPragma pragmaOption = (IDBOptionPragma)option;
                            pragmaOption.ApplyToDatabase(connection);
                        }

                    }
                }
            }
        }



    }
}