using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.SearchFilters.Contracts
{
    public interface IAccountWalletPaymentApprovalFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentApprovalSearchParams searchParam);
    }
}
