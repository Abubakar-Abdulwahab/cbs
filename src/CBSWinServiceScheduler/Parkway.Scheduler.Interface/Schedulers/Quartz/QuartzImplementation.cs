using Parkway.Scheduler.Interface.CentralBillingSystem;
using Parkway.Scheduler.Interface.Loggers.Contracts;
using Parkway.Scheduler.Interface.Schedulers.Contracts;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;

namespace Parkway.Scheduler.Interface.Schedulers.Quartz
{
    public class QuartzImplementation : BaseScheduler, IJobSchedulerInterface
    {
        /// <summary>
        /// Schedule logger
        /// </summary>
        private ISchedulerLogger Logger;

        /// <summary>
        /// Collection of schedulers
        /// </summary>
        private ConcurrentDictionary<string, IScheduler> QuartzSchedulers;

        /// <summary>
        /// List of active schedulers
        /// </summary>
        private List<SchedulerItem> ActiveSchedulers;


        public QuartzImplementation(ISchedulerLogger logger, List<SchedulerItem> activeSchedulers) : base(logger)
        {
            Logger = logger;
            QuartzSchedulers = new ConcurrentDictionary<string, IScheduler>();
            ActiveSchedulers = activeSchedulers;
        }


        /// <summary>
        /// Initialize all schedulers
        /// </summary>
        /// <exception cref="Exception">If initialization fails</exception>
        public void InitializeSchedulers()
        {
            Logger.Information(string.Format("Initializing all active schedulers"));
            List<SchedulerItem> schedulersNotInitialized = new List<SchedulerItem>();
            if (QuartzSchedulers.Count > 1)
            {
                schedulersNotInitialized = ActiveSchedulers.Where(actvshc => QuartzSchedulers.Any(sch => sch.Key != actvshc.Name)).Select(actvsch => actvsch).ToList();
            }
            else
            {
                schedulersNotInitialized = ActiveSchedulers;
            }

            if (schedulersNotInitialized == null || schedulersNotInitialized.Count <= 0 ) { Logger.Information("No active scheduler found."); return; }

            var initializedSchedulers = InitializeSchedulers(schedulersNotInitialized);
            Logger.Information("Schedulers have been initialized. Adding to the list of initialized schedulers");

            foreach (var item in initializedSchedulers)
            {
                if (QuartzSchedulers.ContainsKey(item.Key)) continue;

                QuartzSchedulers.TryAdd(item.Key, item.Value);
            }
        }


        /// <summary>
        /// Initialize the scheduler with the given schedulerName
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void InitializeScheduler(string schedulerName)
        {
            Logger.Information("Initializing scheduler " + schedulerName);
            SchedulerItem activeSchedulerRecord = GetActiveSchedulerRecord(schedulerName);
            //once we have gotten the active schedule record
            //lets check if the active record has been initialized already
            var quartzScheduler = QuartzSchedulers.Where(sch => sch.Key == schedulerName).Select(sch => sch.Value).SingleOrDefault();

            if (quartzScheduler != null) return;

            Logger.Information("Active scheduler found " + schedulerName + " initializing...");
            var initializedScheduler = InitializeSchedulers(new List<SchedulerItem> { { activeSchedulerRecord } }).ElementAt(0);

            Logger.Information(string.Format("{0} initialization done. Now adding to the list of initialized schedulers.", schedulerName));
            if (QuartzSchedulers.ContainsKey(initializedScheduler.Key)) return;

            QuartzSchedulers.TryAdd(initializedScheduler.Key, initializedScheduler.Value);
        }


        /// <summary>
        /// Starts all active quartz schedulers
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartSchedulers()
        {
            string keyValue = string.Empty;
            try
            {
                Logger.Information("All initialized schedulers are about to be started");
                var quartzSchedulersThatHaveNotBeenStarted = QuartzSchedulers.Where(qrtz => !qrtz.Value.IsStarted);
                if (quartzSchedulersThatHaveNotBeenStarted.Count() < 1) Logger.Information("No initialized scheduler found");

                foreach (var quartzScheduler in quartzSchedulersThatHaveNotBeenStarted)
                {
                    keyValue = quartzScheduler.Key;
                    quartzScheduler.Value.Start();
                    Logger.Error(string.Format("{0} scheduler has been started ", keyValue));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("EXCEPTION OCCURED WHILE STARTING UP QUARTZ SCHEDULER {0}. EXCEPTION MESSAGE {1}", keyValue, exception.Message));
                throw new Exception(string.Format("EXCEPTION OCCURED WHILE STARTING UP QUARTZ SCHEDULER {0}. EXCEPTION MESSAGE {1}", keyValue, exception.Message));
            }
        }


