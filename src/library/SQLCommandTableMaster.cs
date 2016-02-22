using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLitePragmaPerf
{

    /// <summary>
    /// Insert the table "Master" into the database
    /// This tables consists of good mix of integer and string fields
    /// </summary>
    public class SQLCommandTableMaster : SQLCommandBaseTableFillFromCsv
    {
        protected class MasterRow
        {
            internal string PlayerID;
            internal int? BirthYear;
            internal int? BirthMonth;
            internal int? BirthDay;
            internal string BirthCountry;
            internal string BirthState;
            internal string BirthCity;
            internal int? DeathYear;
            internal int? DeathMonth;
            internal int? DeathDay;
            internal string DeathCountry;
            internal string DeathState;
            internal string DeathCity;
            internal string NameFirst;
            internal string NameLast;
            internal string NameGiven;
            internal int? Weight;
            internal int? Height;
            internal string Bats;
            internal string Throws;
            internal string Debut;
            internal string FinalGame;
            internal string RetroID;
            internal string BbrefID;
        }

        public SQLCommandTableMaster() : base("Master.csv")
        {
        }


        List<MasterRow> list = new List<MasterRow>();

        protected override void ProcessLineFromCSV(string[] rowData)
        {
            MasterRow row = new MasterRow();

            row.PlayerID = rowData[0];
            row.BirthYear = ConvertToInt32(rowData[1], true);
            row.BirthMonth = ConvertToInt32(rowData[2], true);
            row.BirthDay = ConvertToInt32(rowData[3], true);
            row.BirthCountry = rowData[4];
            row.BirthState = rowData[5];
            row.BirthCity = rowData[6];
            row.DeathYear = ConvertToInt32(rowData[7], true);
            row.DeathMonth = ConvertToInt32(rowData[8], true);
            row.DeathDay = ConvertToInt32(rowData[9], true);
            row.DeathCountry = rowData[10];
            row.DeathState = rowData[11];
            row.DeathCity = rowData[12];
            row.NameFirst = rowData[13];
            row.NameLast = rowData[14];
            row.NameGiven = rowData[15];
            row.Weight = ConvertToInt32(rowData[16], true);
            row.Height = ConvertToInt32(rowData[17], true);
            row.Bats = rowData[18];
            row.Throws = rowData[19];
            row.Debut = rowData[20];
            row.FinalGame = rowData[21];
            row.RetroID = rowData[22];
            row.BbrefID = rowData[23];

            list.Add(row);
        }


        protected override void PrepareDatabase(SQLiteConnection connection)
        {
            ExecuteSQL(connection, "CREATE TABLE master (playerID TEXT, birthYear NUMERIC, birthMonth NUMERIC, birthDay NUMERIC,birthCountry TEXT, birthState TEXT, birthCity TEXT, deathYear NUMERIC, deathMonth NUMERIC, deathDay NUMERIC, deathCountry TEXT, deathState TEXT, deathCity TEXT, nameFirst TEXT, nameLast TEXT, nameGiven TEXT, weight NUMERIC, height NUMERIC, bats TEXT,throws TEXT, debut TEXT, finalGame TEXT, retroID TEXT, bbrefID TEXT);");

        }

        protected override void PrepareCommand(SQLiteCommand command)
        {
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "INSERT INTO [master](playerID,birthYear,birthMonth,birthDay,birthCountry,birthState,birthCity,deathYear,deathMonth,deathDay,deathCountry,deathState,deathCity,nameFirst,nameLast,nameGiven,weight,height,bats,throws,debut,finalGame,retroID,bbrefID)  VALUES(@playerID,@birthYear,@birthMonth,@birthDay,@birthCountry,@birthState,@birthCity,@deathYear,@deathMonth,@deathDay,@deathCountry,@deathState,@deathCity,@nameFirst,@nameLast,@nameGiven,@weight,@height,@bats,@throws,@debut,@finalGame,@retroID,@bbrefID);";

            command.Parameters.Add("playerID", System.Data.DbType.String);
            command.Parameters.Add("birthYear", System.Data.DbType.Int32);
            command.Parameters.Add("birthMonth", System.Data.DbType.Int32);
            command.Parameters.Add("birthDay", System.Data.DbType.Int32);
            command.Parameters.Add("birthCountry", System.Data.DbType.String);
            command.Parameters.Add("birthState", System.Data.DbType.String);
            command.Parameters.Add("birthCity", System.Data.DbType.String);
            command.Parameters.Add("deathYear", System.Data.DbType.Int32);
            command.Parameters.Add("deathMonth", System.Data.DbType.Int32);
            command.Parameters.Add("deathDay", System.Data.DbType.Int32);
            command.Parameters.Add("deathCountry", System.Data.DbType.String);
            command.Parameters.Add("deathState", System.Data.DbType.String);
            command.Parameters.Add("deathCity", System.Data.DbType.String);
            command.Parameters.Add("nameFirst", System.Data.DbType.String);
            command.Parameters.Add("nameLast", System.Data.DbType.String);
            command.Parameters.Add("nameGiven", System.Data.DbType.String);
            command.Parameters.Add("weight", System.Data.DbType.Int32);
            command.Parameters.Add("height", System.Data.DbType.Int32);
            command.Parameters.Add("bats", System.Data.DbType.String);
            command.Parameters.Add("throws", System.Data.DbType.String);
            command.Parameters.Add("debut", System.Data.DbType.String);
            command.Parameters.Add("finalGame", System.Data.DbType.String);
            command.Parameters.Add("retroID", System.Data.DbType.String);
            command.Parameters.Add("bbrefID", System.Data.DbType.String);



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
            command.Parameters[1].Value = list[currentListPosition].BirthYear;
            command.Parameters[2].Value = list[currentListPosition].BirthMonth;
            command.Parameters[3].Value = list[currentListPosition].BirthDay;
            command.Parameters[4].Value = list[currentListPosition].BirthCountry;
            command.Parameters[5].Value = list[currentListPosition].BirthState;
            command.Parameters[6].Value = list[currentListPosition].BirthCity;
            command.Parameters[7].Value = list[currentListPosition].DeathYear;
            command.Parameters[8].Value = list[currentListPosition].DeathMonth;
            command.Parameters[9].Value = list[currentListPosition].DeathDay;
            command.Parameters[10].Value = list[currentListPosition].DeathCountry;
            command.Parameters[11].Value = list[currentListPosition].DeathState;
            command.Parameters[12].Value = list[currentListPosition].DeathCity;
            command.Parameters[13].Value = list[currentListPosition].NameFirst;
            command.Parameters[14].Value = list[currentListPosition].NameLast;
            command.Parameters[15].Value = list[currentListPosition].NameGiven;
            command.Parameters[16].Value = list[currentListPosition].Weight;
            command.Parameters[17].Value = list[currentListPosition].Height;
            command.Parameters[18].Value = list[currentListPosition].Bats;
            command.Parameters[19].Value = list[currentListPosition].Throws;
            command.Parameters[20].Value = list[currentListPosition].Debut;
            command.Parameters[21].Value = list[currentListPosition].FinalGame;
            command.Parameters[22].Value = list[currentListPosition].RetroID;
            command.Parameters[23].Value = list[currentListPosition].BbrefID;



            currentListPosition++;
        }


    }
}
