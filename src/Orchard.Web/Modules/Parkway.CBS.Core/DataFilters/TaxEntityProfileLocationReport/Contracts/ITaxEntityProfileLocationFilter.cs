using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.Contracts
{
    public interface ITaxEntityProfileLocationFilter : IDependency
    {
        /// <summary>
        /// Get report model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(TaxEntityProfileLocationReportSearchParams searchParams);

        /// <summary>
        /// Get Tax Entity Profile Location report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TaxEntityProfileLocationVM></returns>
        IEnumerable<TaxEntityProfileLocationVM> GetReport(TaxEntityProfileLocationReportSearchParams searchParams);


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(TaxEntityProfileLocationReportSearchParams searchParams);
    }
}
