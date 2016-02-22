using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    public class SQLCommandFieldingOfPlayerNames : SQLCommandBaseUseData
    {
        protected override void PrepareDatabase(SQLiteConnection connection)
        {
            ExecuteSQL(connection, "CREATE TABLE FieldingOfPlayerNames (playerID TEXT, yearID NUMERIC, Name TEXT, NameGiven TEXT);");
        }

        public override TimeSpan Execute(SQLiteConnection connection, int batchSize, int rowsRequested)
        {
            Log.Debug("Running command from class {0}...", this.GetType().ToString());

            VerifyExecuteParameters(connection, batchSize, rowsRequested);

            TimeSpan result;

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text; //just to be sure
                
                //TODO: I'm not quite sure if this command is working correctly. Needs further investigation.

                command.CommandText = "INSERT INTO FieldingOfPlayerNames " +
                                      "SELECT F.PlayerID, F.YearID, " +
                                      "(M.NameFirst || ' ' || M.NameLast) Name," +
                                      "M.NameGiven NameGiven " +
                                      "from FieldingOf F " +
                                      "LEFT OUTER JOIN Master M on M.PlayerID = F.PlayerID " +
                                      "WHERE F.ROWID > @rowid " +
                                      "ORDER BY F.YearID, F.PlayerID;";

                command.Parameters.Add("rowid", System.Data.DbType.Int64);

                //For these command, batchSize and rowsRequested are ignored.

                //First run a warmup round. For this, we will first run the command with a rowid parameter of 9223372036854775807 
                //This should not result in an result but ensure that our code is JITed

                command.Parameters[0].Value = 9223372036854775807;
                result = ExecuteInternal(connection, command);

                command.Parameters[0].Value = -1;
                result = ExecuteInternal(connection, command);
            }

            Log.Debug("  Done in {0}ms", (long)result.TotalMilliseconds);
            return result;

        }

        private TimeSpan ExecuteInternal(SQLiteConnection connection, SQLiteCommand command)
        {
            Stopwatch swatch = Stopwatch.StartNew();

            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                command.ExecuteNonQuery();
                transaction.Commit();
            }

            swatch.Stop();

            return swatch.Elapsed;
        }


    }
}
