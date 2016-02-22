using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    //A base class for SQLCommands that use existing data
    public abstract class SQLCommandBaseUseData:SQLCommandBase
    {
        public SQLCommandBaseUseData()
        {
            CommandType = SQLitePragmaPerf.CommandType.UseData;
        }
    }
}
