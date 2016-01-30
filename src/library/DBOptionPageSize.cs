using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Page size option. The amount of bytes a single "page" should have (PRAGMA main.page_size) - http://www.sqlite.org/pragma.html#pragma_page_size
    /// </summary>
    public class DBOptionPageSize : DBOptionBaseConnectionStringParameter<int>
    {
        public DBOptionPageSize() : base(optionName: "Page Size",
                                         connectionStringParameterTemplate: "Page Size={0};",
                                         retrieveActiveValueSQL: "PRAGMA main.page_size;",
                                         isPersistent: true) 
        {
            Log.Debug("Created");

        }

        protected override string ConvertToConnectionStringParameterValue(int value)
        {
            Guard.WhenArgument(value, "value").IsLessThan(512).Throw();
            Guard.WhenArgument(value, "value").IsGreaterThan(65536).Throw();

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
             return string.Format("{0} bytes", value);
        }

    }
}
