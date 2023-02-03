using System;
using log4net;

namespace Parkway.CBS.ClientServices.Loggers
{
    public class Log4Net : Logger
    {
        private static readonly ILog log = LogManager.GetLogger("Parkway.CBS.ClientServices.Implementations");

        public Log4Net()
        {
        }

        public void Debug(string message, string tenantName)
        {
            log.Debug(message);
        }

        public void Debug(string message, string tenantName, Exception exception)
        {
            log4net.LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Debug($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Error(string message, string tenantName)
        {
            log4net.LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Error(message);
        }

        public void Error(string message, string tenantName, Exception exception)
        {
            log4net.LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Error($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Fatal(string message, string tenantName)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Fatal(message);
        }

        public void Fatal(string message, string tenantName, Exception exception)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Fatal($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Info(string message, string tenantName, Exception exception)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Info($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

        public void Info(string message, string tenantName)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Info(message);
        }

        public void Warn(string message, string tenantName)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Warn(message);
        }

        public void Warn(string message, string tenantName, Exception exception)
        {
            LogicalThreadContext.Properties["Tenant"] = tenantName;
            log.Warn($"{message}  {exception.Message + exception.StackTrace + exception.InnerException}");
        }

    }
}
