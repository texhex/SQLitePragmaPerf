using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLitePragmaPerf
{
    public class DBOptionSets
    {
        //TODO: Fix me!

        public static List<DBOptionBase> AllKnownOptions()
        {
            List<DBOptionBase> list = new List<DBOptionBase>();

            list.Add(new DBOptionEncoding());
            list.Add(new DBOptionPageSize());
            list.Add(new DBOptionCacheSize());
            list.Add(new DBOptionSecureDelete());

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

            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 4096;

            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024;

            DBOptionSecureDelete secureDelete = new DBOptionSecureDelete();
            secureDelete.TargetValue = true;

            list.Add(encoding);
            list.Add(pageSize);
            list.Add(cacheSize);
            list.Add(secureDelete);

            return list;
        }
    }

}
