using Autofac;
using Newtonsoft.Json;
using Orchard;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Settings;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Schedulers.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.Scheduler.Interface.Schedulers;
using Parkway.Scheduler.Interface.Schedulers.Contracts;
using Parkway.Scheduler.Interface.Schedulers.Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Core.Schedulers
{
    public class BillingScheduler : IBillingScheduler
    {
        // ShellSettingsManager lets you access the shell settings of all the tenants.
        private readonly IShellSettingsManager _shellSettingsManager;
        // OrchardHost is the very core Orchard service running the environment.
        private readonly IOrchardHost _orchardHost;
        private readonly IBillingScheduleManager<BillingSchedule> _billingScheduleRepository;
        private readonly IAdminSettingManager<ExpertSystemSettings> _tenantRepository;
        private readonly IRemoteClient _remoteClient;

        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public BillingScheduler(IBillingScheduleManager<BillingSchedule> billingScheduleRepository, IAdminSettingManager<ExpertSystemSettings> tenantRepository, IRemoteClient remoteClient, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost, IOrchardServices orchardServices)
        {
            _shellSettingsManager = shellSettingsManager;
            _orchardHost = orchardHost;
            _billingScheduleRepository = billingScheduleRepository;
            _tenantRepository = tenantRepository;
            _remoteClient = remoteClient;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Register a fixed schedule
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="revenueHead"></param>
        /// <param name="helper"></param>
        public void CreateAFixedBillingSchedule(ExpertSystemSettings tenant, RevenueHead revenueHead, ScheduleHelperModel helper, bool reshedule = false)
        {
            IJobSchedulerInterface schedulerInterface = new Scheduler.Interface.SchedulerInterface().GetScheduler();
            var cronObj = CreateCronScheduleModel(tenant, revenueHead, helper, reshedule);
            //initialize the scheduler we want, this is the state name suffixed with _BillingScheduler, add to the web config file
            schedulerInterface.InitializeScheduler(tenant.BillingSchedulerIdentifier);
            schedulerInterface.SetupANewCronSchedule(tenant.BillingSchedulerIdentifier, cronObj, revenueHead.IsActive);
            schedulerInterface = null;
        }


        /// <summary>
        /// Get cron schedule model
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="revenueHead"></param>
        /// <param name="billing"></param>
        /// <returns>CronSchedule</returns>
        private CronSchedule CreateCronScheduleModel(ExpertSystemSettings tenant, RevenueHead revenueHead, ScheduleHelperModel helperModel, bool isAReSchedule)
        {
            Logger.Debug("creating cron schedule");
            CronSchedule cronObj = new CronSchedule();
            cronObj.CronExpression = helperModel.CronExpression;
            cronObj.Description = string.Format("CRON schedule for Billing {0}", helperModel.BillingIdentifier);
            cronObj.Duration = new TriggerDurationHelperModel { EndsAfterRounds = helperModel.Duration.EndsAfterRounds, EndsDate = helperModel.Duration.EndsDate, TriggerDurationType = GetTriggerDurationType(helperModel.Duration.DurationType) };
            cronObj.GroupIdentifier = string.Format("RevenueHead{0}", revenueHead.Id);
            cronObj.Identifier = string.Format("Billing{0}", helperModel.BillingIdentifier);
            cronObj.TriggerIdentifier = string.Format("Billing{0}_Trigger", helperModel.BillingIdentifier);
            cronObj.StartTime = helperModel.StartDateAndTime;
            cronObj.JobData = new Dictionary<string, dynamic>();
            cronObj.JobData.Add("ClientId", tenant.ClientId);
            cronObj.JobData.Add("Tenant", _orchardServices.WorkContext.CurrentSite.SiteName);
            cronObj.IsAReSchedule = isAReSchedule;
            return cronObj;
        }


        /// <summary>
        /// Get trigger duration type
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>TriggerDurationType</returns>
        private TriggerDurationType GetTriggerDurationType(DurationType identifier)
        {
            if (identifier == DurationType.EndsAfter)
            {
                return TriggerDurationType.EndsAfter;
            }
            else if (identifier == DurationType.EndsOn)
            {
                return TriggerDurationType.EndsOn;
            }
            else if (identifier == DurationType.NeverEnds)
            {
                return TriggerDurationType.NeverEnds;
            }
            throw new BillingDurationException(string.Format("No billing duration found for the given identifier {0}", identifier));
        }

        public bool AnyScheduledTask(DateTime dateTime)
        {
            return false;
        }

        public void RunBillingSchedule(DateTime dateTime)
        {
            //var schedules = _billingScheduleRepository.GetCollection(sch => sch.NextRunDate == dateTime);
            var schedules = new List<BillingSchedule>();// _billingScheduleRepository.GetCollection(sch => sch.NextRunDate == dateTime);
            if (schedules.Count() <= 0) { return; }
            Parallel.ForEach(schedules, sch => SendAnInvoice(sch));
            //forEach (var schedule in schedules)
            //{
            //    SendAnInvoice(schedule);
            //}
        }

        public void SendAnInvoice(BillingSchedule schedule)
        {
            //var q = CentralBillingSystemSchedulingWinService.BillingSchedulerService.GetSchedulerServiceName();
            //var sd = ClassLibrary1.Class1.C();
            //var sd = SeriTestLib.TestLogger.Lo();
            //var sdfd = Scheduler.Interface.LK.P();
            //var sds = Parkway.Schedulers.Interface.Sch.F();
            //var mn = Parkway.Scheduler.Interface.LK.P();
            //return;
            //if (!_billingScheduleService.CanScheduleRun(schedule)) { Logger.Error("Schedule has stopped or expired"); return; }
            var tenantShellSettingsx = _shellSettingsManager.LoadSettings().Select(settings => settings);//.Where(settings => settings.Name == "TenantName").Single();
            var ltenantShellSettingsx = _shellSettingsManager.LoadSettings().Select(settings => settings);//.Where(settings => settings.Name == "TenantName").Single();
            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name == "TenantX").Single();
            var s = _shellSettingsManager.LoadSettings().Where(settings => settings.Name == "Default").Single();

            var shellContext = _orchardHost.GetShellContext(tenantShellSettings);
            var sp = _orchardHost.GetShellContext(s);

            using (var wc = sp.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
            {
                // You can resolve services from the tenant that you would normally inject through the constructor.
                var tenantSiteName = wc.Resolve<ISiteService>().GetSiteSettings().SiteName;
                var bvc = wc.Resolve<IAdminSettingManager<ExpertSystemSettings>>().GetCollection(v => v.Id != 0).FirstOrDefault();
                // ...
            }
            // Creating a new work context to run our code. Resolve() needs using Autofac;
            using (var wc = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
            {
                // You can resolve services from the tenant that you would normally inject through the constructor.
                var tenantSiteName = wc.Resolve<ISiteService>().GetSiteSettings().SiteName;
                var bvc = wc.Resolve<IAdminSettingManager<ExpertSystemSettings>>().GetCollection(v => v.Id != 0).FirstOrDefault();
            }


            {
                var properties = new NameValueCollection
                {
                    ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                    ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
                    ["quartz.threadPool.threadCount"] = "5",
                    ["quartz.jobStore.tablePrefix"] = "Parkway_CBS_Core_QRTZ_",
                    ["quartz.scheduler.instanceName"] = "MyScheduler",
                    ["quartz.scheduler.instanceId"] = "AUTO",
                    ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                    ["quartz.jobStore.dataSource"] = "default",
                    ["quartz.jobStore.clustered"] = "false",
                    // this is the default
                    ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.SimpleSemaphore, Quartz",
                    ["quartz.jobStore.useProperties"] = "true",
                    ["quartz.jobStore.selectWithLockSQL"] = "SELECT * FROM {0} LOCKS UPDLOCK WHERE LOCK_NAME = @ lockName",
                    ["quartz.dataSource.default.connectionString"] = "Data Source=(local);Initial Catalog=CentralBillingSystem;User ID=sa;Password=password01",
                    ["quartz.dataSource.default.provider"] = "SqlServer",
                    ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.SimpleSemaphore, Quartz",
                    // this is not the default
                    //["quartz.plugin.xml.failOnSchedulingError"] = "true",
                    ["quartz.serializer.type"] = "binary",
                    ["quartz.jobStore.useProperties"] = "true"
                };

                //ISchedulerFactory sf = new StdSchedulerFactory(properties);
                //IScheduler sched = sf.GetScheduler().Result;

                //var startTime = DateTimeOffset.Now.AddSeconds(5);

                //var job = JobBuilder.Create<SimpleJob>()
                //                    .WithIdentity("job2", "group2")
                //                    .Build();

                //var trigger = TriggerBuilder.Create()
                //    .WithIdentity("trigger2", "group2")
                //    .StartAt(startTime)
                //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())
                //    .Build();

                //sched.ScheduleJob(job, trigger);

                //sched.Start();
                Thread.Sleep(TimeSpan.FromMinutes(2));
            }

            throw new Exception();

            ExpertSystemSettings tenant = _tenantRepository.GetCollection(t => t.Id != 0).First();
            BillingModel billing = schedule.BillingModel;
            RevenueHead revenueHead = schedule.RevenueHead;
            MDA mda = schedule.MDA;

            //if(!_billingService.IsBillingAllowed(mda, revenueHead))
            //{
            //    schedule.ScheduleStatus = (int)ScheduleStatus.Stopped;
            //    Logger.Error(string.Format("Billing is not allowed for this revenue head. Revenue head {0} : MDA {1}", revenueHead.IsActive, mda.IsActive));
            //    return;
            //}

            //DurationModel duration = _billingService.GetDuration(billing);
            //if(!_billingScheduleService.IsScheduleDurationValid(schedule, duration))
            //{
            //    schedule.ScheduleStatus = (int)ScheduleStatus.HasExpired;
            //    Logger.Error(string.Format("Schedule duartion has expired. Schedule id {0}", schedule.Id));
            //    return;
            //}

            if (!typeof(BillingType).IsEnumDefined(billing.BillingType)) { throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed for schedule {0} and type ", schedule.Id, billing.BillingType)); }

            BillingType billingType = (BillingType)billing.BillingType;

            QueueInvoices(tenant, mda, revenueHead, billing, schedule, billingType);
            //_billingService.CreateInvoice(schedule, tenant, billing, revenueHead, mda);
        }

        private void QueueInvoices(ExpertSystemSettings tenant, MDA mda, RevenueHead revenueHead, BillingModel billing, BillingSchedule schedule, BillingType billingType)
        {
            if (billingType == BillingType.Fixed)
            {
                CreateInvoices(tenant, mda, revenueHead, billing, schedule);
            }
            else
            {
                CreateInvoice(tenant, mda, revenueHead, billing, schedule);
            }
            throw new NotImplementedException();
        }

        private void CreateInvoice(ExpertSystemSettings tenant, MDA mda, RevenueHead revenueHead, BillingModel billing, BillingSchedule schedule)
        {
            //check if the tax payer is still on the list of tax payers
            RequestModel request = new RequestModel { URL = "" };
            var result = _remoteClient.SendRequest(request, HttpMethod.Get, new Dictionary<string, string> { { "revenuehead", revenueHead.Id.ToString() }, { "taxpayernumber", schedule.TaxPayerNumber } });
            var taxPayer = JsonConvert.DeserializeObject<TaxEntity>(result);
            if (taxPayer == null) { return; }
            //_invoiceService.CreateInvoice(tenant, mda, revenueHead, billing, schedule, new TaxEntityInvoice { TaxEntity = taxPayer });
        }

        private void CreateInvoices(ExpertSystemSettings tenant, MDA mda, RevenueHead revenueHead, BillingModel billing, BillingSchedule schedule)
        {
            throw new NotImplementedException();
        }

        private void GetTaxPayers(ExpertSystemSettings tenant, RevenueHead revenueHead, BillingModel billing, BillingSchedule schedule, MDA mda, Int64 dataSize, Int64 blockSize, int pointer, bool firstRun = false)
        {
            if (!firstRun && dataSize <= (blockSize * pointer)) { return; }
            RequestModel request = new RequestModel { URL = "" };
            var result = _remoteClient.SendRequest(request, HttpMethod.Get, new Dictionary<string, string> { { "blocksize", blockSize.ToString() }, { "pointer", pointer.ToString() } });
            var refData = JsonConvert.DeserializeObject<TaxPayersReferenceDataRequestModel>(result);
            ProcessInvoicesForFixedSchedules(schedule, tenant, billing, revenueHead, mda, refData.Count, blockSize, pointer++);
        }

        private void DoAnInvoice(BillingSchedule schedule, ExpertSystemSettings tenant, BillingModel billing, RevenueHead revenueHead, MDA mda)
        {
            if (!typeof(BillingType).IsEnumDefined(billing.BillingType)) { throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed for schedule {0} and type ", schedule.Id, billing.BillingType)); }

            BillingType billingType = (BillingType)billing.BillingType;

            if (billingType == BillingType.Fixed)
            {
                ProcessInvoicesForFixedSchedules(schedule, tenant, billing, revenueHead, mda, 0, 1000, 0, true);
            }
            else
            {
                ProcessInvoicesForVariableSchedules(schedule, tenant, billing, revenueHead, mda);
            }
        }

        private void ProcessInvoicesForVariableSchedules(BillingSchedule schedule, ExpertSystemSettings tenant, BillingModel billing, RevenueHead revenueHead, MDA mda)
        {
            RequestModel request = new RequestModel { URL = "" };
        }

        private void ProcessInvoicesForFixedSchedules(BillingSchedule schedule, ExpertSystemSettings tenant, BillingModel billing, RevenueHead revenueHead, MDA mda, Int64 dataSize, Int64 blockSize, int pointer, bool firstRun = false)
        {
            if (!firstRun && dataSize <= (blockSize * pointer)) { return; }
            RequestModel request = new RequestModel { URL = "" };
            var result = _remoteClient.SendRequest(request, HttpMethod.Get, new Dictionary<string, string> { { "BlockSize", blockSize.ToString() }, { "Pointer", pointer.ToString() } });
            var refData = JsonConvert.DeserializeObject<TaxPayersReferenceDataRequestModel>(result);
            ProcessInvoicesForFixedSchedules(schedule, tenant, billing, revenueHead, mda, refData.Count, blockSize, pointer++);
        }

        private string GetTaxPayers(ExpertSystemSettings tenant)
        {
            RequestModel request = new RequestModel { URL = "" };
            int blockSize = 1000;
            int pointer = 0;
            return _remoteClient.SendRequest(request, HttpMethod.Get, new Dictionary<string, string> { { "BlockSize", blockSize.ToString() }, { "Pointer", pointer.ToString() } });
        }

        
    }
}