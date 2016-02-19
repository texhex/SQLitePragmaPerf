using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteDBOptions;

namespace SQLitePragmaPerf
{
    public class OptionSets
    {


        /*
        public static DBOptions DefaultSettings()
        {
            DBOptions list = new DBOptions();
            return list;
        }
        */

        public static DBOptions VeryFast()
        {

            DBOptions list = new DBOptions();

            //UTF-8 only uses one bytes so this should be the fastest option
            DBOptionEncoding encoding = new DBOptionEncoding();
            encoding.TargetValue = SQLiteDBOptions.Encoding.UTF8;
            list.Add(encoding);

            //Page size should be 4k on windows, this alligns with NTFS cluster size
            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 4096;
            list.Add(pageSize);

            //The more cache the better, but we are assuming 10MB to be enough
            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024 * 10;
            list.Add(cacheSize);

            //Temp store is in memory, hence no disk I/O
            DBOptionTempStore tempStore = new DBOptionTempStore();
            tempStore.TargetValue = TempStore.Memory;
            list.Add(tempStore);

            //Synchronous ist set to OFF. This is dangerous, but FAST
            DBOptionSynchronous syncMode = new DBOptionSynchronous();
            syncMode.TargetValue = SynchronousMode.Off;
            list.Add(syncMode);

            //Cell size check helps to detect database corruption but it costs performance as the data is read from disk 
            DBOptionCellSizeCheck cellCheck = new DBOptionCellSizeCheck();
            cellCheck.TargetValue = false;
            list.Add(cellCheck);

            //Locking mode should be EXCLUSIVE as this means we have an exclusive lock on the database and don't need to obtain or release locks between read or write access
            DBOptionExclusiveLocking lockMode = new DBOptionExclusiveLocking();
            lockMode.TargetValue = true;
            list.Add(lockMode);

            //Journal mode should be MEMORY. Dangerous but FAST (OFF would result in no rollback beeing defectiv which is just plain stupid even for a temporary database)
            DBOptionJournalMode journalMode = new DBOptionJournalMode();
            journalMode.TargetValue = JournalMode.Memory;
            list.Add(journalMode);

            //Automatic index can both be good or bad so this is a tricky question. However, it is ON by default so we will also activate it
            DBOptionAutoIndex autoIndex = new DBOptionAutoIndex();
            autoIndex.TargetValue = true;
            list.Add(autoIndex);


            return list;
        }

        public static DBOptions Normal()
        {
            DBOptions list = new DBOptions();

            //UTF-8 is still the most used encoding so we'll stick with it 
            DBOptionEncoding encoding = new DBOptionEncoding();
            encoding.TargetValue = SQLiteDBOptions.Encoding.UTF8;
            list.Add(encoding);

            //Page size should be 4k on windows, this alligns with NTFS
            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 4096;
            list.Add(pageSize);

            //The more cache the better, but we are assuming 10MB to be enough
            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024 * 10;
            list.Add(cacheSize);

            //Temp store is in memory, hence no disk I/O.
            DBOptionTempStore tempStore = new DBOptionTempStore();
            tempStore.TargetValue = TempStore.Memory;
            list.Add(tempStore);

            //Only sync at critical operations
            DBOptionSynchronous syncMode = new DBOptionSynchronous();
            syncMode.TargetValue = SynchronousMode.Normal;
            list.Add(syncMode);

            //Cell size check helps to detect database corruption. Better activate it to prevent corrupted data
            DBOptionCellSizeCheck cellCheck = new DBOptionCellSizeCheck();
            cellCheck.TargetValue = true;
            list.Add(cellCheck);

            //Journal is set to truncate which means the journal is truncate to 0 bytes after beeing used. This is good tradeoff between Delete and Memory
            DBOptionJournalMode journalMode = new DBOptionJournalMode();
            journalMode.TargetValue = JournalMode.Truncate;
            list.Add(journalMode);

            //Automatic index can both be good or bad so this is a tricky question. However, it is ON by default so we will also activate it
            DBOptionAutoIndex autoIndex = new DBOptionAutoIndex();
            autoIndex.TargetValue = true;
            list.Add(autoIndex);



            return list;
        }

        public static DBOptions MaxReliability()
        {
            DBOptions list = new DBOptions();

            //UTF-16 allows us to store anything using two bytes, where UTF-8 sometimes require three bytes
            DBOptionEncoding encoding = new DBOptionEncoding();
            encoding.TargetValue = SQLiteDBOptions.Encoding.UTF16LE;
            list.Add(encoding);

            //Use the same page size as SQL Server
            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 8192;
            list.Add(pageSize);

            //The more cache the better, but we are assuming 10MB to be enough
            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024 * 10;
            list.Add(cacheSize);

            //Temp store is in memory, hence no disk I/O and also we do not have any problems with temp file locks
            DBOptionTempStore tempStore = new DBOptionTempStore();
            tempStore.TargetValue = TempStore.Memory;
            list.Add(tempStore);

            //Always wait Windows to writte our I/Os to the disk
            DBOptionSynchronous syncMode = new DBOptionSynchronous();
            syncMode.TargetValue = SynchronousMode.Full;
            list.Add(syncMode);

            //Cell size check helps to detect database corruption. Better activate it to prevent corrupted data
            DBOptionCellSizeCheck cellCheck = new DBOptionCellSizeCheck();
            cellCheck.TargetValue = true;
            list.Add(cellCheck);

            //Journal is set to WAL Mode which does allow more concurrent read and writer
            DBOptionJournalMode journalMode = new DBOptionJournalMode();
            journalMode.TargetValue = JournalMode.WAL;
            list.Add(journalMode);

            //Automatic index might (in some cases) causes queries to run slower. To keep the database exactly as the author has created it (everybody should be aware that a table without index is slow) we turn it OFF
            DBOptionAutoIndex autoIndex = new DBOptionAutoIndex();
            autoIndex.TargetValue = false;
            list.Add(autoIndex);

            return list;
        }

        public static DBOptions Testing1()
        {
            DBOptions list = new DBOptions();

            DBOptionEncoding encoding = new DBOptionEncoding();
            encoding.TargetValue = SQLiteDBOptions.Encoding.UTF16LE;
            list.Add(encoding);

            DBOptionCacheSize cacheSize = new DBOptionCacheSize();
            cacheSize.TargetValue = -1024;
            list.Add(cacheSize);

            DBOptionSecureDelete secureDelete = new DBOptionSecureDelete();
            secureDelete.TargetValue = true;
            list.Add(secureDelete);



            return list;
        }
    }

}
