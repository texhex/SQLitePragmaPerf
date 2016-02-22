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


            DBOptions normal = OptionSets.Normal_v10();
            DBOptions fast = OptionSets.VeryFast_v10();

            List<DBOptions> optionList = new List<DBOptions>();
            optionList.Add(normal);
            optionList.Add(fast);


            List<SQLCommandBase> commandList = new List<SQLCommandBase>();
            commandList.Add(new SQLCommandTableSimple());
            commandList.Add(new SQLCommandTableFieldingOf());
            commandList.Add(new SQLCommandTableSchools());
            commandList.Add(new SQLCommandTableMaster());
            commandList.Add(new SQLCommandFieldingOfPlayerNames());


            Runner runner = new Runner(@"C:\TEMP", optionList, commandList);
            //runner.Run(1200, 200) = Generate 1.2 Million rows and execute every 200.000 rows of them in one batch
            runner.Run(100, 50); 


            
            /*
            DBOptions options = new DBOptions();

            DBOptionPageSize pageSize = new DBOptionPageSize();
            pageSize.TargetValue = 1024;
            //options.Add(pageSize);

            //options.AddRange(OptionSets.VeryFast());
            //options.AddRange();
            */
            


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
