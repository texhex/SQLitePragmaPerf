using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
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

            DatabaseHandler dbHandler = new DatabaseHandler(@"C:\TEMP");

            //dbHandler.CreateDatabase(DBOptionSets.AllOptionsWithoutTargetValue());
            //string databaseName = "";
            //SQLiteConnection con;

            Tuple<string, SQLiteConnection> dbData; // = new Tuple<string, SQLiteConnection>();
            dbData=dbHandler.CreateDatabase(OptionSets.Testing1());

            SQLCommandSimpleTable cmdTable = new SQLCommandSimpleTable();

            cmdTable.Initialize(dbData.Item2);
            cmdTable.Execute(dbData.Item2, 1000, 9876, false);

            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();
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
