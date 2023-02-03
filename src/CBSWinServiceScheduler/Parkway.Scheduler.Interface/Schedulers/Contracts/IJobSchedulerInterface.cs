using Parkway.Scheduler.Interface.Schedulers.Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Schedulers.Contracts
{
    public interface IJobSchedulerInterface
    {
        /// <summary>
        /// Initialize all schedulers
        /// </summary>
        /// <exception cref="Exception">If initialization fails</exception>
        void InitializeSchedulers();


        /// <summary>
        /// Initialize the scheduler with the given schedulerName
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        void InitializeScheduler(string schedulerName);


        /// <summary>
        /// Get the active scheduler item for the given scheduler name
        /// </summary>
        /// <param name="scheduleName"></param>
        /// <returns>SchedulerItem</returns>
        SchedulerItem GetActiveSchedulerRecord(string scheduleName);


        /// <summary>
        /// Create a schedule that runs on CRON expression.
        /// <para>The scheduler with the given schedulerName must has been started inorder to use this method.
        /// KeyNotFoundException is thrown if scheduler with the given schedulerName is not found, null or multiple scheduler have been instantiated.</para>
        /// </summary>
        /// <param name="model">CronSchedule</param>
        /// <exception cref="KeyNotFoundException">If scheduler with the given name has not been started</exception>
        /// <exception cref="Exception"></exception>
        void SetupANewCronSchedule(string schedulerName, CronSchedule model, bool isPaused = false);

        /// <summary>
        /// Get the name of this scheduler
        /// </summary>
        /// <param name="schedulerName">Name of the schedule</param>
        /// <returns>string</returns>
        string SchedulerName(string schedulerName);

        /// <summary>
        /// Get a string with the name of all the schedulers that have been initialized
        /// </summary>
        /// <returns></returns>
        string SchedulerNames();

        /// <summary>
        /// Start quartz schedulers
        /// </summary>
        /// <exception cref="Exception"></exception>
        void StartSchedulers();

        /// <summary>
        /// Start a particular scheduler with the given name
        /// <para>Before you start a scheduler, initialize it first</para>
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        void StartScheduler(string schedulerName);
        void HelloJob();
    }
}
