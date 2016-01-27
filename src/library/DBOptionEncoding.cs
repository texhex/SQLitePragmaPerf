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
    /// This defines the encoding for a database (PRAGMA main.encoding;). 
    /// It must be set when a new database is created for the first time and cannot be changed afterwards.
    /// </summary>
    public class DBOptionEncoding : DBOptionBaseConnectionStringParameter<Encoding>
    {
        private const string OPTION_NAME = "Encoding";
        private const string CONNECTION_STRING_PARAMETER = "UseUTF16Encoding";

        Encoding _encoding;

        /// <summary>
        /// Creates the option without a target value. It can only be used for retrieving the curnrent value.
        /// </summary>
        public DBOptionEncoding() : base(OPTION_NAME, CONNECTION_STRING_PARAMETER, true)
        {
            Log.Debug("Created without target value");
        }

        /// <summary>
        /// Creates the option with a target value. This option can then be applied to existing databases
        /// </summary>
        /// <param name="encoding">The desired encoding</param>
        public DBOptionEncoding(Encoding encoding) : base(OPTION_NAME, CONNECTION_STRING_PARAMETER, true)
        {
            Log.Debug("Encoding set to {0}", encoding);
            _encoding = encoding;
        }






        public string ConnectionStringParameter
        {
            get
            {
                string template = "UseUTF16Encoding={0};";

                if (_encoding == Encoding.UTF16LE)
                {
                    return string.Format(template, "True");
                }
                else
                {
                    return string.Format(template, "False");
                }
            }
        }

        public override string ToString()
        {
            return ConnectionStringParameter;
        }


    }
}
