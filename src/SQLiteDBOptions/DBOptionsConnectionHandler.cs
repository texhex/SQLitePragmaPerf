using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLiteDBOptions
{
    /// <summary>
    /// A helper class to open (and close) a SQLite connection with all options from a DBOptions object applied
    /// </summary>
    public static class DBOptionsConnectionHandler
    {
        /// <summary>
        /// Opens a connection to a SQLite database file
        /// </summary>
        /// <param name="fileName">Filename of SQLite databse</param>
        /// <param name="options">DBOptions object contianing the objects that should be active</param>
        /// <returns></returns>
        public static SQLiteConnection OpenDatabase(string fileName, DBOptions options, bool CreateIfNotExisting=true)
        {
            throw new NotImplementedException();

            //TODO: Check if all options have their desired values!
        }

        public static SQLiteConnection CreateDatabase(string fileName, DBOptions options)
        {
            throw new NotImplementedException();
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
        public static void CloseConnection(SQLiteConnection connection)
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
    }
}
