using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SQLitePragmaPerf
{
    class BaseClass
    {
        protected BaseClass()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

        protected Logger Log { get; private set; }
    }

    class ExactClass : BaseClass
    {
        public ExactClass() : base() { }

    }
}
