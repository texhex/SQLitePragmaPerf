using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// This class is used as base for settings that are definied as ConnectionString parameters
    /// </summary>
    public abstract class DBOptionBaseConnectionStringParameter<T>:DBOptionBase<T>, IConnectionStringParameter //TODO: Throw away interface and instead use a method directly - then check with typeof (SubClass).IsSubclassOf(typeof (BaseClass)); 
    {
        public override T CurrentOptionValue(SQLiteConnection connection)
        {
            throw new NotImplementedException();
        }

        public override Tuple<string, T> CurrentOptionValueAndName(SQLiteConnection connection)
        {
            throw new NotImplementedException();
        }

        string IConnectionStringParameter.ConnectionStringParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }



    }
}
