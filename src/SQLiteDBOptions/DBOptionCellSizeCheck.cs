using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Cell Size Check will cause SQLite to check pages for errors when data is read from disk
    /// http://www.sqlite.org/pragma.html#pragma_cell_size_check
    /// </summary>
    public class DBOptionCellSizeCheck : DBOptionBasePragma<bool>
    {
        public DBOptionCellSizeCheck() : base(optionName: "Cell Size Check",
                                             setPragmaTemplateSQL: "PRAGMA cell_size_check ={0};",
                                             retrieveActiveValueSQL: "PRAGMA cell_size_check ;",
                                             isPersistent: false)
        {

        }


        protected override bool ConvertFromSQLite(string retrievedValue)
        {
            if (retrievedValue == "1")
            {
                return true;
            }
            else
            {
                if (retrievedValue == "0")
                {
                    return false;
                }
                else
                {
                    throw new NotSupportedException(string.Format("Unsupported cell_size_check value {0}", retrievedValue));
                }
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
                return "Deactivated";
            }
        }

        protected override string ConvertToPragmaValue(bool value)
        {
            if (value == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }


    }
}
