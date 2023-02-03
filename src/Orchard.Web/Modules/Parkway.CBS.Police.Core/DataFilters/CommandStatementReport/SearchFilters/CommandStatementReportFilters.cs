using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandStatementReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters
{
    /// <summary>
    /// Transaction reference filter
    /// </summary>
    public class CommandStatementReportTransactionReferenceFilter : ICommandStatementReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandStatementReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.TransactionReference))
            {
                criteria.Add(Restrictions.Where<WalletStatement>(x => x.TransactionReference == searchParams.TransactionReference));
            }

        }
    }

    /// <summary>
    /// Command statement value date filter
    /// </summary>
    public class CommandStatementReportValueDateFilter : ICommandStatementReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandStatementReportSearchParams searchParams)
        {
            if (searchParams.ValueDate.HasValue)
            {
                criteria.Add(Restrictions.Where<WalletStatement>(x => x.ValueDate.Date == searchParams.ValueDate.Value.Date));
            }

        }
    }

    /// <summary>
    /// Command statement transaction status filter
    /// </summary>
    public class CommandStatementReportTransactionTypeFilter : ICommandStatementReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, CommandStatementReportSearchParams searchParams)
        {
            if (searchParams.TransactionTypeId > 0)
            {
                criteria.Add(Restrictions.Where<WalletStatement>(x => x.TransactionTypeId == searchParams.TransactionTypeId));
            }

        }
    }
}