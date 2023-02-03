using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportPartyBreakdown.SearchFilters
{
    /// <summary>
    /// Command Filter
    /// </summary>
    public class CommandFilter : IPSSSettlementReportPartyBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportPartyBreakdownSearchParams searchParams)
        {
            if (searchParams.CommandId > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(Command)}.{nameof(Command.Id)}", searchParams.CommandId));
            }
        }
    }
}