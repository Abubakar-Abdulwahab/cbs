using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.SearchFilters
{
    public class AccountWalletReportAccountNameFilters : IAccountWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.AccountName))
            {
                criteria.Add(Restrictions.Or(Restrictions.InsensitiveLike($"{nameof(AccountWalletConfiguration.CommandWalletDetail.Command)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.Command.Name)}", searchParams.AccountName, MatchMode.Anywhere), Restrictions.InsensitiveLike($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Name)}", searchParams.AccountName, MatchMode.Anywhere)));
            }
        }
    }

    public class AccountWalletReportAccountNumberFilters : IAccountWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.AccountNumber))
            {
                criteria.Add(Restrictions.Or(Restrictions.Eq($"{nameof(AccountWalletConfiguration.CommandWalletDetail)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.AccountNumber)}", searchParams.AccountNumber), Restrictions.Eq($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.AccountNumber)}", searchParams.AccountNumber)));
            }
        }
    }

    public class AccountWalletReportBankFilters : IAccountWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, AccountWalletReportSearchParams searchParams)
        {
            if (searchParams.BankId > 0)
            {
                criteria.Add(Restrictions.Or(Restrictions.Eq($"BankCWD.{nameof(AccountWalletConfiguration.CommandWalletDetail.Bank.Id)}", searchParams.BankId), Restrictions.Eq($"{nameof(AccountWalletConfiguration.PSSFeeParty.Bank)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Bank.Id)}", searchParams.BankId)));
            }
        }
    }
}