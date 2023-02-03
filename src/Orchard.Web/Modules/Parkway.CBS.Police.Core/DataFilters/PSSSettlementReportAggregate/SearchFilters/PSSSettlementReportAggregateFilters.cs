using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.SearchFilters
{
    /// <summary>
    /// PSS Settlement
    /// </summary>
    public class SettlementFilter : IPSSSettlementReportAggregateFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportAggregateSearchParams searchParams)
        {
            if (searchParams.SettlementId > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(PSSSettlementBatch.PSSSettlement)}.{nameof(PSSSettlement.Id)}", searchParams.SettlementId));
            }
        }
    }

    /// <summary>
    /// Status
    /// </summary>
    public class StatusFilter : IPSSSettlementReportAggregateFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportAggregateSearchParams searchParams)
        {
            if (searchParams.Status > 0)
            {
                criteria.Add(Restrictions.Eq(nameof(PSSSettlementBatch.Status), searchParams.Status));
            }
        }
    }
}