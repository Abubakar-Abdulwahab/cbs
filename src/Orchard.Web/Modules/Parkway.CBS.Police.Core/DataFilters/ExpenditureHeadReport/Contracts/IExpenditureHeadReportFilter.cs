using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.Contracts
{
    public interface IExpenditureHeadReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for expenditure head report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalExpenditureHeadRecord }</returns>
        dynamic GetExpenditureHeadReportViewModel(ExpenditureHeadReportSearchParams searchParams);
    }
}
