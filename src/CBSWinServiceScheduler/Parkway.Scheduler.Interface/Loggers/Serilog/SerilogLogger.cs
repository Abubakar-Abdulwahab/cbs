using Parkway.Scheduler.Interface.Loggers.Contracts;
using Serilog;
using System;

namespace Parkway.Scheduler.Interface.Loggers.Serilog
{
    internal class SerilogLogger : ISchedulerLogger
    {
        private static ILogger Logger;

        internal SerilogLogger(string logFilePath, dynamic parameters = null)
        {
            logFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Logs";
            System.IO.Directory.SetCurrentDirectory(logFilePath);

            Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.RollingFile(@"schedulerlog-{Date}.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();
        }


        public void Error(string message)
        {
            Logger.Error(message);
        }


        public void Information(string message)
        {
            Logger.Information(message);
        }
    }
}
