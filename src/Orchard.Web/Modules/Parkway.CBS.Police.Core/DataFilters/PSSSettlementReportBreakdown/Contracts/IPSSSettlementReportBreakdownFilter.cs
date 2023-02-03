using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.Contracts
{
    public interface IPSSSettlementReportBreakdownFilter : IDependency
    {
        /// <summary>
        /// Get veiw model for settlement report breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalRecordCount, TotalReportAmount }</returns>
        dynamic GetReportViewModel(PSSSettlementReportBreakdownSearchParams searchParams);
    }
}
