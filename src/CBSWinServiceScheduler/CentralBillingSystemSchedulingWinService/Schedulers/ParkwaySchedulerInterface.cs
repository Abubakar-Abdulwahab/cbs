using Parkway.Scheduler.Interface.Schedulers.Contracts;

namespace CentralBillingSystemSchedulingWinService.Schedulers
{
    internal class ParkwaySchedulerInterface : IBillingScheduler
    {
        /// <summary>
        /// Scheduler instance
        /// </summary>
        private IJobSchedulerInterface Schedulers;
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="schedulerType">Name of the scheduler</param>
        internal ParkwaySchedulerInterface(string schedulerType = "Quartz")
        {
            Schedulers = SetupSchedulerInterfaces(schedulerType);
        }


        /// <summary>
        /// Initialize all schedulers.
        /// </summary>
        /// <exception cref="Exception">If initialization fails</exception>
        public void InitializeSchedulers() => Schedulers.InitializeSchedulers();
        

        /// <summary>
        /// Initialize the scheduler with the given schedulerName
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void InitializeScheduler(string schedulerName) => Schedulers.InitializeScheduler(schedulerName);
        

        /// <summary>
        /// Get scheduler name
        /// </summary>
        /// <returns>string</returns>
        public string GetSchedulerName(string schedulerName) => Schedulers.SchedulerName(schedulerName);


        public string GetSchedulerNames() => Schedulers.SchedulerNames();


        /// <summary>
        /// Start a particular scheduler with the given name
        /// <para>Before you start a scheduler, initialize it first</para>
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void StartScheduler(string schedulerName) => Schedulers.StartScheduler(schedulerName);
        

        /// <summary>
        /// Starts all active quartz schedulers
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartSchedulers() => Schedulers.StartSchedulers();

        
        /// <summary>
        /// instantiate the class or lib responsible for the scheduler set up
        /// </summary>
        /// <param name="name">Name of the scheduler</param>
        /// <returns>IJobSchedulerInterface</returns>
        private IJobSchedulerInterface SetupSchedulerInterfaces(string name) => new Parkway.Scheduler.Interface.SchedulerInterface(name).GetScheduler();

        public void HelloJob() => Schedulers.HelloJob();
        
    }
}
