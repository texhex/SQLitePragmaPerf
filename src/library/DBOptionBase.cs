using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Bytes2you.Validation;
using System.Data.SQLite;

namespace SQLitePragmaPerf
{
    public class DBOptionValue
    {
        private DBOptionValue()
        {
            throw new NotImplementedException();
        }

        public DBOptionValue(string name, string displayValue)
        {
            Name = name;
            DisplayValue = displayValue;
        }

        //Name of the option that has generated this value
        public string Name
        {
            get; private set;
        }

        //The current value of this option, intend for display to users only
        public string DisplayValue
        {
            get; private set;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, DisplayValue);
        }
    }

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
        /// <returns>Tuple: OPTION_NAME and VALUE</returns>
        public abstract DBOptionValue ExportActiveValue(SQLiteConnection connection);


    }
}
