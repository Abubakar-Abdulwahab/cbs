using Parkway.CentralBillingScheduler.DAO;
using Parkway.Scheduler.Interface.Loggers;
using Parkway.Scheduler.Interface.Loggers.Contracts;
using Parkway.Scheduler.Interface.Loggers.Serilog;
using Parkway.Scheduler.Interface.Schedulers;
using Parkway.Scheduler.Interface.Schedulers.Contracts;
using Parkway.Scheduler.Interface.Schedulers.Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface
{
    public class SchedulerInterface
    {
        private static ISchedulerLogger Logger;
        public static ISchedulerLogger SeriLogger;
        private static IJobSchedulerInterface SchedulerInterfaces;
        private static IDatabaseSessionManager SessionManagers;
        

        public SchedulerInterface(string schedulerType = "Quartz")
        {
            Logger = SetupSchedulerLogger(GetLoggerConfig());
            SeriLogger = Logger;
            SchedulerInterfaces = SetupSchedulers(Logger, schedulerType, GetSchedulerCollection());
            SessionManagers = StartSessionManagers();
        }

        private IDatabaseSessionManager StartSessionManagers() => new DatabaseSessionManager();
       

        public static ISchedulerLogger GetLoggerInstance() =>  Logger;


        /// <summary>
        /// Get Session manager instance
        /// </summary>
        /// <returns></returns>
        public static IDatabaseSessionManager GetSessionManagerInstance() => SessionManagers;


        /// <summary>
        /// Get the active scheduler item
        /// </summary>
        /// <param name="scheduleName">Name of the scheduler</param>
        /// <returns>SchedulerItem</returns>
        public static SchedulerItem GetScheduleItem(string scheduleName) => SchedulerInterfaces.GetActiveSchedulerRecord(scheduleName);


        /// <summary>
        /// Set up schedulers
        /// </summary>
        /// <param name="logger">ISchedulerLogger</param>
        /// <param name="schedulerType">string</param>
        /// <param name="activeSchedulers">List{SchedulerItem}</param>
        /// <returns>IJobSchedulerInterface</returns>
        private IJobSchedulerInterface SetupSchedulers(ISchedulerLogger logger, string schedulerType, List<SchedulerItem> activeSchedulers)
        {
            switch (schedulerType)
            {
                case "Quartz":
                    return new QuartzImplementation(logger, activeSchedulers);
                default:
                    throw new Exception(string.Format("NO SCHEDULER WITH THE KEY VALUE {0} WAS FOUND. {1}", schedulerType, DateTime.Now.ToLocalTime()));
            }
        }


        /// <summary>
        /// Get active scheduler items
        /// </summary>
        /// <returns>List{SchedulerItem}</returns>
        private List<SchedulerItem> GetSchedulerCollection() => (ConfigurationManager.GetSection("SchedulerCollection") as List<SchedulerItem>).Where(s => s.Active).ToList();


        /// <summary>
        /// Return a reference to a list of schedulers
        /// </summary>
        /// <returns>IJobSchedulerInterface</returns>
        public IJobSchedulerInterface GetScheduler() => SchedulerInterfaces;
       

        /// <summary>
        /// Set up the logger instance
        /// <para>If no logger is found null is returned</para>
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        protected ISchedulerLogger SetupSchedulerLogger(LoggerItem logger)
        {
            try
            {
                switch (logger.Name)
                {
                    case "Serilog":
                        return new SerilogLogger(logger.LogPath);
                    default:
                        return null;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("NO LOGGER FOUND {0} {1}", DateTime.Now.ToLocalTime(), exception.Message));
            }
        }


        /// <summary>
        /// Get logger type from config
        /// <para>Returns a loggeritem object or null. Null if no logger item was found</para>
        /// </summary>
        /// <returns>LoggerItem | null</returns>
        private LoggerItem GetLoggerConfig() => (ConfigurationManager.GetSection("SchedulerLoggerCollection") as List<LoggerItem>).Where(lgr => lgr.Enabled).FirstOrDefault();

    }
}
