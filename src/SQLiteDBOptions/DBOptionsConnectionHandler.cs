using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using NLog;

namespace SQLiteDBOptions
{
    /// <summary>
    /// A helper class to open (and close) a SQLite connection with all options from a DBOptions object applied
    /// </summary>
    public static class DBOptionsConnectionHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Opens a connection to a SQLite database file
        /// </summary>
        /// <param name="fileName">Filename of SQLite databse</param>
        /// <param name="options">DBOptions object contianing the objects that should be active</param>
        /// <returns>Open connection to the database</returns>
        public static SQLiteConnection CreateDatabase(string fileName, DBOptions options)
        {
            log.Debug("Creating database {0}", fileName);

            if (File.Exists(fileName) == false)
            {
                return OpenDatabase(fileName, options, CreateIfNotExisting: true);
            }
            else
            {
                throw new IOException(string.Format("Database {0} already exists", fileName));
            }
        }


        /// <summary>
        /// Opens a connection to a SQLite database file
        /// </summary>
        /// <param name="fileName">Filename of SQLite databse</param>
        /// <param name="options">DBOptions object contianing the objects that should be active</param>
        /// <param name="CreateIfNotExisting">If true, the database is created if required</param>
        /// <returns>Open connection to the database</returns>
        public static SQLiteConnection OpenDatabase(string fileName, DBOptions options, bool CreateIfNotExisting = true)
        {
            log.Debug("Opening database {0}", fileName);

            string connectionString = options.GenerateConnectionString(fileName);

            if (CreateIfNotExisting == true)
            {
                if (File.Exists(fileName) == false)
                {
                    //Create the database
                    SQLiteConnection.CreateFile(fileName);
                }
            }

            //Open it
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            //Now apply all PRAGMA options
            //ApplyOptions(connection, definedOptions, ignorePersistentOptions: false);
            options.ApplyOptions(connection);

            //TODO: Check if all options have their desired values!

            return connection;
        }


        /// <summary>
        /// Ensures that all options for this connection as set as definied in options. Throws an error if not.
        /// </summary>
        /// <param name="connection">Open connection to check</param>
        /// <param name="options">The definied options</param>
        public static void VerifyOptions(SQLiteConnection connection, DBOptions options)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Fully close the database connection, ensuring that it isn't held in any connection pool
        /// </summary>
        /// <param name="connection">The connection that should be closed</param>
        public static void CloseDatabase(SQLiteConnection connection)
        {
            //Make sure the connection is not held in any pool 
            SQLiteConnection.ClearAllPools();

            //Close the connection
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }

            //Just to be sure...
            SQLiteConnection.ClearAllPools();
        }

        /// <summary>
        /// Returns the values for the options from the given connection
        /// </summary>
        /// <param name="connection">Database connection to use</param>
        /// <param name="options">Definied options</param>
        /// <returns>List of the current values of the given DBOptions</returns>
        public static List<CurrentDBOptionValue> GetDBOptionValueList(SQLiteConnection connection, DBOptions options)
        {
            List<CurrentDBOptionValue> optionValueList = new List<CurrentDBOptionValue>();

            foreach (DBOptionBase option in options)
            {
                optionValueList.Add(option.GetCurrentOptionValue(connection));
            }

            return optionValueList;
        }

        /// <summary>
        /// Returns the value for all known options for the given connection
        /// </summary>
        /// <param name="connection">Open database connection to be used</param>
        /// <returns>List of the value for all known DBOptions</returns>
        public static List<CurrentDBOptionValue> GetAllDBOptionsValueList(SQLiteConnection connection)
        {
            return GetDBOptionValueList(connection, DBOptions.AllDBOptions());
        }

    }
}
