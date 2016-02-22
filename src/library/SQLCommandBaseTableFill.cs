using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Base class for SQLCommands that fill tables
    /// </summary>
    public abstract class SQLCommandBaseTableFill : SQLCommandBase
    {
        public SQLCommandBaseTableFill()
        {
            this.CommandType = SQLitePragmaPerf.CommandType.GenerateData;
        }
        

        public override TimeSpan Execute(SQLiteConnection connection, int batchSize, int rowsRequested)
        {
            Log.Debug("Running for {0} rows (Batch size {1})...", rowsRequested, batchSize);

            VerifyExecuteParameters(connection, batchSize, rowsRequested);

            TimeSpan result;

            //The question is if prepared statements really make that big difference because normally the engine 
            //should detect if the same command is executed over and over again: http://stackoverflow.com/a/21376455/612954
            //
            //However, I assume that telling SQlite that we are reusing a statement can't be wrong
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text; //just to be sure
                PrepareCommand(command);

                //The first thing we need to do is the get the entire function JIT compiled or else our measurement of the first row is BS.
                //For this, we let it run with a batch and rows of 1 and later on subtract this from the "real" execution.
                //This nullifies the measurement for one row, but that’s better than having the JIT time measured.

                //Warm up round with only one row
                result=ExecuteInternal(connection, command, batchSize, 1);

                //Now the real run with rows -1
                result=ExecuteInternal(connection, command, batchSize, rowsRequested-1);
            }

            Log.Debug("  Done in {0}ms", (long)result.TotalMilliseconds);
            return result;
        }

        //Real execute function 
        private TimeSpan ExecuteInternal(SQLiteConnection connection, SQLiteCommand command, int batchSize, int rowsRequested)
        {
            Stopwatch swatch = Stopwatch.StartNew();

            //Execute this loop while we do not have reached the requested number of rows
            int rowsProcessed = 0;
            while (rowsProcessed < rowsRequested)
            {
                //Counts how many rows we have processed already for this batch
                int rowsInBatchProcessed = 0;

                //Open a transaction and use it for the current batch
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    while (rowsProcessed < rowsRequested && rowsInBatchProcessed < batchSize)
                    {
                        FillParameters(command);

                        command.ExecuteNonQuery();

                        rowsInBatchProcessed++;
                        rowsProcessed++;
                    }
                    transaction.Commit();
                }
            }

            swatch.Stop();

            return swatch.Elapsed;
        }



        /// <summary>
        /// Used to define the statement (parameters etc.) which will then be used in a loop
        /// </summary>
        /// <param name="command"></param>
        protected abstract void PrepareCommand(SQLiteCommand command);

        /// <summary>
        /// Used to define the actual values for the parameters. This method might be called several thousand times, so be careful!
        /// </summary>
        /// <param name="command"></param>
        protected abstract void FillParameters(SQLiteCommand command);


    }
}
