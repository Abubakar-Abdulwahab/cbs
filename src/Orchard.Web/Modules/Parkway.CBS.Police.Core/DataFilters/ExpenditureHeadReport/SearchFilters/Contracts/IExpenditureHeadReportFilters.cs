using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.SearchFilters.Contracts
{
    public interface IExpenditureHeadReportFilters : IDependency
    {
        void AddCriteriaRestriction(ICriteria criteria, ExpenditureHeadReportSearchParams searchParams);
    }
}
