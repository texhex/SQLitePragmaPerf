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
    enum ExecutionType
    {
        Init,
        Exec,
        CleanUp
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

        /// <summary>
        /// Can be used to prepare the database for the exec of this command. 
        /// The time required for this command is ignored. 
        /// </summary>
        /// <param name="connection">The open database connection to be used</param>
        public virtual void Initialize(SQLiteConnection connection)
        {
            //Make sure we get something useful
            Guard.WhenArgument(connection, "connection").IsNull().Throw();
        }


        /// <summary>
        /// Executes the statement against a database and measures the time is required to execute it
        /// </summary>
        /// <param name="connection">The database to execute against</param>
        /// <param name="batchSize">Process how many rows in one batch</param>
        /// <param name="maxRows">Maximum rows that should be processed</param>
        /// <param name="warumUp">True if this is only a test run to get the code compiled</param>
        //TODO: warmUp shouldn't be necessary, this should be handle by the class itself
        public abstract TimeSpan Execute(SQLiteConnection connection, int batchSize, int maxRows, bool warumUp);


        /// <summary>
        /// Used to check that the given parameters to Execute make sense
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="batchSize"></param>
        /// <param name="maxRows"></param>
        /// <param name="testRun"></param>
        protected void VerifyExecuteParameters(SQLiteConnection connection, int batchSize, int maxRows, bool warumUp)
        {
            //Check parameters
            Guard.WhenArgument(connection, "connection").IsNull().Throw();
            Guard.WhenArgument(batchSize, "batchSize").IsLessThanOrEqual(0).Throw();
            Guard.WhenArgument(maxRows, "maxRows").IsLessThanOrEqual(0).Throw();

            //The batchSize should never be bigger than maxRows
            Guard.WhenArgument(batchSize, "batchSize").IsGreaterThan(maxRows).Throw();

        }


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
