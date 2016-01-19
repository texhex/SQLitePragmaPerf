using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
//using NLog.R
using SQLitePragmaPerf;

namespace InternalTestClient
{
    class Program
    {
        static void ConfigureNLog()
        {
            string outputLayout = "[${date:format=HH\\:MM\\:ss}] ${logger}::${message}";

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


            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting...");


            DBOptionEncoding encPragma = new DBOptionEncoding(SQLitePragmaPerf.Encoding.UTF16LE);

            string sResult = encPragma.ConnectionStringParameter;






            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();
        }
    }
}
