using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Data.SQLite;
using Bytes2you.Validation;

namespace SQLitePragmaPerf
{
    public enum CommandType
    {
        GenerateData,
        UseData,
        RemoveData
    }
    

    /// <summary>
    /// Base class for SQLCommands that are executed against a database
    /// </summary>
    public abstract class SQLCommandBase
    {
        protected SQLCommandBase()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

        //This can directly be used by a sub class 
        protected Logger Log
        {
            get; private set;
        }

        public CommandType CommandType
        {
            get; protected set;
        }

        /// <summary>
        /// Can be used to prepare the database for the exec of this command. 
        /// The time required for this command is ignored. 
        /// </summary>
        /// <param name="connection">The open database connection to be used</param>
        public void Initialize(SQLiteConnection connection)
        {
            VerfiyConnection(connection);

            PrepareDatabase(connection);
        }

        /// <summary>
        /// Prepare the database for this command (Create table schema etc.)
        /// </summary>
        /// <param name="connection"></param>
        protected abstract void PrepareDatabase(SQLiteConnection connection);


        /// <summary>
        /// Executes this statement against a database and measures the time that was required to execute it
        /// </summary>
        /// <param name="connection">The database to execute against</param>
        /// <param name="batchSize">Process how many rows in one batch</param>
        /// <param name="rowsRequested">How many ows that should be generated</param>
        public abstract TimeSpan Execute(SQLiteConnection connection, int batchSize, int rowsRequested);

        /// <summary>
        /// Same as Execute but all values are considered to be 1000, which means batchSize=10 is actually "batchSize=10.000"
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="batchSizeKilo"></param>
        /// <param name="rowsRequestedKilo"></param>
        /// <returns></returns>
        public TimeSpan ExecuteKilo(SQLiteConnection connection, int batchSizeKilo, int rowsRequestedKilo)
        {
            return Execute(connection, batchSizeKilo * 1000, rowsRequestedKilo * 1000);
        }


        /// <summary>
        /// Ensure that the connection is set and valid
        /// </summary>
        /// <param name="connection"></param>
        protected void VerfiyConnection(SQLiteConnection connection)
        {
            Guard.WhenArgument(connection, "connection").IsNull().Throw();

            if (connection.State!=System.Data.ConnectionState.Open)
            {
                throw new ArgumentException("The connection must be OPEN");
            }

        }

        /// <summary>
        /// Used to check that the given parameters to Execute make sense
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="batchSize"></param>
        /// <param name="rowsRequested"></param>
        protected void VerifyExecuteParameters(SQLiteConnection connection, int batchSize, int rowsRequested)
        {
            VerfiyConnection(connection);

            //Check parameters
            Guard.WhenArgument(batchSize, "batchSize").IsLessThanOrEqual(0).Throw();
            Guard.WhenArgument(rowsRequested, "rowsRequested").IsLessThanOrEqual(0).Throw();

            //The batchSize should not be bigger than maxRows
            Guard.WhenArgument(batchSize, "batchSize").IsGreaterThan(rowsRequested).Throw();

        }


        //Helper function to execute a non query single command.
        //This command is only executed from the objects directly, no user input is involved
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected void ExecuteSQL(SQLiteConnection connection, string statement)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = statement;
                //This will always return -1
                command.ExecuteNonQuery();
            }
        }

    }
}
