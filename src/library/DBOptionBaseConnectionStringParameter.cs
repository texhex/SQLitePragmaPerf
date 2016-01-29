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
    /// This class is used as base for settings that are definied as ConnectionString parameters only
    /// </summary>
    public abstract class DBOptionBaseConnectionStringParameter<T> : DBOptionBase<T>
    {
        protected string _connectionStringParameterTemplate = "";

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="optionName">The name of this option</param>
        /// <param name="connectionStringParameterTemplate">A template (Param={0}) to create the connection string parameter for this option</param>
        /// <param name="retrieveActiveValueSQL">SQL command to retrieve the current value for this option from a database</param>
        /// <param name="isPersistent">Option is persistent</param>
        public DBOptionBaseConnectionStringParameter(string optionName, string connectionStringParameterTemplate, string retrieveActiveValueSQL, bool isPersistent) : base(optionName: optionName, isPersistent: isPersistent, retrieveActiveValueSQL: retrieveActiveValueSQL)
        {
            Guard.WhenArgument(connectionStringParameterTemplate, "connectionStringParameterTemplate").IsNullOrWhiteSpace().Throw();


            //Make sure it ends with ";"
            if (connectionStringParameterTemplate.EndsWith(";"))
            {
                _connectionStringParameterTemplate = connectionStringParameterTemplate;
            }
            else
            {
                _connectionStringParameterTemplate = connectionStringParameterTemplate + ";";
            }
        }


        /// <summary>
        /// This will converted the native value to a connection string parameter value (the part behind "=")
        /// </summary>
        /// <returns>The value of the connection string parameter for this option</returns>
        protected abstract string ConvertToConnectionStringParameterValue(T value);


        /// <summary>
        /// Returns a single connection string parameter that can be appened to an ConnectionString to set this option. 
        /// </summary>
        public string ConnectionStringParameter
        {
            get
            {
                if (TargetValueSet)
                {
                    //Translated -> Template: MyParameter={0}, replace {0} with the value from ConvertToConnectionStringParameterValue()
                    string parameterValue = ConvertToConnectionStringParameterValue(TargetValue);
                    return string.Format(_connectionStringParameterTemplate, parameterValue);
                }
                else
                {
                    //Value not set, return empty string
                    return "";
                }
            }
        }





    }
}
