using System;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace Parkway.CBS.ClientServices.Loggers
{
    public class SerilogLoggerImpl : Logger
    {
        private static ILogger Logger;

        public SerilogLoggerImpl()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .WriteTo.File("logs\\myapp-info.log", restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ThreadId [{ThreadId}] Tenant [{Tenant}] {Message:lj}{NewLine}{Exception}", retainedFileCountLimit: null, shared: true)
                .WriteTo.File("logs\\myapp-error.log", restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ThreadId [{ThreadId}] Tenant [{Tenant}] {Message:lj}{NewLine}{Exception}", retainedFileCountLimit: null, shared: true)
                .CreateLogger();
        }


        public void Debug(string message, string tenantName)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Debug(message);
            }
        }

        public void Debug(string message, string tenantName, Exception exception)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Debug(exception,message);
            }
        }

        public void Error(string message, string tenantName)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Error(message);
            }
        }

        public void Error(string message, string tenantName, Exception exception)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Error(exception, message);
            }
        }

        public void Fatal(string message, string tenantName)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Fatal(message);
            }
        }

        public void Fatal(string message, string tenantName, Exception exception)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Fatal(exception, message);
            }
        }

        public void Info(string message, string tenantName, Exception exception)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Information(message);
            }
        }

        public void Info(string message, string tenantName)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Information(message);
            }
        }

        public void Warn(string message, string tenantName)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Warning(message);
            }
        }

        public void Warn(string message, string tenantName, Exception exception)
        {
            using (LogContext.PushProperty("Tenant", tenantName))
            {
                Logger.Warning(message);
            }
        }

    }
}
