using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.SearchFilters.Contracts
{
    public interface IAccountWalletPaymentReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam);
    }
}
