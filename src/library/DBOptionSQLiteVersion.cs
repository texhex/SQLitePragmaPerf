using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    //TODO: This class needs to prevent somebody from setting a value as this value can not be overwritten. This class can only be used for display.

    /// <summary>
    /// Read only DBOption, returns the SQLite version used - https://www.sqlite.org/lang_corefunc.html#sqlite_version
    /// </summary>
    public class DBOptionSQLiteVersion : DBOptionBasePragma<Version>
    {
        public DBOptionSQLiteVersion() : base(optionName: "SQLite Version",
                                              setPragmaTemplateSQL: "PRAGMA main.non_existing_pragma={0};",
                                              retrieveActiveValueSQL: "select sqlite_version();",
                                              isPersistent: true)
        {
            Log.Debug("Created");
        }

        protected override Version VerifyTargetValue(Version value)
        {
            throw new InvalidOperationException("SQLiteVersion can not be changed, this is a read-only option");
        }

        protected override Version ConvertFromSQLite(string retrievedValue)
        {
            return Version.Parse(retrievedValue);
        }

        protected override string ConvertToDisplayString(Version value)
        {
            return value.ToString();
        }

        protected override string ConvertToPragmaValue(Version value)
        {
            throw new InvalidOperationException("SQLiteVersion can not be changed, this is a read-only option");
        }
    }
}
