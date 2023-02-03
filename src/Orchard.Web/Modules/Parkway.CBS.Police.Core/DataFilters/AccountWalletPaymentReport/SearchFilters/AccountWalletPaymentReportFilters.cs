using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.SearchFilters
{
    public class AccountWalletPaymentPaymentIdReportFilters : IAccountWalletPaymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.PaymentId))
            {
                criteria.Add(Restrictions.Eq($"{nameof(AccountPaymentRequestItem.AccountPaymentRequest)}.{nameof(AccountPaymentRequestItem.AccountPaymentRequest.PaymentReference)}", searchParam.PaymentId));
            }
        }
    }

    public class AccountWalletPaymentSourceAccountNameReportFilters : IAccountWalletPaymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.SourceAccountName))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(AccountPaymentRequestItem.AccountPaymentRequest)}.{nameof(AccountPaymentRequestItem.AccountPaymentRequest.AccountName)}", searchParam.SourceAccountName, MatchMode.Anywhere));
            }
        }
    }

    public class AccountWalletPaymentBeneficiaryAccountNumberReportFilters : IAccountWalletPaymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.BeneficiaryAccoutNumber))
            {
                criteria.Add(Restrictions.Where<AccountPaymentRequestItem>(x => x.AccountNumber == searchParam.BeneficiaryAccoutNumber));
            }
        }
    }

    public class AccountWalletPaymentExpenditureHeadReportFilters : IAccountWalletPaymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam)
        {
            if (searchParam.ExpenditureHeadId > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(AccountPaymentRequestItem.PSSExpenditureHead)}.{nameof(AccountPaymentRequestItem.AccountPaymentRequest.Id)}", searchParam.ExpenditureHeadId));
            }
        }
    }

    public class AccountWalletPaymentStatusReportFilters : IAccountWalletPaymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletPaymentSearchParams searchParam)
        {
            if (searchParam.Status > 0)
            {
                criteria.Add(Restrictions.Where<AccountPaymentRequestItem>(x => x.TransactionStatus == searchParam.Status));
            }
        }
    }
}