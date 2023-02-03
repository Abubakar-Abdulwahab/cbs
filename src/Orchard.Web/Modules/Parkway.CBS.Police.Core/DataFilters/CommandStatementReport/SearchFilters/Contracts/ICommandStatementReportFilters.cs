using NHibernate;
using NHibernate.Criterion;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CommandStatementReport.SearchFilters.Contracts
{
    public interface ICommandStatementReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, CommandStatementReportSearchParams searchParams);
    }
}
