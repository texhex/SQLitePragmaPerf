using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Data.SQLite;
using Bytes2you.Validation;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// A base class to implement a DBOption (PRAGMA and/or ConnectionString parameter).
    /// T is later on definied by the class to get the right data type for the option in question. 
    /// </summary>
    public abstract class DBOptionBase<T>
    {

        private DBOptionBase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="optionName">The name of this option</param>
        /// <param name="isPersistent">Option is persistent</param>
        /// <param name="retrieveActiveValueSQL">SQL command to retrieve the current value for this option from a database</param>
        public DBOptionBase(string optionName, bool isPersistent, string retrieveActiveValueSQL)
        {
            Log = LogManager.GetLogger(GetType().FullName);

            //Make sure we get something useful
            Guard.WhenArgument(optionName, "optionName").IsNullOrWhiteSpace().Throw();

            //Dot not check retrieveActiveValueSQL as the class might have decided to use their own implementation 

            _optionName = optionName;
            IsPersistent = isPersistent;
            _retrieveActiveValueSQL = retrieveActiveValueSQL;
        }

        //The name of this option 
        protected string _optionName = "";

        //The SQL command text to retrieve the current value of this option
        protected string _retrieveActiveValueSQL = "";

        //This can directly be used by a sub class 
        protected Logger Log
        {
            get; private set;
        }


        //This contains the value this option is set to
        protected T _targetValue;


        /// <summary>
        /// The value this option should have
        /// </summary>
        public virtual T TargetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                _targetValue = value;
                TargetValueSet = true;
            }
        }

        /// <summary>
        /// This will be true when the TargetValue has been set
        /// </summary>
        public bool TargetValueSet
        {
            get; protected set;
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
        public virtual T GetActiveValue(SQLiteConnection connection)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = _retrieveActiveValueSQL;

                //This might return NULL so we need to check it 
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    throw new InvalidCastException(string.Format("Can't verify DBOption, executing [{0}] returned NULL", _retrieveActiveValueSQL));
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
        /// <returns></returns>
        public virtual Tuple<string, string> ExportActiveValue(SQLiteConnection connection)
        {
            T activeValue = GetActiveValue(connection);
            string displayValue = ConvertToDisplayString(activeValue);
            return new Tuple<string, string>(_optionName, displayValue);
        }



        /// <summary>
        /// This function converts the native value of this option to a string value useable by a PRAGMA command
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        //protected abstract string ConvertToSQLite(T targetValue);
        //TODO: Later...



        /// <summary>
        /// Returns if this option is saved to the database (TRUE) or if it needs to by reapplied when opening the database again (FALSE)
        /// </summary>
        public bool IsPersistent
        {
            get; private set;
        }

    }
}
