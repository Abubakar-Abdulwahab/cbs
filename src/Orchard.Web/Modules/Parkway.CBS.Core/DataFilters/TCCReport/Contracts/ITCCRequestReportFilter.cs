using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.DataFilters.TCCReport.Contracts
{
    public interface ITCCRequestReportFilter : IDependency
    {
        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(TCCReportSearchParams searchParams);

        /// <summary>
        /// Get TCC request report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TCCRequestVM></returns>
        IEnumerable<TCCRequestVM> GetReport(TCCReportSearchParams searchParams);


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(TCCReportSearchParams searchParams);
    }
}
