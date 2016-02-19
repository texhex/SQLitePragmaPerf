using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{

    /// <summary>
    /// Automatic index option. If this is activate (TRUE), SQLite might create an non-persistent automatic index
    /// http://www.sqlite.org/pragma.html#pragma_automatic_index and http://www.sqlite.org/optoverview.html#autoindex
    /// </summary>
    public class DBOptionAutoIndex : DBOptionBasePragma<bool>
    {
        public DBOptionAutoIndex() : base(optionName: "Automatic Index",
                                          setPragmaTemplateSQL: "PRAGMA automatic_index={0};",
                                          retrieveActiveValueSQL: "PRAGMA automatic_index ;",
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
                    throw new NotSupportedException(string.Format("Unsupported automatic_index option {0}", retrievedValue));
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
                return "1";
            }
            else
            {
                return "0";
            }
        }


    }
}
