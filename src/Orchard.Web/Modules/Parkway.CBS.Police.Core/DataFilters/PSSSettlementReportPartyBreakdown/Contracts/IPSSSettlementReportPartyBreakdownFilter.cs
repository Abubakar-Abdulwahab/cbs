using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown.Contracts
{
    public interface IPSSSettlementReportPartyBreakdownFilter : IDependency
    {
        /// <summary>
        /// Get view model for settlement report party breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new {  ReportRecords, TotalNumberOfBatchItems, AmountSettled  }</returns>
        dynamic GetReportViewModel(PSSSettlementReportPartyBreakdownSearchParams searchParams);
    }
}
