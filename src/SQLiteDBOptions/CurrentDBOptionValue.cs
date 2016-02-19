using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBOptions
{
    /// <summary>
    /// Name of the option and the value of it, indent to be displayed to the user.
    /// </summary>
    //TODO: This name is still ****. 
    public class CurrentDBOptionValue
    {
        private CurrentDBOptionValue()
        {
            throw new NotImplementedException();
        }

        public CurrentDBOptionValue(string name, string displayValue)
        {
            Name = name;
            DisplayValue = displayValue;
        }

        /// <summary>
        /// Name of the option that has generated this value
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// The current value of this option, for display to users only
        /// </summary>
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
