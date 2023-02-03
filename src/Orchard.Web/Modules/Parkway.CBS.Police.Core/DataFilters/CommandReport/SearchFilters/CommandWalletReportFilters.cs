using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters
{
    /// <summary>
    /// Account number filter
    /// </summary>
    public class CommandWalletReportAccountNumberFilter : ICommandWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandWalletReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.AccountNumber))
            {
                criteria.Add(Restrictions.Where<CommandWalletDetails>(x => x.AccountNumber == searchParams.AccountNumber));
            }

        }
    }

    /// <summary>
    /// Command name filter
    /// </summary>
    public class CommandWalletReportCommandNameFilter : ICommandWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandWalletReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CommandName))
            {
                criteria.Add(Restrictions.Where<CommandWalletDetails>(x => x.Command.Name.IsLike("%" + searchParams.CommandName + "%")));
            }

        }
    }

    /// <summary>
    /// Account type filter
    /// </summary>
    public class AccountTypeFilter : ICommandWalletReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandWalletReportSearchParams searchParams)
        {
            if (searchParams.SelectedAccountType > 0)
            {
                criteria.Add(Restrictions.Where<CommandWalletDetails>(x => x.SettlementAccountType == searchParams.SelectedAccountType));
            }

        }
    }
}