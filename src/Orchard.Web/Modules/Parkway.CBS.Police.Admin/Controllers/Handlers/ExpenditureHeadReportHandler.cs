using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class ExpenditureHeadReportHandler : IExpenditureHeadReportHandler
    {
        private readonly Lazy<IExpenditureHeadReportFilter> _expenditureHeadReportFilters;
        private readonly IHandlerComposition _handlerComposition;

        public ExpenditureHeadReportHandler(Lazy<IExpenditureHeadReportFilter> expenditureHeadReportFilters, IHandlerComposition handlerComposition)
        {
            _expenditureHeadReportFilters = expenditureHeadReportFilters;
            _handlerComposition = handlerComposition;
        }

        public void CheckForPermission(Permission canViewExpenditureHeadReport)
        {
            _handlerComposition.IsAuthorized(canViewExpenditureHeadReport);
        }

        public ExpenditureHeadReportVM GetVMForReports(ExpenditureHeadReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _expenditureHeadReportFilters.Value.GetExpenditureHeadReportViewModel(searchParams);
            IEnumerable<Core.HelperModels.ExpenditureHeadReportVM> reports = (IEnumerable<Core.HelperModels.ExpenditureHeadReportVM>)recordsAndAggregate.ReportRecords;

            return new ExpenditureHeadReportVM
            {
                ExpenditureHeadReports = reports,
                ExpenditureHeadName = searchParams.ExpenditureHeadName,
                Code = searchParams.Code,
                Status = searchParams.Status,
                TotalExpenditureHeadRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalExpenditureHeadRecord).First().TotalRecordCount,
            };
        }
    }
}