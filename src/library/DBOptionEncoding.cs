using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SQLitePragmaPerf
{
    public enum Encoding
    {
        UTF8,
        UTF16LE
    }

    /// <summary>
    /// This defines the encoding for a database (PRAGMA main.encoding). 
    /// It must be set when a new database is created for the first time and cannot be changed afterwards.
    /// </summary>
    public class DBOptionEncoding : DBOptionBaseConnectionStringParameter<Encoding>
    {

        public DBOptionEncoding() : base(optionName: "Encoding",
                                         connectionStringParameterTemplate: "UseUTF16Encoding={0};",
                                         retrieveActiveValueSQL: "PRAGMA main.encoding;",
                                         isPersistent: true)
        {
            Log.Debug("Created");
        }


        protected override string ConvertToConnectionStringParameterValue(Encoding value)
        {
            if (value == Encoding.UTF16LE)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }

        protected override string ConvertToDisplayString(Encoding value)
        {
            if (value == Encoding.UTF16LE)
            {
                return "UTF-16le (Little-endian 16-bit Unicode)";
            }
            else
            {
                return "UTF-8 (8-bit Unicode)";
            }
        }


        protected override Encoding ConvertFromSQLite(string retrievedValue)
        {
            if (retrievedValue.ToUpper() == "UTF-16LE")
            {
                return Encoding.UTF16LE;
            }
            else
            {
                if (retrievedValue.ToUpper() == "UTF-8")
                {
                    return Encoding.UTF8;
                }
                else
                {
                    throw new NotSupportedException(string.Format("Unsupported encoding {0}", retrievedValue));
                }
            }
        }

    }
}
