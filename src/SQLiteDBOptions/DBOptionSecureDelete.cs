using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Secure Delete option - http://www.sqlite.org/pragma.html#pragma_secure_delete
    /// When secure-delete is on, SQLite overwrites deleted content with zeros. 
    /// </summary>
    public class DBOptionSecureDelete : DBOptionBasePragma<bool>
    {
        public DBOptionSecureDelete() : base(optionName: "Secure Delete",
                                             setPragmaTemplateSQL: "PRAGMA main.secure_delete={0};",
                                             retrieveActiveValueSQL: "PRAGMA main.secure_delete;",
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
                    throw new NotSupportedException(string.Format("Unsupported secure delete option {0}", retrievedValue));
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
