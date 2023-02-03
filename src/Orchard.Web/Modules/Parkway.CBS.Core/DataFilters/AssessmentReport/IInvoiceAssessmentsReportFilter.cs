using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.AssessmentReport
{
    public interface IInvoiceAssessmentsReportFilter : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyRestrictions"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetReportViewModel(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions);


        IEnumerable<DetailReport> GetReport(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions);


        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<CollectionReportStats> GetAggregate(InvoiceAssessmentSearchParams searchParams, bool applyAccessRestrictions);
    }
}
