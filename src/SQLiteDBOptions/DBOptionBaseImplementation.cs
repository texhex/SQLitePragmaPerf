using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Data.SQLite;
using Bytes2you.Validation;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Implement a base for a DBOption (PRAGMA and/or ConnectionString parameter).
    /// T is later on defined by the class to get the right data type for the option in question. 
    /// </summary>
    public abstract class DBOptionBaseImplementation<T> : DBOptionBase //, IDBOption
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="optionName">The name of this option</param>
        /// <param name="isPersistent">Option is persistent</param>
        /// <param name="retrieveActiveValueSQL">SQL command to retrieve the current value for this option from a database</param>
        public DBOptionBaseImplementation(string optionName, bool isPersistent, string retrieveValueSQL) : base(optionName: optionName, isPersistent: isPersistent)
        {
            //Make sure we get something useful
            Guard.WhenArgument(retrieveValueSQL, "retrieveValueSQL").IsNullOrWhiteSpace().Throw();
            _retrieveActiveValueSQL = retrieveValueSQL;

            //Make sure it ends with ";"
            if (!_retrieveActiveValueSQL.EndsWith(";"))
            {
                _retrieveActiveValueSQL += ";";
            }
        }


        //The SQL command text to retrieve the current value of this option
        protected string _retrieveActiveValueSQL = "";

        //This contains the value this option is set to
        protected T _targetValue;

        /// <summary>
        /// This function is caleld before the TargetValue is set and can be used by a sub class to change the value
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <returns>Value that will then be used as TargetValue</returns>
        protected virtual T VerifyTargetValue(T value)
        {
            return value;
        }


        /// <summary>
        /// The value this option should have
        /// </summary>
        public T TargetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                _targetValue = VerifyTargetValue(value);
                TargetValueSet = true;
            }
        }



        /// <summary>
        /// Returns the current value of TargetValue as string useable for display
        /// </summary>
        public string TargetValueAsDisplayString
        {
            get
            {
                ThrowExceptionIfTargetValueIsNotSet();

                return ConvertToDisplayString(TargetValue);
            }

        }

        /// <summary>
        /// This should be called when the TargetValue is about to be used to make sure it is set to something useful 
        /// </summary>
        protected void ThrowExceptionIfTargetValueIsNotSet()
        {
            if (TargetValueSet == false || TargetValue == null)
            {
                throw new InvalidOperationException("TargetValue is not defined");
            }
        }


        /// <summary>
        /// This function converts the retrieved value (PRAGMA xxx) from SQLite to the native value for this option.
        /// </summary>
        /// <param name="retrievedValue">Retrieved value from SQLite that requires conversion</param>
        /// <returns></returns>
        protected abstract T ConvertFromSQLite(string retrievedValue);

        /// <summary>
        /// Returns a string representation of value used for display
        /// </summary>
        /// <param name="value">Native value</param>
        /// <returns>A string used for display</returns>
        protected abstract string ConvertToDisplayString(T value);



        /// <summary>
        /// Returns the current value of this option for the database used by the given connection
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns>The current option value</returns>
        public T GetActiveValue(SQLiteConnection connection)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = _retrieveActiveValueSQL;

                //This might return NULL so we need to check it 
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    throw new InvalidCastException(string.Format("Can't verify DBOption {1}, executing [{0}] returned NULL", _retrieveActiveValueSQL, OptionName));
                }

                string optionValueString = result.ToString();
                return ConvertFromSQLite(optionValueString);
            }

        }


        /// <summary>
        /// Returns the name of this option and the current active value (not the target value) as string used for displaying it to the user.
        /// The value is retrieved directly from SQLite.
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns>Tuple: OPTION_NAME and VALUE</returns>
        public override CurrentDBOptionValue GetCurrentOptionValue(SQLiteConnection connection)
        {
            T activeValue = GetActiveValue(connection);
            string displayValue = ConvertToDisplayString(activeValue);
            return new CurrentDBOptionValue(OptionName, displayValue);
        }



    }
}
