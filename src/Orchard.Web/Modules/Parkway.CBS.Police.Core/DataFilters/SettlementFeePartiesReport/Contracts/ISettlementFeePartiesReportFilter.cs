using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.SettlementFeePartiesReport.Contracts
{
    public interface ISettlementFeePartiesReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for settlement reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveSettlementFeeParties }</returns>
        dynamic GetSettlementReportViewModel(SettlementFeePartiesSearchParams searchParams);
    }
}
