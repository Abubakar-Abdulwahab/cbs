using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.SearchFilters.Contracts
{
    public interface IDeploymentAllowancePaymentReportFilters : IDependency
    {
        /// <summary>
        /// Applies the search filters and gets only items with matching search filter parameters
        /// </summary>
        void AddCriteriaRestriction(ICriteria criteria, DeploymentAllowancePaymentSearchParams searchParam);
    }
}
