using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CollectionReport.Contracts
{
    public interface IPoliceCollectionFilter : IDependency
    {

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetRequestReportViewModel(PSSCollectionSearchParams searchParams, bool applyAccessRestrictions);
    }
}
