using NHibernate;
using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.SearchFilters
{
    public interface IPSSSettlementReportBreakdownFilters : IDependency
    {
        /// <summary>
        /// PSS Settlement
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams);
    }
}
