using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementReport.Contracts
{
    public interface ISettlementReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for settlement reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveSettlement }</returns>
        dynamic GetSettlementReportViewModel(SettlementReportSearchParams searchParams);
    }
}
