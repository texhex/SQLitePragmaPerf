using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{
    public enum TempStore
    {
        Default = 0,
        File = 1,
        Memory = 2
    }

    /// <summary>
    /// Defines where temporary objects are stored: File or memory. 
    /// PRAGMA temp_store; - http://www.sqlite.org/pragma.html#pragma_temp_store
    /// Note: The SQLite macro SQLITE_TEMP_STORE preprocessor symbol can be used to override this setting but I’m not aware of any method to retrieve the value of this symbol. 
    /// </summary>
    public class DBOptionTempStore : DBOptionBasePragma<TempStore>
    {

        public DBOptionTempStore() : base(optionName: "Temp Store",
                                          setPragmaTemplateSQL: "PRAGMA temp_store={0};",
                                          retrieveActiveValueSQL: "PRAGMA temp_store;",
                                          isPersistent: false)
        {
        }



        protected override string ConvertToPragmaValue(TempStore value)
        {
            if (value == TempStore.File)
            {
                return "FILE";
            }
            else
            {
                if (value == TempStore.Memory)
                {
                    return "MEMORY";
                }
                else
                {
                    return "DEFAULT";
                }
            }
        }

        protected override string ConvertToDisplayString(TempStore value)
        {
            if (value == TempStore.File)
            {
                return "File based";
            }
            else
            {
                if (value == TempStore.Memory)
                {
                    return "In Memory";
                }
                else
                {
                    return "Default (Not definied)";
                }
            }
        }


        protected override TempStore ConvertFromSQLite(string retrievedValue)
        {
            switch (retrievedValue.ToUpper())
            {
                case "DEFAULT":
                    return TempStore.Default;
                case "0":
                    return TempStore.Default;
                case "FILE":
                    return TempStore.File;
                case "1":
                    return TempStore.File;
                case "MEMORY":
                    return TempStore.Memory;
                case "2":
                    return TempStore.Memory;
                default:
                    throw new NotSupportedException(string.Format("Value {0} for temp_store is unsupported", retrievedValue));
            }

        }

    }
}
