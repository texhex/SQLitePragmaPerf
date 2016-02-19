using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{

    /// <summary>
    /// This controls if the datbase is execlusivly locked (TRUE) or not (FALSE - SQLite default)
    /// http://www.sqlite.org/pragma.html#pragma_locking_mode
    /// </summary>
    public class DBOptionExclusiveLocking : DBOptionBasePragma<bool>
    {

        public DBOptionExclusiveLocking() : base(optionName: "Exclusive Locking",
                                            setPragmaTemplateSQL: "PRAGMA main.locking_mode={0};",
                                            retrieveActiveValueSQL: "PRAGMA main.locking_mode;",
                                            isPersistent: false)
        {
            
        }

        protected override string ConvertToPragmaValue(bool value)
        {
            if (value == true)
            {
                return "EXCLUSIVE";
            }
            else
            {
                return "NORMAL";
            }
        }

        protected override bool ConvertFromSQLite(string retrievedValue)
        {
            switch (retrievedValue.ToUpper())
            {
                case "EXCLUSIVE":
                    return true;
                case "NORMAL":
                    return false;
                default:
                    throw new NotSupportedException(string.Format("Value {0} for locking_mode pragma is unsupported", retrievedValue));
            }
        }

        protected override string ConvertToDisplayString(bool value)
        {
            if (value == true)
            {
                return "Activated";
            }
            else
            {
                return "Off";
            }
        }
    }

}
