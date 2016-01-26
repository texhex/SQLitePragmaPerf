using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// This interface should be implemented when am option can be defined using a ConnectionString parameter.
    /// If an option/pragma can be definied both as a ConnectionString and applied to an existing database using a
    /// PRAGMA statement, the ConnectionString parameter should be used. 
    /// This is easier to implement for the programmers that wants to take advantage of this option in their own programs. 
    /// </summary>
    interface IConnectionStringParameter
    {
        /// <summary>
        /// Returns a string that can be appened to the ConnectionString to activate this option.
        /// </summary>
        string ConnectionStringParameter
        {
            get;
        }

    }
}
