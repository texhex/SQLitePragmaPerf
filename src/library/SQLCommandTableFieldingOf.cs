using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SQLitePragmaPerf
{

    /// <summary>
    /// Insert the table "FieldingOF" into the database
    /// This tables consists of one string field and five integers
    /// </summary>
    public class SQLCommandTableFieldingOf : SQLCommandBaseTableFillFromCsv
    {
        protected class FieldingOfRow
        {
            internal string PlayerID;
            internal int? YearID;
            internal int? Stint;
            internal int? Glf;
            internal int? Gcf;
            internal int? Grf;
        }


        public SQLCommandTableFieldingOf() : base("FieldingOF.csv")
        {
        }


        //List<Tuple<string, string, string, string, string, string>> list = new List<Tuple<string, string, string, string, string, string>>();
        List<FieldingOfRow> list = new List<FieldingOfRow>();

        protected override void ProcessLineFromCSV(string[] rowData)
        {
            FieldingOfRow row = new FieldingOfRow();

            row.PlayerID = rowData[0];
            row.YearID = ConvertToInt32(rowData[1], true);
            row.Stint = ConvertToInt32(rowData[2], true);
            row.Glf = ConvertToInt32(rowData[3], true);
            row.Gcf = ConvertToInt32(rowData[4], true);
            row.Grf = ConvertToInt32(rowData[5], true);

            list.Add(row);
        }

   
        protected override void PrepareDatabase(SQLiteConnection connection)
        {
            ExecuteSQL(connection, "CREATE TABLE FieldingOf (playerID TEXT, yearID NUMERIC, stint NUMERIC, Glf NUMERIC, Gcf NUMERIC, Grf NUMERIC);");
        }

        protected override void PrepareCommand(SQLiteCommand command)
        {
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "INSERT INTO [FieldingOf](playerID, yearID, stint, Glf, Gcf, Grf)  VALUES(@playerID, @yearID, @stint, @Glf, @Gcf, @Grf);";

            command.Parameters.Add("playerID", System.Data.DbType.String);
            command.Parameters.Add("yearID", System.Data.DbType.Int32);
            command.Parameters.Add("stint", System.Data.DbType.Int32);
            command.Parameters.Add("Glf", System.Data.DbType.Int32);
            command.Parameters.Add("Gcf", System.Data.DbType.Int32);
            command.Parameters.Add("Grf", System.Data.DbType.Int32);

            currentListPosition = 0;
        }

        //Positiong inside the list were we are currently
        int currentListPosition;

        protected override void FillParameters(SQLiteCommand command)
        {
            if (currentListPosition >= list.Count)
            {
                currentListPosition = 0;
            }

            command.Parameters[0].Value = list[currentListPosition].PlayerID;
            command.Parameters[1].Value = list[currentListPosition].YearID;
            command.Parameters[2].Value = list[currentListPosition].Stint;
            command.Parameters[3].Value = list[currentListPosition].Glf;
            command.Parameters[4].Value = list[currentListPosition].Gcf;
            command.Parameters[5].Value = list[currentListPosition].Grf;

            currentListPosition++;
        }

    }
}
