using CentralBillingSystemSchedulingWinService.Loggers.Contracts;
using Serilog;

namespace CentralBillingSystemSchedulingWinService.Loggers.Serilog
{
    internal class SerilogLogger : ISchedulerServiceLogger
    {

        private static ILogger Logger;

        /// <summary>
        /// Implementation for Serilog
        /// </summary>
        /// <param name="logFilePath">string</param>
        /// <param name="parameters">dynamic</param>
        internal SerilogLogger(string logFilePath, dynamic parameters = null)
        {
            System.IO.Directory.SetCurrentDirectory(logFilePath);
            Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.RollingFile(@"CentralBillingSchedulerLog.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();
        }


        public void Error(string v)
        {
            Logger.Error(v);
        }

        public void S()
        {
            Logger.Error("Error from inside serilog");
        }
    }
}
