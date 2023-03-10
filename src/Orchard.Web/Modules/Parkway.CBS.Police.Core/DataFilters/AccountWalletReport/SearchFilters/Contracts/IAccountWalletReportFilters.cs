using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.SearchFilters.Contracts
{
    public interface IAccountWalletReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, AccountWalletReportSearchParams searchParams);
    }
}
