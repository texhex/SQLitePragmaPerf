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
    /// Interface to note that DBOption is a ConnectionStringParameter type
    /// </summary>
    public interface IDBOptionConnectionStringParameter
    {
        string ApplyToConnectionString(string connectionString);
    }


    /// <summary>
    /// This class is used as base for options that are definid as ConnectionString parameters only
    /// </summary>
    public abstract class DBOptionBaseConnectionStringParameter<T> : DBOptionBaseImplementation<T>, IDBOptionConnectionStringParameter
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="optionName">The name of this option</param>
        /// <param name="connectionStringParameterTemplate">A template (Param={0}) to create the connection string parameter for this option</param>
        /// <param name="retrieveActiveValueSQL">SQL command to retrieve the current value for this option from a database</param>
        /// <param name="isPersistent">Option is persistent</param>
        public DBOptionBaseConnectionStringParameter(string optionName, string connectionStringParameterTemplate, string retrieveActiveValueSQL, bool isPersistent) : base(optionName: optionName, isPersistent: isPersistent, retrieveValueSQL: retrieveActiveValueSQL)
        {
            Guard.WhenArgument(connectionStringParameterTemplate, "connectionStringParameterTemplate").IsNullOrWhiteSpace().Throw();

            _connectionStringParameterTemplate = connectionStringParameterTemplate;

            //Make sure it ends with ";"
            if (!_connectionStringParameterTemplate.EndsWith(";"))
            {
                _connectionStringParameterTemplate += ";";
            }
        }


        protected string _connectionStringParameterTemplate = "";

        /// <summary>
        /// Converts the native value to a connection string parameter value (the part right of the "=")
        /// </summary>
        /// <returns>The value of the connection string parameter for this option</returns>
        protected abstract string ConvertToConnectionStringParameterValue(T value);


        /// <summary>
        /// This applies TargetValue to a given connectionString. Right now it is simply appened
        /// </summary>
        /// <param name="connectionString">The connection string that should be changed</param>
        /// <returns>The new connection string</returns>
        public string ApplyToConnectionString(string connectionString)
        {
            ThrowExceptionIfTargetValueIsNotSet();

            //Translated -> Template: MyParameter={0}, replace {0} with the value from ConvertToConnectionStringParameterValue()
            string parameterValue = ConvertToConnectionStringParameterValue(TargetValue);
            string fullParameter = string.Format(_connectionStringParameterTemplate, parameterValue);

            return connectionString + fullParameter;
        }





    }
}
