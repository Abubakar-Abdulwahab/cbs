using CentralBillingSystemSchedulingWinService.Loggers.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using System.Linq;
using CentralBillingSystemSchedulingWinService.Loggers.Serilog;
using CentralBillingSystemSchedulingWinService.Schedulers;
using CentralBillingSystemSchedulingWinService.Loggers;

namespace CentralBillingSystemSchedulingWinService
{
    public partial class BillingSchedulerService : ServiceBase
    {
        private bool exceptionOccurred;
        private Timer timeDelay;
        private int count;

        /// <summary>
        /// Service logger
        /// </summary>
        private static ISchedulerServiceLogger ServiceLogger;


        /// <summary>
        /// Scheduler implementation
        /// </summary>
        private static IBillingScheduler Schedulers;


        /// <summary>
        /// Set up schedulers and logger
        /// </summary>
        /// <exception cref="Exception"></exception>
        public BillingSchedulerService()
        {
            InitializeComponent();
            try
            {
                timeDelay = new Timer();
                timeDelay.Elapsed += new ElapsedEventHandler(StillRunning);
                ServiceLogger = SetupLogger(GetLoggerConfig());
                Schedulers = SetupSchedulers(GetSchedulerType());
                Schedulers.InitializeSchedulers();
                Schedulers.StartSchedulers();
            }
            #region catch clauses
            catch (Exception exception)
            {
                exceptionOccurred = true;
                if (ServiceLogger != null)
                {
                    ServiceLogger.Error(string.Format("EXCEPTION SETTING UP SCHEDULER {0} {1}", exception.Message, DateTime.Now.ToLocalTime()));
                }
                else
                {
                    FallBackLogger(exception.Message, exception);
                }
                throw exception;
            } 
            #endregion
        }


        /// <summary>
        /// Set up scheduler
        /// </summary>
        /// <param name="schedulerType"></param>
        /// <returns>IBillingScheduler</returns>
        protected IBillingScheduler SetupSchedulers(string schedulerType)
        {
            try
            {
                return new ParkwaySchedulerInterface(schedulerType);
            }
            catch (Exception exception)
            {
                exceptionOccurred = true;
                ServiceLogger.Error("Exception occured when setting up schedulers " + exception.Message);
                throw new Exception("Exception occured when setting up schedulers " + exception.Message);
            }
        }


        /// <summary>
        /// Get logger type from config
        /// <para>Returns a loggeritem object or throws an exception if no logger item was found</para>
        /// </summary>
        /// <returns>LoggerItem</returns>
        /// <exception cref="Exception"></exception>
        private LoggerItem GetLoggerConfig() => (ConfigurationManager.GetSection("LoggerCollection") as List<LoggerItem>).Where(lgr => lgr.Enabled).FirstOrDefault();


        /// <summary>
        /// Get the name of the scheduler you would like to use from config file
        /// <para>E.g Quartz, or Hangfire</para>
        /// </summary>
        /// <returns>string</returns>
        /// <exception cref="Exception">If no scheduler is found</exception>
        private string GetSchedulerType()
        {
            var scheduleProviders = ConfigurationManager.GetSection("ScheduleImplementation") as NameValueCollection;
            NameValueCollection properties = new NameValueCollection { };
            if (scheduleProviders.Count == 0)
            {
                throw new Exception("SCHEDULER CONFIG NOT FOUND " + DateTime.Now.ToLocalTime());
            }
            else
            {
                foreach (var key in scheduleProviders.AllKeys)
                {
                    if (scheduleProviders[key].Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return key;
                    }
                }
            }
            throw new Exception("NO SCHEDULER IMPLEMENTATION FOUND " + DateTime.Now.ToLocalTime());
        }

        /// <summary>
        /// Set up the logger instance
        /// <para>If no logger is found null is returned</para>
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        protected ISchedulerServiceLogger SetupLogger(LoggerItem logger)
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
        /// Start the service
        /// </summary>
        /// <param name="args">string{}</param>
        protected override void OnStart(string[] args)
        {
            if (exceptionOccurred) { OnStop(); }
            ServiceLogger.Error("SERVICE HAS STARTED " + DateTime.Now.ToLocalTime());
            timeDelay.Enabled = true;
        }

        /// <summary>
        /// Is the service still alive and healthy
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void StillRunning(object sender, ElapsedEventArgs e)
        {
            //if (count == 100)
            //{
            //    count++;
            //    ServiceLogger = SetupLogger(GetLoggerConfig());
            //    Schedulers = SetupSchedulers(GetSchedulerType());
            //    Schedulers.InitializeSchedulers();
            //    Schedulers.StartSchedulers();
            //    Schedulers.HelloJob();
            //}
            //count++;
           //ServiceLogger.Error(string.Format("SCHEDULER STILL RUNNING. TIME : {0} SCHEDULER NAME : {1}", DateTime.Now.ToLocalTime(), Schedulers.GetSchedulerNames()));
        }

        /// <summary>
        /// On service stop
        /// </summary>
        protected override void OnStop()
        {
            if (exceptionOccurred)
            {
                if (ServiceLogger != null)
                {
                    ServiceLogger.Error("AN EXCEPTION OCCURRED, CBS BILLING SCHEDULER SERVICE HAS BEEN STOPPED " + DateTime.Now.ToLocalTime());
                }
                else
                {
                    FallBackLogger("AN EXCEPTION OCCURRED, CBS BILLING SCHEDULER SERVICE HAS BEEN STOPPED " + DateTime.Now.ToLocalTime());
                }
            }
            else
            {
               // ServiceLogger.Error("CBS BILLING SCHEDULER SERVICE HAS BEEN STOPPED " + DateTime.Now.ToLocalTime());
            }
            timeDelay.Enabled = false;
        }

        /// <summary>
        /// Log something
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private void FallBackLogger(string message, Exception exception = null)
        {
            FileStream fs = new FileStream(@"c:\\FallBackCentralBillingLoggerLogs.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            message += exception == null ? "" : exception.Message;
            sw.WriteLine(message);
            sw.Flush();
            sw.Close();
        }
    }
}
