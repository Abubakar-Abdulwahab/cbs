using System;


namespace Parkway.CBS.ClientServices
{
    public interface Logger
    {
        void Debug(string message, string tenantName);

        void Debug(string message, string tenantName, Exception exception);

        void Error(string message, string tenantName);

        void Error(string message, string tenantName, Exception exception);

        void Fatal(string message, string tenantName);

        void Fatal(string message, string tenantName, Exception exception);

        void Info(string message, string tenantName, Exception exception);

        void Info(string message, string tenantName);

        void Warn(string message, string tenantName);

        void Warn(string message, string tenantName, Exception exception);
    }
}
