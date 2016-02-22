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
    public class SQLCommandTableSimple : SQLCommandBaseTableFill
    {

        protected override void PrepareDatabase(SQLiteConnection connection)
        {
            ExecuteSQL(connection, "CREATE TABLE simple_name_table (name TEXT);");
        }

        protected override void PrepareCommand(SQLiteCommand command)
        {
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "INSERT INTO [simple_name_table] VALUES(@name);";
            command.Parameters.Add("name", System.Data.DbType.String);
        }

        protected override void FillParameters(SQLiteCommand command)
        {
            command.Parameters[0].Value = "Joe";
        }

    }
}
