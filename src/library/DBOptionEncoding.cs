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

    //https://www.sqlite.org/pragma.html#pragma_encoding
    public class DBOptionEncoding
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        Encoding _encoding;

        public DBOptionEncoding(Encoding encoding)
        {
            logger.Debug("Encoding set to {0}", encoding);
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
