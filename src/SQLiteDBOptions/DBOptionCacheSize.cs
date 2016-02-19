using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Cache Size (PRAGMA main.cache_size) - http://www.sqlite.org/pragma.html#pragma_cache_size
    /// This value can bei be positive, which means "X pages cache" or negative in which case it means "X kB of cache"
    /// </summary>
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
            return value.ToString();
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
                return string.Format("{0} kB", (value*-1));
            }
            else
            {
                return string.Format("{0} pages", value);
            }
        }

    }
}
