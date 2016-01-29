using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace SQLitePragmaPerf
{
    public class DBOptionCacheSize : DBOptionBaseConnectionStringParameter<int>
    {
        public DBOptionCacheSize() : base(optionName: "Cache Size",
                                          connectionStringParameterTemplate: "Cache Size={0};",
                                          retrieveActiveValueSQL: "PRAGMA main.cache_size;",
                                          isPersistent: false)
        {

        }

        protected override string ConvertToConnectionStringParameterValue(int value)
        {
            if (value < 0)
            {
                return value.ToString();
            }
            else
            {
                return (value * -1).ToString();
            }

        }

        protected override int ConvertFromSQLite(string retrievedValue)
        {
            Guard.WhenArgument(retrievedValue, "retrievedValue").IsNullOrWhiteSpace().Throw();

            int value = Convert.ToInt32(retrievedValue);

            return value;
        }

        protected override string ConvertToDisplayString(int value)
        {
            if (value < 0)
            {
                return string.Format("{0} KB", value);
            }
            else
            {
                return string.Format("{0} pages", value);
            }
        }

    }
}
