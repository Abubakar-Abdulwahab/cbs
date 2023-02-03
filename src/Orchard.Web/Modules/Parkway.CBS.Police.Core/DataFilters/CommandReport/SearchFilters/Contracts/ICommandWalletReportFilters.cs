using NHibernate;
using NHibernate.Criterion;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters.Contracts
{
    public interface ICommandWalletReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, CommandWalletReportSearchParams searchParams);
    }
}
