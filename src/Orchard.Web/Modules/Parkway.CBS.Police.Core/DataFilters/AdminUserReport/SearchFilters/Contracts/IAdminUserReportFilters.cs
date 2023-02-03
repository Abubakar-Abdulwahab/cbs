using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AdminUserReport.SearchFilters.Contracts
{
    public interface IAdminUserReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, AdminUserReportSearchParams searchParams);
    }
}
