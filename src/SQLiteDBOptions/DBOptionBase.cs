using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Bytes2you.Validation;
using System.Data.SQLite;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Base class for a DBOption
    /// </summary>
    public abstract class DBOptionBase
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
        public DBOptionBase(string optionName, bool isPersistent)
        {
            Log = LogManager.GetLogger(GetType().FullName);

            //Make sure we get something useful
            Guard.WhenArgument(optionName, "optionName").IsNullOrWhiteSpace().Throw();

            OptionName = optionName;
            IsPersistent = isPersistent;
            TargetValueSet = false;
        }

        /// <summary>
        /// The name of this option
        /// </summary>
        public string OptionName
        {
            get; private set;
        }

        //This can directly be used by a sub class 
        protected Logger Log
        {
            get; private set;
        }

        /// <summary>
        /// Returns if this option is saved to the database (TRUE) or if it needs to by reapplied when opening the database again (FALSE)
        /// </summary>
        public bool IsPersistent
        {
            get; private set;
        }

        /// <summary>
        /// This will be true when TargetValue has been defined
        /// </summary>
        public bool TargetValueSet
        {
            get; protected set;
        }

        /// <summary>
        /// Returns the name of this option and the current active value (not the target value) as string used for displaying it to the user.
        /// The value is retrieved directly from SQLite.
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns>CurrentDBOptionValue with the current value</returns>
        public abstract CurrentDBOptionValue GetCurrentOptionValue(SQLiteConnection connection);


    }
}
