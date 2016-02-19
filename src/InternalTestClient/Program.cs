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

            options.AddRange(OptionSets.VeryFast());


            //SQLCommandSimpleTable cmdTable = new SQLCommandSimpleTable();

            DatabaseHandler dbHandler1 = new DatabaseHandler(@"C:\TEMP");
            //Tuple<string, SQLiteConnection> dbData = dbHandler1.CreateDatabase(OptionSets.MaxReliability());
            Tuple<string, SQLiteConnection> dbData = dbHandler1.CreateDatabase(options);
            //cmdTable.Initialize(dbData.Item2);
            //cmdTable.ExecuteKilo(dbData.Item2, 1, 2);
            log.Debug("-----------------------------------------------------");
            

            //Creates a 20MB database...
            //DatabaseHandler dbHandler2 = new DatabaseHandler(@"C:\TEMP");
            /*
            Tuple<string, SQLiteConnection> dbData2 = dbHandler1.CreateDatabase(OptionSets.VeryFast());
            cmdTable.Initialize(dbData2.Item2);
            cmdTable.ExecuteKilo(dbData2.Item2, 200, 1800);

            Tuple<string, SQLiteConnection> dbData3 = dbHandler1.CreateDatabase(OptionSets.MaxReliability());
            cmdTable.Initialize(dbData3.Item2);
            cmdTable.ExecuteKilo(dbData3.Item2, 200, 1800);
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
