using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.DataFilters.SettlementRules
{
    public interface ISettlementRulesFilter : IDependency
    {
        /// <summary>
        /// Get aggreagate for the search param
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReportStatsVM> GetAggregate(SettlementRulesSearchParams searchParams);

        /// <summary>
        /// Get settlement rules
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<SettlementRuleLite> GetSettlementRules(SettlementRulesSearchParams searchParams);

        /// <summary>
        /// Get settlement rules list view model
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { RulesRecords, Aggregate }</returns>
        dynamic GetSettlementRulesListViewModel(SettlementRulesSearchParams searchParams);
    }
}
