using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralBillingSystemSchedulingWinService
{
    public interface IBillingScheduler
    {
        /// <summary>
        /// Initialize schedulers. This method initializes all schedulers
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
        /// Start all schedulers
        /// </summary>
        void StartSchedulers();

        /// <summary>
        /// Get name of scheduler
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns>string</returns>
        string GetSchedulerName(string schedulerName);

        /// <summary>
        /// Start a particular scheduler with the given name
        /// <para>Before you start a scheduler, initialize it first</para>
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        void StartScheduler(string schedulerName);

        /// <summary>
        /// Get a string with the names of all the schedulers that have been initialized
        /// </summary>
        /// <returns></returns>
        string GetSchedulerNames();
        void HelloJob();
    }
}
