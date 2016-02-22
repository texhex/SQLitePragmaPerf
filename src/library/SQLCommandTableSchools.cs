using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Insert the table "Schools" into the database
    /// This tables consists of five string fields
    /// </summary>
    public class SQLCommandTableSchools : SQLCommandBaseTableFillFromCsv
    {
        protected class SchoolsRow
        {
            internal string SchoolID;
            internal string Name_full;
            internal string City;
            internal string State;
            internal string Country;
        }


        public SQLCommandTableSchools() : base("Schools.csv")
        {
        }


        List<SchoolsRow> list = new List<SchoolsRow>();

        protected override void ProcessLineFromCSV(string[] rowData)
        {
            SchoolsRow row = new SchoolsRow();

            row.SchoolID = rowData[0];
            row.Name_full = rowData[1];
            row.City = rowData[2];
            row.State = rowData[3];
            row.Country = rowData[4];

            list.Add(row);
        }


        protected override void PrepareDatabase(SQLiteConnection connection)
        {
            ExecuteSQL(connection, "CREATE TABLE Schools (schoolID TEXT, name_full TEXT, city TEXT, state TEXT, country TEXT);");
        }

        protected override void PrepareCommand(SQLiteCommand command)
        {
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "INSERT INTO [Schools](schoolID, name_full, city, state, country)  VALUES(@schoolID, @name_full, @city, @state, @country);";

            command.Parameters.Add("schoolID", System.Data.DbType.String);
            command.Parameters.Add("name_full", System.Data.DbType.String);
            command.Parameters.Add("city", System.Data.DbType.String);
            command.Parameters.Add("state", System.Data.DbType.String);
            command.Parameters.Add("country", System.Data.DbType.String);

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

            command.Parameters[0].Value = list[currentListPosition].SchoolID;
            command.Parameters[1].Value = list[currentListPosition].Name_full;
            command.Parameters[2].Value = list[currentListPosition].City;
            command.Parameters[3].Value = list[currentListPosition].State;
            command.Parameters[4].Value = list[currentListPosition].Country;

            currentListPosition++;
        }


    }
}
