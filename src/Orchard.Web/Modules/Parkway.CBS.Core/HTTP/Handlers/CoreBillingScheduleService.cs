using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Modules.Services;
using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using System;
using Parkway.CBS.Core.Models.Enums;
using System.Linq;
using Parkway.CBS.Core.Schedulers.Contracts;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreBillingScheduleService : CoreBaseService, ICoreBillingScheduleService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IBillingModelManager<BillingModel> _biilingModelRepository;
        public IInvoicingService _invoicingService;
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        private readonly IEnumerable<IBillingFrequencyFilter> _billingFrequencyFilter;
        public Localizer T { get; set; }
        private readonly IBillingScheduleManager<BillingSchedule> _scheduleRepository;
        private readonly IBillingScheduler _scheduler;

        public CoreBillingScheduleService(IOrchardServices orchardServices, IBillingModelManager<BillingModel> biilingModelRepository,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, ICoreRevenueHeadService revenueHeadCoreService, IEnumerable<IBillingFrequencyFilter> billingFrequencyFilter, IBillingScheduleManager<BillingSchedule> scheduleRepository, IBillingScheduler scheduler)
            : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _biilingModelRepository = biilingModelRepository;
            _invoicingService = invoicingService;
            _revenueHeadCoreService = revenueHeadCoreService;
            _billingFrequencyFilter = billingFrequencyFilter;
            Logger = NullLogger.Instance;
            _scheduleRepository = scheduleRepository;
            _scheduler = scheduler;
        }

        
        /// <summary>
        /// Check if the schedule is still valid to run
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool IsScheduleDurationValid(BillingSchedule schedule, DurationModel duration)
        {
            switch (duration.DurationType)
            {
                case DurationType.NeverEnds:
                    return true;
                case DurationType.EndsAfter:
                    return schedule.Rounds <= duration.EndsAfterRounds ? true : false;
                case DurationType.EndsOn:
                    //return schedule.NextRunDate <= duration.EndsDate ? true : false;
                    return true;// schedule.NextRunDate <= duration.EndsDate ? true : false;
                default:
                    throw new BillingDurationException("No billing duration type found");
            }
        }

        /// <summary>
        /// Check if schdule has been stopped
        /// </summary>
        /// <returns>bool</returns>
        public bool CanScheduleRun(BillingSchedule schedule)
        {
            ScheduleStatus status = GetStatus(schedule);
            return (status == ScheduleStatus.Running) || (status == ScheduleStatus.NotStarted) ? true : false;
        }

        private ScheduleStatus GetStatus(BillingSchedule schedule)
        {
            if (typeof(ScheduleStatus).IsEnumDefined(schedule.ScheduleStatus)) { throw new NoScheduleStatusFoundException("No billing schedule status found. Schedule: " + schedule.ScheduleStatus); }
            return (ScheduleStatus)schedule.ScheduleStatus;
        }


        public BillingSchedule GetSchedule(BillingModel billing)
        {
            BillingSchedule schedule = _scheduleRepository.GetCollection(sch => sch.BillingModel == billing).FirstOrDefault();
            if (schedule == null) { throw new NoScheduleFoundException(); }
            return schedule;
        }

        public List<BillingSchedule> TryCreateVariableSchedules(MDA mda, RevenueHead revenueHead, BillingModel billing, IEnumerable<TaxEntityInvoice> validatedTaxPayers, DateTime startTime)
        {
            List<BillingSchedule> schedules = new List<BillingSchedule>();

            foreach (TaxEntityInvoice item in validatedTaxPayers)
            {
                BillingSchedule schedule = new BillingSchedule
                {
                    BillingModel = billing,
                    MDA = mda,
                    RevenueHead = revenueHead,
                    TaxPayer = item.TaxEntity,
                    ScheduleStatus = (int)ScheduleStatus.Running,
                    //StartDateAndTime = startTime.Date,
                    //NextRunDate = startTime.Date,
                };
                schedules.Add(schedule);
            }
            SaveSchedules(schedules);
            return schedules;
        }


        private void SaveSchedules(List<BillingSchedule> schedules)
        {
            if (_scheduleRepository.SaveBundle(schedules)) { throw new CouldNotSaveBillingScheduleException(); }
        }


        public IEnumerable<BillingSchedule> GetSchedules(BillingModel billing, IEnumerable<TaxEntity> taxPayers)
        {
            //get schedules for these taxpayers
            return _scheduleRepository.GetScheduleForTaxPayers(billing, taxPayers);
        }


        public void RegisterAFixedBillingSchedule(ExpertSystemSettings tenant, RevenueHead revenueHead, ScheduleHelperModel helper, BillingType billingType)
        {
            //check if the billing is fixed
            if (billingType != BillingType.Fixed) throw new Exception("This billing type is not fixed");
            _scheduler.CreateAFixedBillingSchedule(tenant, revenueHead, helper);
        }
    }
}