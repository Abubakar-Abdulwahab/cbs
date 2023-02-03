using System.Configuration;

namespace CentralBillingSystemSchedulingWinService.Loggers
{
    /// <summary>
    /// AvailableLogger section items
    /// </summary>
    public class LoggerItem
    {
        /// <summary>
        /// Name of the logging lib
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the logger enabled to run
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Path you want the log to be stored at
        /// </summary>
        public string LogPath { get; set; }
    }
}