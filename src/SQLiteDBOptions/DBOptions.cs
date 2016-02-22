using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using NLog;

namespace SQLiteDBOptions
{
    /// <summary>
    /// A collection of DBOptionXXX instances which can be used to retrieve the configured DBOptions as connection string and apply them to a database.
    /// Note that this class is not performance optimized. For best use, use it once to open the database connection and then resuse the connection. 
    /// </summary>
    public class DBOptions : List<DBOptionBase>
    {
        Logger log = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Generates a connection string that includes all configured options that can be changed using the connection string. You still need to call ApplyOptions() after opening the database!
        /// </summary>
        /// <param name="filename">Filename of the database, will be used as Data Source=XXX; parameter</param>
        /// <returns>The connection string to hand over to a SQLiteConnection</returns>
        public string GenerateConnectionString(string filename)
        {
            return GenerateConnectionString(filename, "");
        }

        /// <summary>
        /// Optional name for these options
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Generates a connection string that includes all configured options that can be changed using the connection string. You still need to call ApplyOptions() after opening the database!
        /// </summary>
        /// <param name="fileName">Filename of the database, will be used as Data Source=XXX; parameter</param>
        /// <param name="connectionStringExtra">Additional connection string parameters that will be appended to the generated connection string</param>
        /// <returns>The connection string to hand over to a SQLiteConnection</returns>
        public string GenerateConnectionString(string fileName, string connectionStringExtra)
        {
            //Ensure all options are set only once
            CheckForDuplicates();

            //Start with the datasource
            string connectionString = string.Format("Data Source = {0};", fileName);

            foreach (DBOptionBase option in this)
            {
                if (option is IDBOptionConnectionStringParameter)
                {
                    if (option.TargetValueSet)
                    {
                        IDBOptionConnectionStringParameter connectionStringOption = (IDBOptionConnectionStringParameter)option;
                        connectionString = connectionStringOption.AppendToConnectionString(connectionString);
                    }
                }
            }

            connectionString += connectionStringExtra;

            log.Debug("Generated connection string: {0}", connectionString);
            return connectionString;
        }



        /// <summary>
        /// Applies all non-connectionstring options to the open database. You need to construct the connection string first and use it to open the database
        /// </summary>
        /// <param name="connection"></param>
        public void ApplyOptions(SQLiteConnection connection)
        {
            //Ensure all options are set only once
            CheckForDuplicates();
            
            foreach (DBOptionBase option in this)
            {
                if (option is IDBOptionPragma)
                {
                    if (option.TargetValueSet)
                    {
                        log.Debug("Applying PRAGMA option: {0} ", option.OptionName);

                        IDBOptionPragma pragmaOption = (IDBOptionPragma)option;
                        pragmaOption.ApplyToDatabase(connection);
                    }
                }
            }
        }

        /// <summary>
        /// Checks this object for duplicate options which could cause strange effects when using. If a duplicate is found, an exeception is thrown
        /// </summary>
        public void CheckForDuplicates()
        {
            foreach (DBOptionBase itemOuterLoop in this)
            {
                //now search the inner list for the outer item
                Type itemOuterLoopType = itemOuterLoop.GetType();

                int counter = 0;
                foreach (DBOptionBase itemInnerLoop in this)
                {
                    if (itemInnerLoop.GetType() == itemOuterLoopType)
                    {
                        counter++;
                    }
                }

                //If counter>1, we have an duplicate item
                if (counter > 1)
                {
                    throw new NotSupportedException(string.Format("Only one DBOption of the same type is supported, but [{0}] was used more than once", itemOuterLoopType));
                }

            }

        }


        /// <summary>
        /// Returns a list of all known DBOptions, all of them unconfigured
        /// </summary>
        /// <returns>DBOptions</returns>
        public static DBOptions AllDBOptions()
        {
            DBOptions options = new DBOptions();

            options.Add(new DBOptionEncoding());
            options.Add(new DBOptionPageSize());
            options.Add(new DBOptionJournalMode());
            options.Add(new DBOptionSynchronous());
            options.Add(new DBOptionTempStore());
            options.Add(new DBOptionExclusiveLocking());
            options.Add(new DBOptionCacheSize());
            options.Add(new DBOptionCellSizeCheck());
            options.Add(new DBOptionSecureDelete());
            options.Add(new DBOptionAutoIndex());
            options.Add(new DBOptionSQLiteVersion());

            return options;
        }

        /*
        I leave this here if we every need it again

        public new void Add(DBOptionBase value)
        {
            Type typeNewItem = value.GetType();

            foreach (DBOptionBase current in this)
            {
                if (current.GetType() == typeNewItem)
                {
                    throw new ArgumentException(
                        string.Format(
                             "DBOption {0} can not be added as it is already present", current.GetType().ToString()
                             )
                        );
                }
            }

            base.Add(value);
        }

        
        public new void AddRange(IEnumerable<DBOptionBase> enumerator)
        {
            foreach(DBOptionBase current in enumerator)
            {
                Add(current);
            }

        }

        public new void Insert(Int32 position, DBOptionBase value)
        {
            throw new NotImplementedException();
        }
        */
    }
}
