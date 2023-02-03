namespace Parkway.Scheduler.Interface.Loggers
{
    /// <summary>
    /// AvailableLogger section items
    /// </summary>
    public class LoggerItem
    {

        /// <summary>
        /// Name of the logging lib
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Is the logger enabled to run
        /// </summary>
        internal bool Enabled { get; set; }

        /// <summary>
        /// Path you want the log to be stored at
        /// </summary>
        internal string LogPath { get; set; }
    }
}