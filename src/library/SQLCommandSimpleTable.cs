using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Insert a table with only one column "NAME" in it and inserts always the same name "Joe"
    /// </summary>
    public class SQLCommandSimpleTable: SQLCommandBase
    {
        public override void Initialize(SQLiteConnection connection)
        {
            base.Initialize(connection);

            ExecuteSQL(connection, "CREATE TABLE simple_name_table (name TEXT);");
        }

        public override TimeSpan Execute(SQLiteConnection connection, int batchSize, int maxRows, bool warumUp)
        {
            Log.Debug("Starting execute...");

            VerifyExecuteParameters(connection, batchSize, maxRows, warumUp);

            //If it's a testRun, we set both batchSize and maxRows to 1 
            if (warumUp)
            {
                Log.Debug("This is a test run");
                batchSize = 1;
                maxRows = 1;

            }

            //The question is if prepared statements really make that big difference because normally the engine 
            //should detect if the same command is executed over and over again: http://stackoverflow.com/a/21376455/612954
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "INSERT INTO [simple_name_table] VALUES(@name);";
                command.Parameters.Add("name", System.Data.DbType.String);

                //TODO: Need to hanle batchSize!
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    for (int i = 1; i <= maxRows; i++)
                    {
                        command.Parameters[0].Value = "Joe";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

            }

            //If it's a test run, we delete everything from our table again
            if (warumUp)
            {
                //TODO we could also use where where ROWID=9223372036854775807 which should (in any "normal" way) never result in anything beeing deleted
                ExecuteSQL(connection, "DELETE FROM simple_name_table;");
                
            }

            Log.Debug("Done");

            return new TimeSpan(0);
        }
    }
}
