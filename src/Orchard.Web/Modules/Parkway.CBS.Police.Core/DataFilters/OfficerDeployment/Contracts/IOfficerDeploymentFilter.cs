using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.Contracts
{
    public interface IOfficerDeploymentFilter : IDependency
    {

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        dynamic GetRequestReportViewModel(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions);

    }
}
