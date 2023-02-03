using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.Contracts
{
    public interface IDeploymentAllowancePaymentReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for Deployment Allowance payment report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalDeploymentAllowancePaymentReportRecord }</returns>
        dynamic GetDeploymentAllowancePaymentReportViewModel(DeploymentAllowancePaymentSearchParams searchParams);
    }
}
