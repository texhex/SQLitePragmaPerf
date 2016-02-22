using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using SQLiteDBOptions;
using SQLitePragmaPerf;
using System.Data.SQLite;

namespace InternalTestClient
{
    class Program
    {
        static void ConfigureNLog()
        {
            string outputLayout = "[${time}-${level}] ${logger} >> ${message}";

            LoggingConfiguration config = new LoggingConfiguration();

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = outputLayout;
            config.AddTarget("console", consoleTarget);

            DebuggerTarget debuggerTarget = new DebuggerTarget();
            debuggerTarget.Layout = outputLayout;
            config.AddTarget("debugger", debuggerTarget);


            LoggingRule ruleConsole = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(ruleConsole);

            LoggingRule ruleDebugger = new LoggingRule("*", LogLevel.Trace, debuggerTarget);
            config.LoggingRules.Add(ruleDebugger);


            LogManager.Configuration = config;
        }

        static void Main(string[] args)
        {
            //Set up NLog
            ConfigureNLog();


            Logger log = LogManager.GetCurrentClassLogger();
            log.Info("Starting...");



            


            DBOptions options = new DBOptions();

            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 1024;
            //options.Add(pageSize);

            //options.AddRange(OptionSets.VeryFast());
            //options.AddRange();


            

            DatabaseHandler dbHandler1 = new DatabaseHandler(@"C:\TEMP");
            SQLiteConnection connection = dbHandler1.CreateDatabase(OptionSets.VeryFast());
            //Tuple<string, SQLiteConnection> dbData = dbHandler1.CreateDatabase(OptionSets.MaxReliability());
            //Tuple<string, SQLiteConnection> dbData = dbHandler1.CreateDatabase(options);
            //cmdTable.Initialize(dbData.Item2);
            //cmdTable.ExecuteKilo(dbData.Item2, 1, 2);
            log.Debug("-----------------------------------------------------");

            //SQLCommandSimpleTable cmdTable = new SQLCommandSimpleTable();
            //cmdTable.Initialize(connection);
            //cmdTable.ExecuteKilo(connection, 200, 1800);

            SQLCommandTableFieldingOf cmdFieldingOf = new SQLCommandTableFieldingOf();
            cmdFieldingOf.Initialize(connection);
            cmdFieldingOf.ExecuteKilo(connection, 500, 1800);

            SQLCommandTableSchools cmdSchools = new SQLCommandTableSchools();
            cmdSchools.Initialize(connection);
            cmdSchools.ExecuteKilo(connection, 500, 1800);

            

            /*
            DatabaseHandler dbHandler2 = new DatabaseHandler(@"C:\TEMP");
            SQLiteConnection connection2 = dbHandler2.CreateDatabase(OptionSets.MaxReliability());

            cmdFieldingOf.Initialize(connection2);
            cmdFieldingOf.ExecuteKilo(connection2, 200, 1800);
            */


            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();

            //TODO: Check for high resolution times in StopWatch!
        }


        //Code from: http://stackoverflow.com/a/457708/612954 by JaredPar - http://stackoverflow.com/users/23283/jaredpar
        static bool IsSubclassOfRawGeneric(Type genericBase, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (genericBase == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
