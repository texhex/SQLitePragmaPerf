using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{
    public enum JournalMode
    {
        Delete, //The rollback journal is deleted at the conclusion of each transaction.
        Truncate, //Commits transactions by truncating the rollback journal to zero-length instead of deleting it.        
        Persist, //The header of the journal is overwritten with zeros. 
        Memory, //Stores the rollback journal in RAM. This saves disk I/O but at the expense of database safety and integrity
        WAL //Uses a write-ahead log instead of a rollback journal to implement transactions

        //Note: SQLite also supports OFF but we ignore it (The ROLLBACK command no longer works; it behaves in an undefined way.)
    }

    /// <summary>
    /// This controls which journal option is used (see enum above)
    /// http://www.sqlite.org/pragma.html#pragma_journal_mode
    /// </summary>
    public class DBOptionJournalMode : DBOptionBaseConnectionStringParameter<JournalMode>
    {
        public DBOptionJournalMode() : base(optionName: "Journal mode",
                                            connectionStringParameterTemplate: "Journal Mode={0};",
                                            retrieveActiveValueSQL: "PRAGMA main.journal_mode;",
                                            isPersistent: false)
        {
            
        }

        protected override string ConvertToConnectionStringParameterValue(JournalMode value)
        {
            switch (value)
            {
                case JournalMode.Delete:
                    return "Delete";

                case JournalMode.Memory:
                    return "Memory";

                case JournalMode.Persist:
                    return "Persist";

                case JournalMode.Truncate:
                    return "Truncate";

                case JournalMode.WAL:
                    return "WAL";

                default:
                    throw new NotSupportedException(string.Format("Value {0} for journal mode is unsupported", value.ToString()));

            }

        }

        protected override JournalMode ConvertFromSQLite(string retrievedValue)
        {
            switch (retrievedValue.ToUpper())
            {
                case "DELETE":
                    return JournalMode.Delete;

                case "MEMORY":
                    return JournalMode.Memory;

                case "PERSIST":
                    return JournalMode.Persist;

                case "TRUNCATE":
                    return JournalMode.Truncate;

                case "WAL":
                    return JournalMode.WAL;

                 default:
                    throw new NotSupportedException(string.Format("Value {0} for journal mode is unsupported", retrievedValue));
            }
        }

        protected override string ConvertToDisplayString(JournalMode value)
        {
            return ConvertToConnectionStringParameterValue(value);
        }

    }

}
