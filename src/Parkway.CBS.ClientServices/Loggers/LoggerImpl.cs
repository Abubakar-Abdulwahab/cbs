using System;

namespace Parkway.CBS.ClientServices.Loggers
{
    public class LoggerImpl
    {
        private static readonly Lazy<Logger> _settings = new Lazy<Logger>(() =>
        {
            return new SerilogLoggerImpl();
        });

        public static Logger GetLogger
        {
            get { return _settings.Value; }
        }

    }
}
