using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBatchBreakdown.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportParty.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSSettlementReportHandler : IPSSSettlementReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPSSSettlementBatchItemsManager<PSSSettlementBatchItems> _pssSettlementBatchItemsManager;
        private readonly IPSSSettlementBatchAggregateManager<PSSSettlementBatchAggregate> _pssSettlementBatchAggregateManager;
        private readonly IPSSSettlementBatchManager<PSSSettlementBatch> _pssSettlementBatchManager;
        private readonly Lazy<ICoreStateAndLGA> _coreStateAndLGA;
        private readonly IPSServiceManager<PSService> _psServiceManager;
        private readonly IPSSSettlementReportPartyFilter _iPSSSettlementReportPartyFilter;
        private readonly IPSSSettlementReportAggregateFilter _iPSSSettlementReportAggregateFilter;
        private readonly IPSSSettlementManager<PSSSettlement> _pssSettlementManager;
        private readonly IPSSSettlementReportPartyBreakdownFilter _iPSSSettlementReportPartyBreakdownFilter;
        private readonly IPSSSettlementFeePartyBatchAggregateManager<PSSSettlementFeePartyBatchAggregate> _pSSSettlementFeePartyBatchAggregateManager;
        private readonly IPSSSettlementReportBreakdownFilter _iPSSSettlementReportBreakdownFilter;
        private readonly IPSSFeePartyManager<PSSFeeParty> _pSSFeePartyManager;
        private readonly ICoreCommand _coreCommand;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSSettlementReportBatchBreakdownFilter _iPSSSettlementReportBatchBreakdownFilter;
        ILogger Logger { get; set; }
        public PSSSettlementReportHandler(IHandlerComposition handlerComposition, IPSSSettlementBatchItemsManager<PSSSettlementBatchItems> pssSettlementBatchItemsManager, IPSSSettlementBatchAggregateManager<PSSSettlementBatchAggregate> pssSettlementBatchAggregateManager, IPSSSettlementBatchManager<PSSSettlementBatch> pssSettlementBatchManager, Lazy<ICoreStateAndLGA> coreStateAndLGA, IPSServiceManager<PSService> psServiceManager, IPSSSettlementReportPartyFilter iPSSSettlementReportPartyFilter, IPSSSettlementReportAggregateFilter iPSSSettlementReportAggregateFilter, IPSSSettlementManager<PSSSettlement> pssSettlementManager, IPSSSettlementReportPartyBreakdownFilter iPSSSettlementReportPartyBreakdownFilter, IPSSSettlementFeePartyBatchAggregateManager<PSSSettlementFeePartyBatchAggregate> pSSSettlementFeePartyBatchAggregateManager, IPSSSettlementReportBreakdownFilter iPSSSettlementReportBreakdownFilter, IPSSFeePartyManager<PSSFeeParty> pSSFeePartyManager, ICoreCommand coreCommand, IOrchardServices orchardServices, IPSSSettlementReportBatchBreakdownFilter iPSSSettlementReportBatchBreakdownFilter)
        {
            _handlerComposition = handlerComposition;
            _pssSettlementBatchItemsManager = pssSettlementBatchItemsManager;
            _pssSettlementBatchAggregateManager = pssSettlementBatchAggregateManager;
            _pssSettlementBatchManager = pssSettlementBatchManager;
            _coreStateAndLGA = coreStateAndLGA;
            _psServiceManager = psServiceManager;
            _iPSSSettlementReportPartyFilter = iPSSSettlementReportPartyFilter;
            _iPSSSettlementReportAggregateFilter = iPSSSettlementReportAggregateFilter;
            _pssSettlementManager = pssSettlementManager;
            _iPSSSettlementReportPartyBreakdownFilter = iPSSSettlementReportPartyBreakdownFilter;
            _pSSSettlementFeePartyBatchAggregateManager = pSSSettlementFeePartyBatchAggregateManager;
            _iPSSSettlementReportBreakdownFilter = iPSSSettlementReportBreakdownFilter;
            _pSSFeePartyManager = pSSFeePartyManager;
            _coreCommand = coreCommand;
            _orchardServices = orchardServices;
            _iPSSSettlementReportBatchBreakdownFilter = iPSSSettlementReportBatchBreakdownFilter;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Gets PSS Settlement Report Summary VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportSummaryVM GetVMForReportSummary(PSSSettlementReportSearchParams searchParams)
        {
            try
            {
                return new PSSSettlementReportSummaryVM
                {
                    ReportRecords = _pssSettlementBatchAggregateManager.GetReportRecords(searchParams),
                    TotalReportRecords = _pssSettlementBatchAggregateManager.GetCount(searchParams).FirstOrDefault(),
                    From = searchParams.StartDate.ToString("dd/MM/yyyy"),
                    End = searchParams.EndDate.ToString("dd/MM/yyyy"),
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets PSS Settlement Report Invoices VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportInvoicesVM GetVMForReportInvoices(PSSSettlementReportSearchParams searchParams)
        {
            try
            {
                return new PSSSettlementReportInvoicesVM
                {
                    ReportRecords = _pssSettlementBatchItemsManager.GetReportRecords(searchParams),
                    TotalReportRecords = _pssSettlementBatchItemsManager.GetCount(searchParams.BatchId).FirstOrDefault(),
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets PSS Settlement Report Fee Party Breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportPartiesBreakdownVM GetVMForReportPartyBreakdown(PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            try
            {
                dynamic recordsAndAggregate = _iPSSSettlementReportPartyBreakdownFilter.GetReportViewModel(searchParams);
                IEnumerable<PSSSettlementBatchItemsVM> reports = ((IEnumerable<PSSSettlementBatchItemsVM>)recordsAndAggregate.ReportRecords);
                PSSSettlementFeePartyBatchAggregateVM feePartyBatchAggregate = _pSSSettlementFeePartyBatchAggregateManager.GetFeePartyBatchInfo(searchParams.BatchRef, searchParams.FeePartyBatchAggregateId);

                return new PSSSettlementReportPartiesBreakdownVM
                {
                    ReportRecords = (reports == null || !reports.Any()) ? new List<PSSSettlementBatchItemsVM> { } : reports.ToList(),
                    TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfBatchItems).First().TotalRecordCount),
                    AmountSettled = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.AmountSettled).First().TotalAmount,
                    SettlementName = feePartyBatchAggregate.Batch.SettlementName,
                    SettlementStartDate = feePartyBatchAggregate.Batch.SettlementRangeStartDate,
                    SettlementEndDate = feePartyBatchAggregate.Batch.SettlementRangeEndDate,
                    FeePartyName = feePartyBatchAggregate.FeePartyName,
                    SettlementDate = feePartyBatchAggregate.SettlementDate
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets Id of PSS Settlement Batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public long GetSettlementBatchId(string batchRef)
        {
            try
            {
                return _pssSettlementBatchManager.GetSettlementBatchId(batchRef);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Gets PSS Settlement Batch Breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportBatchBreakdownVM GetVMForReportBatchBreakdown(PSSSettlementReportBatchBreakdownSearchParams searchParams)
        {
            try
            {
                dynamic recordsAndAggregate = _iPSSSettlementReportBatchBreakdownFilter.GetReportViewModel(searchParams);
                IEnumerable<PSSSettlementBatchItemsVM> reports = ((IEnumerable<PSSSettlementBatchItemsVM>)recordsAndAggregate.ReportRecords);
                PSSSettlementBatchVM batch = _pssSettlementBatchManager.GetSettlementBatchWithRef(searchParams.BatchRef);

                return new PSSSettlementReportBatchBreakdownVM
                {
                    ReportRecords = (reports == null || !reports.Any()) ? new List<PSSSettlementBatchItemsVM> { } : reports.ToList(),
                    TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfBatchItems).First().TotalRecordCount),
                    AmountSettled = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.AmountSettled).First().TotalAmount,
                    SettlementName = batch.SettlementName,
                    SettlementStartDate = batch.SettlementRangeStartDate,
                    SettlementEndDate = batch.SettlementRangeEndDate,
                    SettlementDate = batch.TransactionDate
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets settlement report breakdown vm
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportBreakdownVM GetSettlementReportBreakdownVM(PSSSettlementReportBreakdownSearchParams searchParams)
        {
            try
            {
                string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
                dynamic recordsAndAggregate = _iPSSSettlementReportBreakdownFilter.GetReportViewModel(searchParams);
                IEnumerable<PSSSettlementBatchItemsVM> reports = ((IEnumerable<PSSSettlementBatchItemsVM>)recordsAndAggregate.ReportRecords);

                IEnumerable<CommandVM> commands = ObjectCacheProvider.GetCachedObject<IEnumerable<CommandVM>>(tenant, nameof(POSSAPCachePrefix.Commands));
                IEnumerable<PSSFeePartyVM> feeParties = ObjectCacheProvider.GetCachedObject<IEnumerable<PSSFeePartyVM>>(tenant, nameof(POSSAPCachePrefix.FeeParties));
                IEnumerable<PSServiceVM> services = ObjectCacheProvider.GetCachedObject<IEnumerable<PSServiceVM>>(tenant, nameof(POSSAPCachePrefix.Services));

                if (commands == null)
                {
                    commands = _coreCommand.GetCommands();

                    if(commands != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, nameof(POSSAPCachePrefix.Commands), commands);
                    }
                }

                if(feeParties == null)
                {
                    feeParties = _pSSFeePartyManager.GetAllActiveFeeParties();

                    if(feeParties != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, nameof(POSSAPCachePrefix.FeeParties), feeParties);
                    }
                }

                if(services == null)
                {
                    services = _psServiceManager.GetServices().ToList();

                    if(services != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, nameof(POSSAPCachePrefix.Services), services);
                    }
                }

                return new PSSSettlementReportBreakdownVM
                {
                    StartDate = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                    EndDate = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                    ReportRecords = (reports == null || !reports.Any()) ? new List<PSSSettlementBatchItemsVM> { } : reports.ToList(),
                    TotalRecordCount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalRecordCount).First().TotalRecordCount,
                    StateAndLGAs = _coreStateAndLGA.Value.GetStateVMs(),
                    LGAs = (searchParams.SelectedState > 0) ? _coreStateAndLGA.Value.GetLgas(searchParams.SelectedState).Select(x => new LGAVM { Id = x.Id, Name = x.Name }) : new List<LGAVM> { },
                    Services = services,
                    SelectedLGA = searchParams.SelectedLGA,
                    SelectedState = searchParams.SelectedState,
                    SelectedService = searchParams.SelectedService,
                    SelectedSettlementParty = searchParams.SelectedSettlementParty,
                    FeeParties = feeParties,
                    FileNumber = searchParams.FileNumber,
                    InvoiceNumber = searchParams.InvoiceNumber,
                    Commands = commands,
                    SelectedCommand = searchParams.SelectedCommand,
                    TotalReportAmount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalReportAmount).First().TotalAmount
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets View Model for settlement report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportAggregateVM GetVMForReportAggregate(PSSSettlementReportAggregateSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _iPSSSettlementReportAggregateFilter.GetRequestReportViewModel(searchParams);
            IEnumerable<PSSSettlementBatchVM> reports = ((IEnumerable<PSSSettlementBatchVM>)recordsAndAggregate.ReportRecords);

            return new PSSSettlementReportAggregateVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                ReportRecords = (reports == null || !reports.Any()) ? new List<PSSSettlementBatchVM> { } : reports.ToList(),
                TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfSettlements).First().TotalRecordCount),
                Settlements = _pssSettlementManager.GetActiveSettlements(),
                SelectedSettlement = searchParams.SettlementId
            };
        }


        /// <summary>
        /// Gets View Model for settlement report party
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportPartyVM GetVMForReportParty(PSSSettlementReportPartySearchParams searchParams)
        {
            dynamic recordsAndAggregate = _iPSSSettlementReportPartyFilter.GetRequestReportViewModel(searchParams);
            IEnumerable<PSSSettlementFeePartyBatchAggregateVM> reports = ((IEnumerable<PSSSettlementFeePartyBatchAggregateVM>)recordsAndAggregate.ReportRecords);

            return new PSSSettlementReportPartyVM
            {
                ReportRecords = (reports == null || !reports.Any()) ? new List<PSSSettlementFeePartyBatchAggregateVM> { } : reports.ToList(),
                TotalRecordCount = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfFeeParties).First().TotalRecordCount),
                SettlementBatch = _pssSettlementBatchManager.GetSettlementBatchWithRef(searchParams.BatchRef),
                TotalAmountSettled = (decimal)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAmountSettled).First().TotalAmount)
            };
        }
    }
}