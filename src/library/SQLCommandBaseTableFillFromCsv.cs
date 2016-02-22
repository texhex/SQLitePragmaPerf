using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace SQLitePragmaPerf
{
    public abstract class SQLCommandBaseTableFillFromCsv : SQLCommandBaseTableFill
    {

        protected SQLCommandBaseTableFillFromCsv(string fileNameCSV) : base()
        {
            Log.Debug("Reading CSV file {0}", fileNameCSV);

            using (CsvReader csv = new CsvReader(new StreamReader(@"csv\" + fileNameCSV), true))
            {
                csv.SkipEmptyLines = true;

                int fieldCount = csv.FieldCount;

                while (csv.ReadNextRecord())
                {
                    
                    string[] rowData = new string[fieldCount];
                    for (int i = 0; i < fieldCount; i++)
                    {
                        rowData[i] = csv[i];
                    }

                    ProcessLineFromCSV(rowData);

                }

            }
            Log.Debug("   Done");
        }


   
        /// <summary>
        /// Will be called for each line found in the given CSV file
        /// </summary>
        /// <param name="values">String array containing the values of the current line</param>
        protected abstract void ProcessLineFromCSV(string[] rowData);


        /// <summary>
        /// Converts a string value to int32
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="replaceWithNull">If the string is null or empty, replace with NULL or Zeor (0)?</param>
        /// <returns></returns>
        protected int? ConvertToInt32(string value, bool replaceWithNull)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (replaceWithNull)
                {
                    return null;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        protected string ConvertToString(string value, bool replaceWithNull)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (replaceWithNull)
                {
                    return null;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return value;
            }
        }


    }
}
