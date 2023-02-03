using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DataFilters.SettlementReport.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class SettlementReportHandler : ISettlementReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly Lazy<ISettlementReportFilter> _settlementReportFilter;

        public ILogger Logger { get; set; }
        public SettlementReportHandler(IHandlerComposition handlerComposition, Lazy<ISettlementReportFilter> settlementReportFilter)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _settlementReportFilter = settlementReportFilter;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }

        /// <summary>
        /// Gets the view model for settlement report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSSettlementReportVM GetVMForReports(SettlementReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _settlementReportFilter.Value.GetSettlementReportViewModel(searchParams);
            IEnumerable<PSSSettlementVM> reports = (IEnumerable<PSSSettlementVM>)recordsAndAggregate.ReportRecords;

            return new PSSSettlementReportVM
            {
                Settlements = reports.ToList(),
                TotalActiveSettlements = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveSettlements).First().TotalRecordCount,
            };
        }
    }
}