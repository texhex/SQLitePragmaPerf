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
            string outputLayout = "[${date:format=HH\\:MM\\:ss}] ${logger} >> ${message}";

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

            string fileName = @"C:\TEMP\SQLitePragmaPerf_" + Guid.NewGuid().ToString() + ".sqlite";
            log.Info("Testing database {0}", fileName);

            SQLiteConnection.CreateFile(fileName);
            SQLiteConnection con = new SQLiteConnection(string.Format("Data Source={0};", fileName));
            con.Open();

            
            //DBOptionEncoding encPragma = new DBOptionEncoding(SQLitePragmaPerf.Encoding.UTF16LE);
            DBOptionEncoding encPragma = new DBOptionEncoding();
            //encPragma.TargetValue = SQLitePragmaPerf.Encoding.UTF16LE;
            //encPragma.TargetValue = SQLitePragmaPerf.Encoding.UTF8;

            log.Info("Parameter: {0}", encPragma.ConnectionStringParameter);
            log.Info("Status: {0}", encPragma.ExportActiveValue(con));


            DBOptionCacheSize optCache = new DBOptionCacheSize();
            optCache.TargetValue = -1024;
            log.Info("Parameter: {0}", optCache.ConnectionStringParameter);
            log.Info("Status: {0}", optCache.ExportActiveValue(con));



            //Close and reopen databse
            //If option IsPersistent=TRUE -> Value should be same as before (verfiy run)
            //If option IsPersistent=FALSE -> Reapply



            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();
        }
    }
}
