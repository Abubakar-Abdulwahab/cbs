using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.DataFilters.DirectAssessmentReport.Contracts
{
    public interface IDirectAssessmentRequestReportFilter : IDependency
    {
        /// <summary>
        /// Get report view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(DirectAssessmentSearchParams searchParams);

        /// <summary>
        /// Get direct assessment request report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<TCCRequestVM></returns>
        IEnumerable<DirectAssessmentReportItemsVM> GetReport(DirectAssessmentSearchParams searchParams);

        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetAggregate(DirectAssessmentSearchParams searchParams);
    }
}