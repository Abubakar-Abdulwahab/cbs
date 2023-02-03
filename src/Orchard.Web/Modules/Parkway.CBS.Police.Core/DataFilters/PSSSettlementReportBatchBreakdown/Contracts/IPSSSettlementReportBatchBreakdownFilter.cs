using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBatchBreakdown.Contracts
{
    public interface IPSSSettlementReportBatchBreakdownFilter : IDependency
    {
        /// <summary>
        /// Get view model for settlement report batch breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new {  ReportRecords, TotalNumberOfBatchItems, AmountSettled  }</returns>
        dynamic GetReportViewModel(PSSSettlementReportBatchBreakdownSearchParams searchParams);
    }
}
