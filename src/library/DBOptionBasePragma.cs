using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;
using System.Data.SQLite;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Interface to note that a DBOption is PRAGMA option
    /// </summary>
    public interface IDBOptionPragma
    {
        void ApplyToDatabase(SQLiteConnection connection);
    }


    /// <summary>
    /// This is used as the base for options that can only be defined by a PRAGMA call (no support for connection string)
    /// </summary>
    public abstract class DBOptionBasePragma<T> : DBOptionBaseImplementation<T>, IDBOptionPragma
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="optionName">Name of the option</param>
        /// <param name="setPragmaTemplateSQL">SQL statement template using to define the option</param>
        /// <param name="retrieveActiveValueSQL">SQL statement (no template) to retrieve the current value of this option</param>
        /// <param name="isPersistent">Is the option persistent or not</param>
        public DBOptionBasePragma(string optionName, string setPragmaTemplateSQL, string retrieveActiveValueSQL, bool isPersistent) : base(optionName: optionName, isPersistent: isPersistent, retrieveValueSQL: retrieveActiveValueSQL)
        {
            Guard.WhenArgument(setPragmaTemplateSQL, "setPragmaTemplateSQL").IsNullOrWhiteSpace().Throw();

            _setPragmaTemplateSQL = setPragmaTemplateSQL;

            //Make sure it ends with ";"
            if (!_setPragmaTemplateSQL.EndsWith(";"))
            {
                _setPragmaTemplateSQL += ";";
            }
        }

        //The SQL teamplate to activate this option (usually PRAGMA main.Something={0};)
        protected string _setPragmaTemplateSQL = "";


        /// <summary>
        /// Converts the native value to a pragma value (the part right of the "=")
        /// </summary>
        /// <returns>The value of this option as string</returns>
        protected abstract string ConvertToPragmaValue(T value);


        /// <summary>
        /// Applies TargetValue to a database
        /// </summary>
        /// <param name="connection">Open connection that will be changed</param>
        public void ApplyToDatabase(SQLiteConnection connection)
        {
            ThrowExceptionIfTargetValueIsNotSet();

            string changePragmaSQL = string.Format(_setPragmaTemplateSQL, ConvertToPragmaValue(TargetValue));
            //string changePragmaSQL = string.Format("PRAGMA broken={0};", ConvertToPragmaValue(TargetValue));

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = changePragmaSQL;

                //This will always return -1
                command.ExecuteNonQuery();
            }

            //We need to verify if the changes was done correctly
            string activeValueString = ConvertToDisplayString(GetActiveValue(connection));
            string targetValueString = ConvertToDisplayString(TargetValue);

            if (activeValueString != targetValueString)
            {
                throw new InvalidOperationException(string.Format("Failed to update PRAGMA [{0}] - Current value is [{1}] but expected was [{2}]", OptionName, activeValueString, targetValueString));
            }

        }


    }
}
