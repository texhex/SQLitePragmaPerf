using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    /// <summary>
    /// Name of the option and the value of it, indent to be displayed to the user.
    /// </summary>
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

        //The current value of this option, for display to users only
        public string DisplayValue
        {
            get; private set;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, DisplayValue);
        }
    }
}
