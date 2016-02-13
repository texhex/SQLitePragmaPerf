using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    public class OptionSets
    {

        /// <summary>
        /// Returns a list of all known DBOptions, all of them unconfigured
        /// </summary>
        /// <returns>List of DBOptions</returns>
        public static List<DBOptionBase> AllKnownOptions()
        {
            List<DBOptionBase> list = new List<DBOptionBase>();

            list.Add(new DBOptionEncoding());
            list.Add(new DBOptionPageSize());
            list.Add(new DBOptionCacheSize());
            list.Add(new DBOptionSecureDelete());
            list.Add(new DBOptionTempStore());
            list.Add(new DBOptionSQLiteVersion());

            return list;
        }

        public static List<DBOptionBase> EmptyList()
        {
            List<DBOptionBase> list = new List<DBOptionBase>();
            return list;
        }

        public static List<DBOptionBase> Testing1()
        {
            List<DBOptionBase> list = new List<DBOptionBase>();

            DBOptionEncoding encoding = new DBOptionEncoding();
            encoding.TargetValue = Encoding.UTF16LE;
            list.Add(encoding);

            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 4096;
            list.Add(pageSize);

            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024;
            list.Add(cacheSize);

            DBOptionSecureDelete secureDelete = new DBOptionSecureDelete();
            secureDelete.TargetValue = true;
            list.Add(secureDelete);

            DBOptionTempStore tempStore = new DBOptionTempStore();
            tempStore.TargetValue = TempStore.Memory;
            list.Add(tempStore);

                        
            return list;
        }
    }

}
