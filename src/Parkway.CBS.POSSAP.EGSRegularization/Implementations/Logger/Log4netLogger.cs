using log4net;
using System;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Logger
{
    public class Log4netLogger : Logger.Contracts.ILogger
    {
        private static readonly ILog log = LogManager.GetLogger("Parkway.CBS.POSSAP.EGSRegularization.Implementations");
        
        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            log.Debug($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            log.Error($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            log.Fatal($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Info(string message, Exception exception)
        {
            log.Info($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            log.Warn($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }
    }
}
