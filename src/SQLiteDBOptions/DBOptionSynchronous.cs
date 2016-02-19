using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace SQLiteDBOptions
{
    public enum SynchronousMode
    {
        Off = 0, //Do not wait for the OS to complete the disk I/O at all, continue directly (FAST but dangerous)
        Normal = 1, //Only wait for the OS to complete disk I/O a the most critical moments, else simply continue (Good tradeoff)
        Full = 2 //Always wait for the OS to complete disk I/O (Very reliable)
    }

    /// <summary>
    /// This controls the Synchronous Mode of the current database (see enum description above)
    /// http://www.sqlite.org/pragma.html#pragma_synchronous
    /// </summary>
    public class DBOptionSynchronous : DBOptionBaseConnectionStringParameter<SynchronousMode>
    {
        public DBOptionSynchronous() : base(optionName: "Synchronous",
                                            connectionStringParameterTemplate: "Synchronous={0};",
                                            retrieveActiveValueSQL: "PRAGMA main.synchronous;",
                                            isPersistent: false)
        {

        }

        protected override string ConvertToConnectionStringParameterValue(SynchronousMode value)
        {
            if (value == SynchronousMode.Off)
            {
                return "Off";
            }
            else
            {
                if (value == SynchronousMode.Full)
                {
                    return "Full";
                }
                else
                {
                    return "Normal";
                }
            }
        }

        protected override SynchronousMode ConvertFromSQLite(string retrievedValue)
        {
            switch (retrievedValue.ToUpper())
            {
                case "OFF":
                    return SynchronousMode.Off;
                case "0":
                    return SynchronousMode.Off;
                case "NORMAL":
                    return SynchronousMode.Normal;
                case "1":
                    return SynchronousMode.Normal;
                case "FULL":
                    return SynchronousMode.Full;
                case "2":
                    return SynchronousMode.Full;
                default:
                    throw new NotSupportedException(string.Format("Value {0} for synchronous pragma is unsupported", retrievedValue));
            }
        }

        protected override string ConvertToDisplayString(SynchronousMode value)
        {
            if (value == SynchronousMode.Off)
            {
                return "Disabled (Do not wait for I/O to be written to disk)";
            }
            else
            {
                if (value == SynchronousMode.Full)
                {
                    return "Full (Always wait for I/O to be written to disk)";
                }
                else
                {
                    return "Normal (Only wait for critical I/Os to be written to disk)";
                }
            }
        }

    }


}
