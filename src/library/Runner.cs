using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteDBOptions;
using Bytes2you.Validation;
using NLog;
using System.Data.SQLite;
using System.Diagnostics;

namespace SQLitePragmaPerf
{

    public class RunnerResult
    {
        public string NameDBOption;

        public long GenerateDataRuntime;
        public long UseDataRuntime;

        public long TotalRuntime
        {
            get
            {
                return GenerateDataRuntime + UseDataRuntime;
            }
        }
    }


    /// <summary>
    /// Main class of SQLitePragmaPerf, this executes a test run
    /// </summary>
    public class Runner
    {
        private Runner()
        {
            throw new NotImplementedException();
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        string _path;
        List<DBOptions> _listDBOptions;
        List<SQLCommandBase> _listCommands;

        /// <summary>
        /// Create a new runner class that will execute listofCommands against each entry in listDBOptions
        /// </summary>
        /// <param name="listOfDBOptions">One or more DBOptions objects</param>
        /// <param name="listOfCommands">Commands to be executed against each DBOption object</param>
        public Runner(string testPath, List<DBOptions> listDBOptions, List<SQLCommandBase> listCommands)
        {
            Guard.WhenArgument(testPath, "testPath").IsNullOrEmpty().Throw();
            Guard.WhenArgument(listDBOptions, "listDBOptions").IsNull().Throw();
            Guard.WhenArgument(listCommands, "listCommands").IsNull().Throw();

            _path = testPath;
            _listDBOptions = listDBOptions;
            _listCommands = listCommands;

            //Check if we have a high perceision timer for StopWatch or else we can't measure anything
            if (!Stopwatch.IsHighResolution)
            {
                throw new NotSupportedException("This system does not suppoert a high resolution timer for the StopWatch class");
            }

        }

        /// <summary>
        /// Starts running all tests.
        /// </summary>
        /// <param name="totalRowsKilo">Number of rows that should be generated, in kilo (1 = 1000)</param>
        /// <param name="batchSizeKilo">Number of rows that should be executed in one batch, in kilo (1 = 1000)</param>
        public void Run(int totalRowsKilo, int batchSizeKilo)
        {
            Log.Info("Starting for {0} databases...", _listDBOptions.Count);

            List<RunnerResult> resultList = new List<RunnerResult>();

            foreach (DBOptions options in _listDBOptions)
            {
                RunnerResult result = new RunnerResult();

                Log.Info("Running DBOptions named {0}...", options.Name);
                result.NameDBOption = options.Name;

                //Create the database using the database handler to make sure everything is fine
                DatabaseHandler dbhandler = new DatabaseHandler(_path);
                Tuple<string, SQLiteConnection> openResult = dbhandler.CreateDatabase(options);
                string fileName = openResult.Item1;
                SQLiteConnection connection = openResult.Item2;

                //Close it again
                DBOptionsConnectionHandler.CloseDatabase(connection);

                //Real checks start here. 
                Log.Info("Running commands for database {0}...", fileName);


                //Step one: Execute all commands that identiy themselves as "GenerateData"
                result.GenerateDataRuntime = ExecuteCommands(fileName, options, CommandType.GenerateData, batchSizeKilo, totalRowsKilo);

                //Step two: Execute all commands that identify thenselves as "UseData"
                result.UseDataRuntime = ExecuteCommands(fileName, options, CommandType.UseData, batchSizeKilo, totalRowsKilo);
            

                resultList.Add(result);
            }

            //Output all results 
            foreach (RunnerResult curResult in resultList)
            {
                Log.Info("Statistics for DBOptions: {0}", curResult.NameDBOption);
                Log.Info("   Workload 'Generate data' runtime {0}ms", curResult.GenerateDataRuntime);
                Log.Info("   Workload 'Use data' runtime {0}ms", curResult.UseDataRuntime);
            }

        }


        private long ExecuteCommands(string fileName, DBOptions options, SQLitePragmaPerf.CommandType commandType, int batchSizeKilo, int totalRowsKilo)
        {
            SQLiteConnection connection = DBOptionsConnectionHandler.OpenDatabase(fileName, options);

            TimeSpan tsTotal = new TimeSpan(0);
            foreach (SQLCommandBase command in _listCommands)
            {
                if (command.CommandType == commandType) // SQLitePragmaPerf.CommandType.UseData)
                {
                    command.Initialize(connection);
                    TimeSpan ts = command.ExecuteKilo(connection, batchSizeKilo, totalRowsKilo);

                    tsTotal += ts;
                }
            }

            //We close the database to make sure everything is back to default and for example the cache is empty
            DBOptionsConnectionHandler.CloseDatabase(connection);

            return (long)tsTotal.TotalMilliseconds;
        }



    }
}
