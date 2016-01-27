using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// This class is used as base for settings that are definied as ConnectionString parameters
    /// </summary>
    public abstract class DBOptionBaseConnectionStringParameter<T> : DBOptionBase<T>
    {
        protected string _connectionStringParameterName = "";

        public DBOptionBaseConnectionStringParameter(string optionName, string connectionStringParameter, bool isPersistent) : base(optionName, isPersistent)
        {
            Guard.WhenArgument(optionName, "connectionStringParameter").IsNullOrWhiteSpace().Throw();
            _connectionStringParameterName = connectionStringParameter;

        }

        public override T GetActiveValue(SQLiteConnection connection)
        {
            throw new NotImplementedException();
        }

        public override Tuple<string, string> ExportActiveValue(SQLiteConnection connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a single connection string parameter that can be appened to an ConnectionString to set this option. The returned value is ended with ";"
        /// </summary>
        string ConnectionStringParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }



    }
}
