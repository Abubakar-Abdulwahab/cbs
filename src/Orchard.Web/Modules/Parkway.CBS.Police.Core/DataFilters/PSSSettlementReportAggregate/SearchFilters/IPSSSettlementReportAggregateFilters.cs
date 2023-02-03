using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.SearchFilters
{
    public interface IPSSSettlementReportAggregateFilters : IDependency
    {
        /// <summary>
        /// PSS Settlement
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportAggregateSearchParams searchParams);
    }
}
