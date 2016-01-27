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

        public DBOptionBase(string optionName, bool isPersistent)
        {
            Log = LogManager.GetLogger(GetType().FullName);

            //Make sure we get something useful
            Guard.WhenArgument(optionName, "optionName").IsNullOrWhiteSpace().Throw();

            _isPersistent = isPersistent;
        }


        //This contains the value this option is set to 
        protected T _targetValue;

        //This will be true when the TargetValue has been set
        protected bool _targetValueSet = false;

        //TODO: Convert this to a property? 
        

        //Stores the name of the option this is used by Export...
        protected string _optionName = "";

        //This can directly be derivated classes to access the logger
        protected Logger Log { get; private set; }

        /// <summary>
        /// Returns the current value of this option for the MAIN database used by the given connection
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns>The current option value</returns>
        public abstract T GetActiveValue(SQLiteConnection connection);
        //{            return default(T);        }

        /// <summary>
        /// Returns the name of this option and the current value as string used for displaying it to the user
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns></returns>
        public abstract Tuple<string, string> ExportActiveValue(SQLiteConnection connection);
        //{            return default(Tuple<string,T>);        }


        protected Nullable<bool> _isPersistent; //One day I will be as cool as the big guys and write "bool?"

        /// <summary>
        /// Returns if this option is saved to the database (TRUE) or not and needs to by reapplied when opening the database again
        /// </summary>
        public bool IsPersistent
        {
            get
            {
                //Make sure this value was definied in the derivated class
                Guard.WhenArgument(_isPersistent, "_isPersistent").IsNull().Throw(); //TODO Guard will always return an argument exception. According to http://stackoverflow.com/a/1903143/612954 it should throw an InvalidOperationException as the null value is already there when the function is called

                return _isPersistent.Value;
            }
        }

    }
}
