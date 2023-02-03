using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.Contracts
{
    public interface ICBSUserTaxEntityProfileLocationFilter : IDependency
    {
        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(CBSUserTaxEntityProfileLocationReportSearchParams searchParams);

        /// <summary>
        /// Get CBS User Tax Entity Profile Location report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<CBSUserTaxEntityProfileLocationVM></returns>
        IEnumerable<CBSUserTaxEntityProfileLocationVM> GetReport(CBSUserTaxEntityProfileLocationReportSearchParams searchParams);


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(CBSUserTaxEntityProfileLocationReportSearchParams searchParams);
    }
}
