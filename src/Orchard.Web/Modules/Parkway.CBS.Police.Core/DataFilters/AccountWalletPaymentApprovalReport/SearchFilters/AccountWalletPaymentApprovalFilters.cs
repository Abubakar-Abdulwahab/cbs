using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.SearchFilters
{
    public class AccountWalletPaymentApprovalSourceAccountNameFilters : IAccountWalletPaymentApprovalFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentApprovalSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.SourceAccountName))
            {
                criteria.Add(Restrictions.Or(Restrictions.InsensitiveLike($"{nameof(AccountWalletConfiguration.CommandWalletDetail.Command)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.Command.Name)}", searchParam.SourceAccountName, MatchMode.Anywhere), Restrictions.InsensitiveLike($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Name)}", searchParam.SourceAccountName, MatchMode.Anywhere)));
            }
        }
    }

    public class AccountWalletPaymentApprovalPaymentIdentifierFilters : IAccountWalletPaymentApprovalFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentApprovalSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.PaymentId))
            {
                criteria.Add(Restrictions.Where<AccountPaymentRequest>(x => x.PaymentReference == searchParam.PaymentId));
            }
        }
    }
}