        /// <summary>
        /// Start a particular scheduler with the given name
        /// <para>Before you start a scheduler, initialize it first</para>
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void StartScheduler(string schedulerName)
        {
            try
            {
                Logger.Information(string.Format("Starting up a scheduler {0} ...", schedulerName));
                var quartzScheduler = QuartzSchedulers.Where(sch => sch.Key == schedulerName).Select(sch => sch.Value).Single();

                if (quartzScheduler.IsStarted) return;

                quartzScheduler.Start().Wait();
            }
            #region catch clauses
            catch (ArgumentNullException exception)
            {
                Logger.Error(string.Format("Error starting up scheduler with name {0}. Null reference was found. Exception {1}", schedulerName, exception.Message));
                throw new KeyNotFoundException(string.Format("Error starting up scheduler with name {0}. Null reference was found. Exception {1}", schedulerName, exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(string.Format("Error starting up scheduler with name {0}. Multiple reference was found. Exception {1}", schedulerName, exception.Message));
                throw new KeyNotFoundException(string.Format("Error starting up scheduler with name {0}. Multiple reference was found. Exception {1}", schedulerName, exception.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("EXCEPTION OCCURED WHILE STARTING UP QUARTZ SCHEDULER {0}. EXCEPTION MESSAGE {1}", schedulerName, exception.Message));
                throw new Exception(string.Format("EXCEPTION OCCURED WHILE STARTING UP QUARTZ SCHEDULER {0}. EXCEPTION MESSAGE {1}", schedulerName, exception.Message));
            }
            #endregion
        }


        /// <summary>
        /// Initialize Quartz engine for the given schedulers
        /// </summary>
        /// <param name="parameters">dynamic</param>
        /// <exception cref="Exception">If error occurred while setting Quartz up</exception>
        protected Dictionary<string, IScheduler> InitializeSchedulers(List<SchedulerItem> activeSchedulers)
        {
            Logger.Information(string.Format("Initializing active schedulers"));
            Dictionary<string, IScheduler> schedulers = new Dictionary<string, IScheduler>();
            string schedulerName = string.Empty;
            try
            {
                foreach (var item in activeSchedulers)
                {
                    schedulerName = item.Name;
                    var config = item.Properties;
                    NameValueCollection properties = new NameValueCollection { };
                    if (config.Count == 0)
                    {
                        Logger.Error(string.Format("{0} QUARTZ SCHEDULER CONFIG NOT FOUND ", item.Name, DateTime.Now.ToLocalTime()));
                        throw new Exception(string.Format("{0} QUARTZ SCHEDULER CONFIG NOT FOUND ", item.Name, DateTime.Now.ToLocalTime()));
                    }
                    else
                    {
                        foreach (XmlNode key in config)
                        {
                            properties.Add(new NameValueCollection { [key.Attributes.GetNamedItem("key").Value] = key.Attributes.GetNamedItem("value").Value });
                        }
                    }
                    ISchedulerFactory scheduleFactory = new StdSchedulerFactory(properties);
                    schedulers.Add(item.Name, scheduleFactory.GetScheduler().Result);
                    Logger.Error(string.Format("{0} QUARTZ SCHEDULER HAS BEEN INITIALIZED {1}", item.Name, DateTime.Now.ToLocalTime()));
                }
                return schedulers;
            }
            #region catch clauses
            catch (Exception exception)
            {
                foreach (var item in schedulers)
                {
                    item.Value.Shutdown();
                }
                Logger.Error(string.Format("QUARTZ EXCEPTION OCCURRED IN SCHEDULER {0} {1} {2}. ALL SCHEDULERS HAVE BEEN SHUT DOWN IN THE PROCESS", schedulerName, DateTime.Now.ToLocalTime(), exception.Message));
                throw new Exception(string.Format("QUARTZ EXCEPTION OCCURRED IN SCHEDULER {0} {1} {2}. ALL SCHEDULERS HAVE BEEN SHUT DOWN IN THE PROCESS", schedulerName, DateTime.Now.ToLocalTime(), exception.Message));
            }
            #endregion
        }



        /// <summary>
        /// Get active scheduler record
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns>SchedulerItem</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public SchedulerItem GetActiveSchedulerRecord(string schedulerName)
        {
            try
            {
                return ActiveSchedulers.Where(actvsch => actvsch.Name == schedulerName).Single();
            }
            #region catch clauses
            catch (ArgumentNullException exception)
            {
                Logger.Error(string.Format("No active scheduler with the name name {0} was found. Null reference was found. Exception {1}", schedulerName, exception.Message));
                throw new KeyNotFoundException("Active scheduler not found. Null value return");
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(string.Format("No active scheduler with name {0}. Multiple reference was found or no reference found. Exception {1}", schedulerName, exception.Message));
                throw new KeyNotFoundException("Active scheduler not found. Multiple values were returned. Scheduler name " + schedulerName);
            }
            #endregion
        }


        /// <summary>
        /// Get scheduler name
        /// </summary>
        /// <returns>string</returns>
        public string SchedulerName(string schedulerName)
        {
            var name = QuartzSchedulers.Where(s => s.Key == schedulerName).Select(s => s.Key).FirstOrDefault();
            if (string.IsNullOrEmpty(name)) { throw new KeyNotFoundException("No scheduler with the given name was found"); }
            return name;
        }


        /// <summary>
        /// Create a schedule that runs on CRON expression.
        /// <para>The scheduler with the given schedulerName must has been started inorder to use this method.
        /// KeyNotFoundException is thrown if</para>
        /// </summary>
        /// <param name="model">CronSchedule</param>
        /// <exception cref="KeyNotFoundException">If scheduler with the given name has not been started</exception>
        /// <exception cref="Exception"></exception>
        public void SetupANewCronSchedule(string schedulerName, CronSchedule model, bool isActive = false)
        {
            try
            {
                Logger.Information(string.Format("setting a job schedule for {0} {1} {2} ", schedulerName, model.TriggerIdentifier, model.GroupIdentifier));

                var quartzScheduler = QuartzSchedulers.Where(sch => sch.Key == schedulerName).Select(sch => sch.Value).Single();
                //first let's check if this schedule is a fresh schedule, that is it has never been created before
                //if this schedule is a re schedule of an existing schedule, let us just simply delet the existing one and recreate an new one
                if (model.IsAReSchedule) { DeleteExistingSchedules(model.GroupIdentifier, quartzScheduler); }

                //get main job builder
                JobBuilder job = CreateJob(model);
                //get discount job builder
                JobBuilder discountJob = GetDailyRecurringScheduleJob<DiscountJob>(model.Identifier + "_Discount", model.GroupIdentifier);
                //get penalty job builder
                JobBuilder penaltyJob = GetDailyRecurringScheduleJob<PenaltyJob>(model.Identifier + "_Penalty", model.GroupIdentifier);

                //job trigger
                var trigger = TriggerBuilder.Create().WithIdentity(model.TriggerIdentifier, model.GroupIdentifier)
                              .StartAt(model.StartTime).WithCronSchedule("* * * * * ? *");//.WithCronSchedule(model.CronExpression);

                SetDurationForCronSchedule(model.Duration, job, trigger, schedulerName + " " + model.TriggerIdentifier, model.GroupIdentifier);
                //discount trigger
                TriggerBuilder discountTrigger = GetDailyRecurringScheduleTrigger(model.TriggerIdentifier + "_Discount", model.GroupIdentifier, model.StartTime, schedulerName);
                //penalty trigger
                TriggerBuilder penaltyTrigger = GetDailyRecurringScheduleTrigger(model.TriggerIdentifier + "_Penalty", model.GroupIdentifier, model.StartTime, schedulerName);


                //job builds
                var jobBuild = job.Build();
                var discountBuild = discountJob.Build();
                var penaltyBuild = penaltyJob.Build();

                //trigger builds
                var triggerBuild = trigger.Build();
                var discounTriggerBuild = discountTrigger.Build();
                var penaltyTriggerBuild = penaltyTrigger.Build();

                var result = quartzScheduler.ScheduleJob(jobBuild, triggerBuild).Result;
                var discountResult = quartzScheduler.ScheduleJob(discountBuild, discounTriggerBuild).Result;
                var penaltyResult = quartzScheduler.ScheduleJob(penaltyBuild, penaltyTriggerBuild).Result;

                if (!isActive)
                { GroupMatcher<JobKey> groupMatcher = GroupMatcher<JobKey>.GroupContains(model.GroupIdentifier);  quartzScheduler.PauseJobs(groupMatcher); }

                Logger.Information(string.Format("Job for scheduler has been created {0} {1} {2}", schedulerName, model.TriggerIdentifier, model.GroupIdentifier));
            }
            #region catch clauses
            catch (ArgumentNullException exception)
            {
                Logger.Error(string.Format("Error getting scheduler from the collection of schedulers. Query return null. Exception {0}. Schedule name {1} {2} {3}", exception.Message, schedulerName, model.TriggerIdentifier, model.GroupIdentifier));
                throw new KeyNotFoundException("Scheduler not found.");
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(string.Format("Error getting scheduler from the collection of schedulers. Query return multiple schedulers with the same name. Exception {0}. Name {1} {2} {3}", exception.Message, schedulerName, model.TriggerIdentifier, model.GroupIdentifier));
                throw new KeyNotFoundException("Multiple schedulers with the same name. Name: " + schedulerName);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("ERROR SETTING UP SCHEDULE {0} {1} {2}", exception.Message, schedulerName, model.TriggerIdentifier, model.GroupIdentifier));
                throw new Exception(string.Format("ERROR SETTING UP SCHEDULE {0} {1} {2}", exception.Message, schedulerName, model.TriggerIdentifier, model.GroupIdentifier));
            }
            #endregion
        }


        /// <summary>
        /// Get triggerbuilder for daily recurring schedules
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="groupIdentifier"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private static TriggerBuilder GetDailyRecurringScheduleTrigger(string identifier, string groupIdentifier, DateTime startTime, string schedulerName) => TriggerBuilder.Create().WithIdentity(identifier, groupIdentifier).StartAt(startTime).WithSimpleSchedule(act => act.RepeatForever().WithIntervalInSeconds(30));//.WithDailyTimeIntervalSchedule(action => action.StartingDailyAt(new TimeOfDay(00, 00)));


        /// <summary>
        /// Get jobbuilder for daily recurring schedules
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JobBuilder</returns>
        private JobBuilder GetDailyRecurringScheduleJob<M>(string identifier, string groupIdentifier) where M : IJob  => JobBuilder.Create<M>().WithDescription("DISCOUNT JOB: " + identifier + " " + groupIdentifier).WithIdentity(identifier, groupIdentifier);
           

        /// <summary>
        /// Delete existing job under a group
        /// </summary>
        /// <param name="model"></param>
        /// <param name="quartzScheduler"></param>
        private void DeleteExistingSchedules(string groupName, IScheduler quartzScheduler)
        {
            GroupMatcher<JobKey> groupMatcher = GroupMatcher<JobKey>.GroupContains(groupName);
            var jobs = quartzScheduler.GetJobKeys(groupMatcher).Result;
            foreach (var item in jobs)
            {
                var jobKey = new JobKey(item.Name, groupName);
                var result = quartzScheduler.DeleteJob(jobKey).Result;
            }
        }


        /// <summary>
        /// Create a job
        /// </summary>
        /// <param name="model">CronSchedule</param>
        /// <returns>JobBuilder</returns>
        private static JobBuilder CreateJob(CronSchedule model)
        {
            var job = JobBuilder.Create<FixedBillingJob>().WithDescription(model.Description).WithIdentity(model.Identifier, model.GroupIdentifier);
            foreach (var item in model.JobData) { job.UsingJobData(item.Key, item.Value.ToString()); }
            return job;
        }


        /// <summary>
        /// Set trigger duration for cron schedules
        /// </summary>
        /// <param name="duration">TriggerDurationHelperModel</param>
        /// <param name="trigger">TriggerBuilder</param>
        private void SetDurationForCronSchedule(TriggerDurationHelperModel duration, JobBuilder job, TriggerBuilder trigger, string jobName, string jobGroup)
        {
            try
            {
                if (duration.TriggerDurationType == TriggerDurationType.EndsAfter)
                {
                    if (duration.EndsAfterRounds <= 0)
                    {
                        Logger.Error(string.Format("Ends after round not valid. Value {0}. JN: {1} JG: {2}", duration.EndsAfterRounds, jobName, jobGroup));
                        throw new Exception(string.Format("No ends after round found. Trigger: {0} Duration: {1}. JN: {2} JG: {3}", Utils.ComplexDump(trigger), Utils.ComplexDump(duration), jobName, jobGroup));
                    } 
                    //add the number of rounds you want the job to run.
                    job.UsingJobData(JobDataKs.ROUNDS, duration.EndsAfterRounds.ToString());
                    Logger.Information(string.Format("Ends after {0} has been set for JN {1} JG {2}", duration.EndsAfterRounds, jobName, jobGroup));
                    return;
                }
                else if (duration.TriggerDurationType == TriggerDurationType.EndsOn)
                {
                    try
                    {
                        trigger.EndAt(duration.EndsDate);
                        Logger.Information(string.Format("Ends after date has been set {0}. For JN: {1} JG: {2}", duration.EndsDate.Value.ToString("dd'/'MM'/'yyyy"), jobName, jobGroup));
                        return;
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(string.Format("Error set cron schedule duration {0}. For JN: {1} JG: {2}", exception.Message, jobName, jobGroup));
                        throw new Exception("Unexpected date format found. Expected Date format is dd/MM/yyyy e.g 29/12/2017");
                    }
                }
                else if (duration.TriggerDurationType == TriggerDurationType.NeverEnds) return;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error set cron schedule duration {0}. For JN: {1} JG: {2}", exception.Message, jobName, jobGroup));
                throw;
            }
            Logger.Error(string.Format("No billing duration type found Trigger: {0}. Duration: {1}. For JN: {3} JG: {4}", Utils.ComplexDump(trigger), Utils.ComplexDump(duration), jobName, jobGroup));
            throw new Exception("No billing duration type found");
        }
        
        
        public string SchedulerNames() => string.Join(",", QuartzSchedulers.Select(schd => schd.Value.SchedulerName).ToArray());

        public void HelloJob()
        {
            throw new NotImplementedException();
        }        
    }
}
