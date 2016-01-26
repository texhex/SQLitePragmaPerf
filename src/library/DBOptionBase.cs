using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// A base class to implement a DBOption (PRAGMA and/or ConnectionString parameter).
    /// T is later on definied by the class to get the right data type for the option in question. 
    /// </summary>
    public abstract class DBOptionBase<T>
    {
        /// <summary>
        /// Returns the current value of this option for the MAIN database used by the given connection
        /// </summary>
        /// <param name="connection">SQLite connection with an open connection to the database</param>
        /// <returns>The current option value</returns>
        public abstract T CurrentOptionValue(SQLiteConnection connection);
        //{            return default(T);        }

        //Returns the name of this option and the current value as string used for displaying it to the user
        public abstract Tuple<string, T> CurrentOptionValueAndName(SQLiteConnection connection);
        //{            return default(Tuple<string,T>);        }
    }
}
